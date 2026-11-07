using System.Collections.Generic;
using SYBO.Subway.Routes;
using UnityEngine;

// Opções de treino das Configurações: Modo de Treino, Tipo de Impossível e Não Morrer.
public static class TrainingOptions
{
	public enum Mode
	{
		TreinoMisto = 0,
		Pogostick = 1,
		Nerdola = 2,
		TresFileiras = 3,
		Impossiveis = 4,
		Quininha = 5,
		RachaCuca = 6,
		TreinoDeObstaculos = 7,
		TreinoNormal = 8,
		TremEmMovimento = 9,
		TrensEmMovimento = 10
	}

	public enum ImpossibleType
	{
		Misto = 0,
		Fechada = 1,
		Aberta = 2,
		TresBarreiras = 3,
		Francesa = 4
	}

	private const string MODE_KEY = "gj_training_mode";

	private const string IMPOSSIBLE_KEY = "gj_training_impossible";

	private const string NO_DEATH_KEY = "gj_training_no_death";

	// Ordem em que aparecem no menu.
	public static readonly Mode[] ModeOrder = new Mode[11]
	{
		Mode.TreinoMisto,
		Mode.Pogostick,
		Mode.Nerdola,
		Mode.TresFileiras,
		Mode.Impossiveis,
		Mode.Quininha,
		Mode.RachaCuca,
		Mode.TreinoDeObstaculos,
		Mode.TreinoNormal,
		Mode.TremEmMovimento,
		Mode.TrensEmMovimento
	};

	public static readonly ImpossibleType[] ImpossibleOrder = new ImpossibleType[5]
	{
		ImpossibleType.Misto,
		ImpossibleType.Fechada,
		ImpossibleType.Aberta,
		ImpossibleType.TresBarreiras,
		ImpossibleType.Francesa
	};

	// Trechos (route sections) que cada modo usa, pelo nome do prefab. Vazio = pista normal do jogo.
	// Três Fileiras: só os trechos "bonus" (moedas nas três faixas, sem obstáculos), igual ao /miami-treino do site.
	private static readonly Dictionary<Mode, string[]> SectionsByMode = new Dictionary<Mode, string[]>
	{
		{
			Mode.TresFileiras,
			new string[2] { "routeSection_default_bonus_long", "routeSection_default_bonus_short" }
		},
		{
			Mode.TremEmMovimento,
			new string[2] { "route_section_default_train_tops_moving", "route_section_default_short_train_tops_moving" }
		},
		{
			Mode.TrensEmMovimento,
			new string[4] { "route_section_default_train_tops_moving_multiple", "route_section_default_train_tops_moving_combined", "route_section_default_short_train_tops_moving_multiple", "route_section_default_short_train_tops_moving_combined" }
		}
	};

	public static Mode CurrentMode
	{
		get
		{
			int value = PlayerPrefs.GetInt(MODE_KEY, (int)Mode.Impossiveis);
			return (!System.Enum.IsDefined(typeof(Mode), value)) ? Mode.Impossiveis : ((Mode)value);
		}
		set
		{
			PlayerPrefs.SetInt(MODE_KEY, (int)value);
			PlayerPrefs.Save();
		}
	}

	public static ImpossibleType CurrentImpossibleType
	{
		get
		{
			int value = PlayerPrefs.GetInt(IMPOSSIBLE_KEY, (int)ImpossibleType.Misto);
			return (!System.Enum.IsDefined(typeof(ImpossibleType), value)) ? ImpossibleType.Misto : ((ImpossibleType)value);
		}
		set
		{
			PlayerPrefs.SetInt(IMPOSSIBLE_KEY, (int)value);
			PlayerPrefs.Save();
		}
	}

#if UNITY_WEBGL && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern int GjIsOriginalMode();
#else
	private static int GjIsOriginalMode()
	{
		return 0;
	}
#endif

	private static int _original = -1;

	// Modo original do site (?mode=original): sem botões de treino, sem burlas e sem Não Morrer
	public static bool OriginalMode
	{
		get
		{
			if (_original < 0)
			{
				_original = GjIsOriginalMode();
			}
			return _original == 1;
		}
	}

	public static bool NoDeath
	{
		get
		{
			if (OriginalMode)
			{
				return false;
			}
			return PlayerPrefs.GetInt(NO_DEATH_KEY, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(NO_DEATH_KEY, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ToggleNoDeath()
	{
		NoDeath = !NoDeath;
	}

	private const string AUTO_BOOST_KEY = "gj_auto_boost";

	public static bool AutoBoost
	{
		get
		{
			return PlayerPrefs.GetInt(AUTO_BOOST_KEY, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(AUTO_BOOST_KEY, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ToggleAutoBoost()
	{
		AutoBoost = !AutoBoost;
	}

	// Brilho das moedas (o halo em volta de cada moeda na pista). Vem desligado.
	private const string COIN_GLOW_KEY = "gj_coin_glow";

	public static bool CoinGlow
	{
		get
		{
			return PlayerPrefs.GetInt(COIN_GLOW_KEY, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(COIN_GLOW_KEY, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ToggleCoinGlow()
	{
		CoinGlow = !CoinGlow;
	}

	// Textura da moeda: Clássica (a do jogo, padrão) ou Atual (a das versões novas).
	private const string COIN_TEXTURE_KEY = "gj_coin_texture";

	public static bool CoinTextureAtual
	{
		get
		{
			return PlayerPrefs.GetInt(COIN_TEXTURE_KEY, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(COIN_TEXTURE_KEY, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ToggleCoinTexture()
	{
		CoinTextureAtual = !CoinTextureAtual;
	}

	private static Material _coinAtualMaterial;

	private static Material _coinClassicMaterial;

	// Material da moeda conforme a opção. A Atual é uma cópia da textura de objetos (props_tex) com a
	// face da moeda trocada (Resources/gj_coin_atual.png): o resto da textura fica igual.
	public static Material CoinMaterial(Material classic)
	{
		if (classic == null)
		{
			return null;
		}
		if (_coinClassicMaterial == null)
		{
			_coinClassicMaterial = classic;
		}
		if (!CoinTextureAtual)
		{
			return _coinClassicMaterial;
		}
		if (_coinAtualMaterial == null)
		{
			Texture2D texture = Resources.Load<Texture2D>("gj_coin_atual");
			if (texture == null)
			{
				return _coinClassicMaterial;
			}
			_coinAtualMaterial = new Material(_coinClassicMaterial);
			_coinAtualMaterial.name = "gj_coin_atual";
			_coinAtualMaterial.mainTexture = texture;
		}
		return _coinAtualMaterial;
	}

	private const string TRIPLE_TAP_KEY = "gj_triple_tap_board";

	// Prancha com 3 toques: a prancha só liga no terceiro toque seguido, em vez de dois.
	public static bool TripleTapBoard
	{
		get
		{
			return PlayerPrefs.GetInt(TRIPLE_TAP_KEY, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(TRIPLE_TAP_KEY, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ToggleTripleTapBoard()
	{
		TripleTapBoard = !TripleTapBoard;
	}

	// Qualidade gráfica do ambiente (céu, trilhos, trens...): 0 = Baixa, 1 = Alta (padrão).
	// Fica no localStorage do navegador ("gj-graficos").
#if UNITY_WEBGL && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern int GjGetGraphics();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void GjSetGraphics(int level);
#else
	private static int GjGetGraphics()
	{
		return PlayerPrefs.GetInt("gj_graphics", 1);
	}

	private static void GjSetGraphics(int level)
	{
		PlayerPrefs.SetInt("gj_graphics", level);
	}
#endif

	private static int _graphicsLevel = -1;

	public static int GraphicsLevel
	{
		get
		{
			if (_graphicsLevel < 0)
			{
				_graphicsLevel = GjGetGraphics() == 0 ? 0 : 1;
			}
			return _graphicsLevel;
		}
	}

	public static bool GraphicsHigh
	{
		get
		{
			return GraphicsLevel > 0;
		}
	}

	public static string GraphicsName(int level)
	{
		return level == 0 ? Tr("GFX_LOW") : Tr("GFX_HIGH");
	}

	// Baixa <-> Alta. O jogo monta o cenário com a qualidade escolhida quando abre,
	// então trocar salva e recarrega a página (volta ao menu com o save no lugar).
	public static void ToggleGraphics()
	{
		int next = GraphicsLevel == 0 ? 1 : 0;
		_graphicsLevel = next;
		PlayerPrefs.Save();
		GjSetGraphics(next);
	}

	// Só o ambiente muda (DeviceInfo.performanceLevel: materiais e modelos do cenário).
	// Interface, personagem e resolução continuam iguais.
	public static void ApplyGraphics()
	{
		QualitySettings.globalTextureMipmapLimit = 0;
	}

	// Ícones do atlas têm tamanhos diferentes; deixa todos do tamanho dos ícones de música/som.
	public static void FitIcon(UISprite sprite)
	{
		sprite.MakePixelPerfect();
		float scale = 46f / Mathf.Max(sprite.width, sprite.height);
		if (scale < 1f)
		{
			sprite.width = Mathf.RoundToInt(sprite.width * scale);
			sprite.height = Mathf.RoundToInt(sprite.height * scale);
		}
	}

	// Traduções dos botões novos. Chave = idioma do jogo (pt, en, es, fr, ...). Sem tradução, usa português.
	// Racha Cuca, Quininha, Nerdola e Pogostick ficam com o mesmo nome em todos os idiomas, e a Francesa também
	// (menos em francês, que vira "Pièce Colèe").
	private static readonly Dictionary<string, Dictionary<string, string>> Texts = new Dictionary<string, Dictionary<string, string>>
	{
		{
			"pt",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "Modo de Treino" },
				{ "IMPOSSIBLE_TYPE", "Tipo de Impossível" },
				{ "NO_DEATH", "Não Morrer" },
				{ "AUTO_BOOST", "Vantagem Automática" },
				{ "COIN_GLOW", "Brilho das Moedas" },
				{ "COIN_TEXTURE", "Textura da Moeda" },
				{ "COIN_CLASSIC", "Clássica" },
				{ "COIN_CURRENT", "Atual" },
				{ "TRIPLE_TAP", "Prancha com 3 Toques" },
				{ "GRAPHICS", "Qualidade Gráfica" },
				{ "GFX_LOW", "Baixa" },
				{ "GFX_MEDIUM", "Média" },
				{ "GFX_HIGH", "Alta" },
				{ "MODE_MIXED", "Treino Misto" },
				{ "MODE_THREE_ROWS", "Três Fileiras" },
				{ "MODE_IMPOSSIBLES", "Impossíveis" },
				{ "MODE_OBSTACLES", "Treino de Obstáculos" },
				{ "MODE_NORMAL", "Treino Normal" },
				{ "MODE_MOVING_TRAIN", "Trem em Movimento" },
				{ "MODE_MOVING_TRAINS", "Trens em Movimento" },
				{ "IMP_MIXED", "Misto" },
				{ "IMP_CLOSED", "Fechada" },
				{ "IMP_OPEN", "Aberta" },
				{ "IMP_THREE_BARRIERS", "Três Barreiras" },
				{ "IMP_FRENCH", "Francesa" }
			}
		},
		{
			"en",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "Training Mode" },
				{ "IMPOSSIBLE_TYPE", "Impossible Type" },
				{ "NO_DEATH", "No Death" },
				{ "AUTO_BOOST", "Auto Headstart" },
				{ "COIN_GLOW", "Coin Glow" },
				{ "COIN_TEXTURE", "Coin Texture" },
				{ "COIN_CLASSIC", "Classic" },
				{ "COIN_CURRENT", "Current" },
				{ "TRIPLE_TAP", "Triple Tap Hoverboard" },
				{ "GRAPHICS", "Graphics Quality" },
				{ "GFX_LOW", "Low" },
				{ "GFX_MEDIUM", "Medium" },
				{ "GFX_HIGH", "High" },
				{ "MODE_MIXED", "Mixed Training" },
				{ "MODE_THREE_ROWS", "Three Rows" },
				{ "MODE_IMPOSSIBLES", "Impossibles" },
				{ "MODE_OBSTACLES", "Obstacle Training" },
				{ "MODE_NORMAL", "Normal Training" },
				{ "MODE_MOVING_TRAIN", "Moving Train" },
				{ "MODE_MOVING_TRAINS", "Moving Trains" },
				{ "IMP_MIXED", "Mixed" },
				{ "IMP_CLOSED", "Closed" },
				{ "IMP_OPEN", "Open" },
				{ "IMP_THREE_BARRIERS", "Three Barriers" },
				{ "IMP_FRENCH", "Francesa" }
			}
		},
		{
			"es",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "Modo de Entrenamiento" },
				{ "IMPOSSIBLE_TYPE", "Tipo de Imposible" },
				{ "NO_DEATH", "No Morir" },
				{ "AUTO_BOOST", "Ventaja Automática" },
				{ "COIN_GLOW", "Brillo de Monedas" },
				{ "COIN_TEXTURE", "Textura de Moneda" },
				{ "COIN_CLASSIC", "Clásica" },
				{ "COIN_CURRENT", "Actual" },
				{ "TRIPLE_TAP", "Tabla con 3 Toques" },
				{ "GRAPHICS", "Calidad Gráfica" },
				{ "GFX_LOW", "Baja" },
				{ "GFX_MEDIUM", "Media" },
				{ "GFX_HIGH", "Alta" },
				{ "MODE_MIXED", "Entrenamiento Mixto" },
				{ "MODE_THREE_ROWS", "Tres Filas" },
				{ "MODE_IMPOSSIBLES", "Imposibles" },
				{ "MODE_OBSTACLES", "Entrenamiento de Obstáculos" },
				{ "MODE_NORMAL", "Entrenamiento Normal" },
				{ "MODE_MOVING_TRAIN", "Tren en Movimiento" },
				{ "MODE_MOVING_TRAINS", "Trenes en Movimiento" },
				{ "IMP_MIXED", "Mixto" },
				{ "IMP_CLOSED", "Cerrada" },
				{ "IMP_OPEN", "Abierta" },
				{ "IMP_THREE_BARRIERS", "Tres Barreras" },
				{ "IMP_FRENCH", "Francesa" }
			}
		},
		{
			"fr",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "Mode Entraînement" },
				{ "IMPOSSIBLE_TYPE", "Type d'Impossible" },
				{ "NO_DEATH", "Immortel" },
				{ "AUTO_BOOST", "Départ Turbo Auto" },
				{ "COIN_GLOW", "Éclat des Pièces" },
				{ "COIN_TEXTURE", "Texture des Pièces" },
				{ "COIN_CLASSIC", "Classique" },
				{ "COIN_CURRENT", "Actuelle" },
				{ "TRIPLE_TAP", "Hoverboard en 3 Touches" },
				{ "GRAPHICS", "Qualité Graphique" },
				{ "GFX_LOW", "Basse" },
				{ "GFX_MEDIUM", "Moyenne" },
				{ "GFX_HIGH", "Haute" },
				{ "MODE_MIXED", "Entraînement Mixte" },
				{ "MODE_THREE_ROWS", "Trois Rangées" },
				{ "MODE_IMPOSSIBLES", "Impossibles" },
				{ "MODE_OBSTACLES", "Entraînement d'Obstacles" },
				{ "MODE_NORMAL", "Entraînement Normal" },
				{ "MODE_MOVING_TRAIN", "Train en Mouvement" },
				{ "MODE_MOVING_TRAINS", "Trains en Mouvement" },
				{ "IMP_MIXED", "Mixte" },
				{ "IMP_CLOSED", "Fermée" },
				{ "IMP_OPEN", "Ouverte" },
				{ "IMP_THREE_BARRIERS", "Trois Barrières" },
				{ "IMP_FRENCH", "Pièce Colèe" }
			}
		},
		{
			"de",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "Trainingsmodus" },
				{ "IMPOSSIBLE_TYPE", "Unmöglich-Typ" },
				{ "NO_DEATH", "Unsterblich" },
				{ "AUTO_BOOST", "Auto-Starthilfe" },
				{ "COIN_GLOW", "Münzglanz" },
				{ "COIN_TEXTURE", "Münztextur" },
				{ "COIN_CLASSIC", "Klassisch" },
				{ "COIN_CURRENT", "Aktuell" },
				{ "TRIPLE_TAP", "Hoverboard mit 3 Taps" },
				{ "GRAPHICS", "Grafikqualität" },
				{ "GFX_LOW", "Niedrig" },
				{ "GFX_MEDIUM", "Mittel" },
				{ "GFX_HIGH", "Hoch" },
				{ "MODE_MIXED", "Gemischtes Training" },
				{ "MODE_THREE_ROWS", "Drei Reihen" },
				{ "MODE_IMPOSSIBLES", "Unmögliche" },
				{ "MODE_OBSTACLES", "Hindernistraining" },
				{ "MODE_NORMAL", "Normales Training" },
				{ "MODE_MOVING_TRAIN", "Fahrender Zug" },
				{ "MODE_MOVING_TRAINS", "Fahrende Züge" },
				{ "IMP_MIXED", "Gemischt" },
				{ "IMP_CLOSED", "Geschlossen" },
				{ "IMP_OPEN", "Offen" },
				{ "IMP_THREE_BARRIERS", "Drei Barrieren" },
				{ "IMP_FRENCH", "Francesa" }
			}
		},
		{
			"zh",
			new Dictionary<string, string>
			{
				{ "TRAINING_MODE", "训练模式" },
				{ "IMPOSSIBLE_TYPE", "不可能类型" },
				{ "NO_DEATH", "不会死亡" },
				{ "AUTO_BOOST", "自动起跑加速" },
				{ "COIN_GLOW", "金币闪光" },
				{ "COIN_TEXTURE", "金币样式" },
				{ "COIN_CLASSIC", "经典" },
				{ "COIN_CURRENT", "当前" },
				{ "TRIPLE_TAP", "三击滑板" },
				{ "GRAPHICS", "画质" },
				{ "GFX_LOW", "低" },
				{ "GFX_MEDIUM", "中" },
				{ "GFX_HIGH", "高" },
				{ "MODE_MIXED", "混合训练" },
				{ "MODE_THREE_ROWS", "三排" },
				{ "MODE_IMPOSSIBLES", "不可能" },
				{ "MODE_OBSTACLES", "障碍训练" },
				{ "MODE_NORMAL", "普通训练" },
				{ "MODE_MOVING_TRAIN", "移动火车" },
				{ "MODE_MOVING_TRAINS", "多辆移动火车" },
				{ "IMP_MIXED", "混合" },
				{ "IMP_CLOSED", "封闭" },
				{ "IMP_OPEN", "开放" },
				{ "IMP_THREE_BARRIERS", "三道栅栏" },
				{ "IMP_FRENCH", "Francesa" }
			}
		}
	};

	public static string Tr(string key)
	{
		string language = "pt";
		try
		{
			if (Strings.language != null && Strings.language.locale != null && !string.IsNullOrEmpty(Strings.language.locale.language))
			{
				language = Strings.language.locale.language;
			}
		}
		catch (System.Exception)
		{
		}
		Dictionary<string, string> table;
		string text;
		if (Texts.TryGetValue(language, out table) && table.TryGetValue(key, out text))
		{
			return text;
		}
		return Texts["pt"].TryGetValue(key, out text) ? text : key;
	}

	public static string GetName(Mode mode)
	{
		switch (mode)
		{
		case Mode.TreinoMisto:
			return Tr("MODE_MIXED");
		case Mode.Pogostick:
			return "Pogostick";
		case Mode.Nerdola:
			return "Nerdola";
		case Mode.TresFileiras:
			return Tr("MODE_THREE_ROWS");
		case Mode.Impossiveis:
			return Tr("MODE_IMPOSSIBLES");
		case Mode.Quininha:
			return "Quininha";
		case Mode.RachaCuca:
			return "Racha Cuca";
		case Mode.TreinoDeObstaculos:
			return Tr("MODE_OBSTACLES");
		case Mode.TreinoNormal:
			return Tr("MODE_NORMAL");
		case Mode.TremEmMovimento:
			return Tr("MODE_MOVING_TRAIN");
		case Mode.TrensEmMovimento:
			return Tr("MODE_MOVING_TRAINS");
		default:
			return mode.ToString();
		}
	}

	public static string GetName(ImpossibleType type)
	{
		switch (type)
		{
		case ImpossibleType.Misto:
			return Tr("IMP_MIXED");
		case ImpossibleType.Fechada:
			return Tr("IMP_CLOSED");
		case ImpossibleType.Aberta:
			return Tr("IMP_OPEN");
		case ImpossibleType.TresBarreiras:
			return Tr("IMP_THREE_BARRIERS");
		case ImpossibleType.Francesa:
			return Tr("IMP_FRENCH");
		default:
			return type.ToString();
		}
	}

	// Chamado pelo RouteBuilder antes de sortear o próximo trecho. Null = segue o sorteio normal do jogo.
	// As burlas ainda não foram definidas: o treino usa a pista do modo original (igual gjsantolin22.vercel.app/miami).
	// Para ligar as burlas, é só trocar para true e preencher SectionsByMode.
	private static readonly bool BurlasEnabled = false;

	public static RouteSection PickMidSection(WeightRandomizedRouteSectionContainter midSections)
	{
		if (!BurlasEnabled)
		{
			return null;
		}
		string[] names;
		if (OriginalMode || !SectionsByMode.TryGetValue(CurrentMode, out names) || names.Length == 0)
		{
			return null;
		}
		List<RouteSection> matches = new List<RouteSection>();
		foreach (WeightedRouteSection weighted in midSections)
		{
			if (weighted != null && weighted.Prefab != null && System.Array.IndexOf(names, weighted.Prefab.name) >= 0)
			{
				matches.Add(weighted.Prefab);
			}
		}
		if (matches.Count == 0)
		{
			return null;
		}
		return matches[Random.Range(0, matches.Count)];
	}
}
