using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

// Build WebGL: execute -executeMethod GjBuild.WebGL. O modo por arquivo-gatilho é opcional
// e usa a variável de ambiente GJBUILD_TRIGGER.
[InitializeOnLoad]
public static class GjBuild
{
	// Caminhos configuráveis por variáveis de ambiente para funcionar no Windows e no GitHub Actions/Linux.
	static string Trigger => System.Environment.GetEnvironmentVariable("GJBUILD_TRIGGER");
	static string Output => System.Environment.GetEnvironmentVariable("GJBUILD_OUTPUT")
		?? Path.Combine(Directory.GetCurrentDirectory(), "build", "WebGL");
	static string ResultFile => System.Environment.GetEnvironmentVariable("GJBUILD_RESULT")
		?? Path.Combine(Directory.GetCurrentDirectory(), "build", "gjbuild-result.txt");
	static double _next;

	static GjBuild()
	{
		EditorApplication.update += Poll;
	}

	static void Poll()
	{
		if (EditorApplication.timeSinceStartup < _next) return;
		_next = EditorApplication.timeSinceStartup + 1;
		if (string.IsNullOrEmpty(Trigger) || !File.Exists(Trigger) || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) return;
		bool dev = File.ReadAllText(Trigger).Contains("dev");
		File.Delete(Trigger);
		EditorApplication.delayCall += () => Run(dev, false);
	}

	public static void WebGL()
	{
		Run(System.Environment.GetCommandLineArgs().Contains("-gjdev"), true);
	}

	// Os materiais dos pacotes de personagens procuram o shader pelo nome (Shader.Find), então os shaders
	// refeitos de Assets/Shader precisam ir no build mesmo sem nenhuma cena usar
	static void IncludeProjectShaders()
	{
		var graphics = new SerializedObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("ProjectSettings/GraphicsSettings.asset"));
		var list = graphics.FindProperty("m_AlwaysIncludedShaders");
		var have = Enumerable.Range(0, list.arraySize).Select(i => list.GetArrayElementAtIndex(i).objectReferenceValue).ToList();
		foreach (string guid in AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shader", "Assets/Resources/shaders" }))
		{
			var shader = AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(guid));
			if (shader == null || have.Contains(shader)) continue;
			list.InsertArrayElementAtIndex(list.arraySize);
			list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = shader;
			have.Add(shader);
		}
		graphics.ApplyModifiedProperties();
		AssetDatabase.SaveAssets();
	}


	static void Run(bool dev, bool exit)
	{
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
		// Os shaders refeitos são GLSL (GLSLPROGRAM) do build original, então só WebGL 2.0
		PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
		PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES3 });
		PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
		PlayerSettings.WebGL.decompressionFallback = false;
		// O código do jogo conta com exceções de verdade (ex.: null) para seguir em frente; sem isso alguns
		// sistemas (sons como o de morrer) ficam quebrados. Sem o rastreio de pilha, que é a parte cara.
		PlayerSettings.WebGL.exceptionSupport = dev ? WebGLExceptionSupport.FullWithStacktrace : WebGLExceptionSupport.FullWithoutStacktrace;
		// Página própria (Assets/WebGLTemplates/GjMiami): PC e celular, scripts do site, sem aviso de "não suportado"
		PlayerSettings.WebGL.template = "PROJECT:GjMiami";
		PlayerSettings.runInBackground = true;
		// Assets/link.xml guarda o código do jogo; o resto só é removido no nível mínimo
		PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Minimal);
		// Otimização (FPS): código C++ no modo mais rápido e sem símbolos de depuração na build final
		PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, dev ? Il2CppCompilerConfiguration.Debug : Il2CppCompilerConfiguration.Master);
		PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.WebGL, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSpeed);
		PlayerSettings.WebGL.debugSymbolMode = dev ? WebGLDebugSymbolMode.External : WebGLDebugSymbolMode.Off;
		PlayerSettings.WebGL.dataCaching = true;
		PlayerSettings.stripEngineCode = true;
		// Memória: começa pequena e cresce conforme precisa (celular aguenta melhor)
		PlayerSettings.WebGL.initialMemorySize = 64;
		PlayerSettings.WebGL.memoryGrowthMode = WebGLMemoryGrowthMode.Geometric;
		PlayerSettings.WebGL.maximumMemorySize = 2048;
		PlayerSettings.WebGL.powerPreference = WebGLPowerPreference.HighPerformance;

		IncludeProjectShaders();

		var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		if (scenes.Length == 0)
			throw new System.InvalidOperationException("GjBuild: nenhuma cena está habilitada em Build Settings.");
		Directory.CreateDirectory(Output);
		Directory.CreateDirectory(Path.GetDirectoryName(ResultFile));
		var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = Output,
			target = BuildTarget.WebGL,
			options = dev ? BuildOptions.Development : BuildOptions.None
		});
		// os scripts do site agora vêm do template (Assets/WebGLTemplates/GjMiami)
		string result = "GJBUILD RESULT: " + report.summary.result + " errors=" + report.summary.totalErrors + " dev=" + dev;
		Debug.Log(result);
		File.WriteAllText(ResultFile, result + "\n" + System.DateTime.Now);
		if (exit) EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
	}
}
