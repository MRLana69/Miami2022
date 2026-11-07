using System;
using System.Threading;
using SYBO.Components.Base;

namespace SYBO.Components.Utilities
{
	public class EnvironmentUtility
	{
		public const SYBO.Components.Utilities.ActiveTargetPlatform ActiveTargetPlatform = SYBO.Components.Utilities.ActiveTargetPlatform.Android;

		public const string ActiveTargetPlatformString = "android";

		public static bool IsMainThread
		{
			get
			{
				return Thread.CurrentThread.Equals(Thread.CurrentThread);
			}
		}

		public static bool IsRunningUnitTests
		{
			get
			{
				return BaseEnvironment.IsRunningUnitTests;
			}
		}

		public static string BuildNumber
		{
			get
			{
				return GetEnvironmentVariable("BUILD_NUMBER");
			}
		}

		public static string Workspace
		{
			get
			{
				return GetEnvironmentVariable("WORKSPACE");
			}
		}

		public static string Path
		{
			get
			{
				return GetEnvironmentVariable("PATH");
			}
		}

		public static string GitCommit
		{
			get
			{
				return GetEnvironmentVariable("GIT_COMMIT");
			}
		}

		public static string GitBranch
		{
			get
			{
				return GetEnvironmentVariable("GIT_BRANCH");
			}
		}

		public static bool IsBatchMode()
		{
			string commandLine = Environment.CommandLine;
			if (commandLine.Contains("-batchmode"))
			{
				return true;
			}
			return false;
		}

		public static bool HasEnvironmentVariable(string name)
		{
			return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name));
		}

		public static string GetEnvironmentVariable(string name)
		{
			return Environment.GetEnvironmentVariable(name);
		}

		public static void SetEnvironmentVariable(string name, string value)
		{
			Environment.SetEnvironmentVariable(name, value);
		}
	}
}
