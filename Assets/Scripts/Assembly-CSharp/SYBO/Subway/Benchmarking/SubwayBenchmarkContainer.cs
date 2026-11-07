using System.Collections.Generic;

namespace SYBO.Subway.Benchmarking
{
	public static class SubwayBenchmarkContainer
	{
		private class BenchmarkWrapper
		{
			public string BenchmarkName;

			public SubwayBenchmark Benchmark;

			public string ProcessorSheetNamePostfix;
		}

		private static readonly List<BenchmarkWrapper> Benchmarks = new List<BenchmarkWrapper>
		{
			new BenchmarkWrapper
			{
				BenchmarkName = "Perf. Standard",
				Benchmark = new SubwayRunningBenchmark(),
				ProcessorSheetNamePostfix = "perf"
			},
			new BenchmarkWrapper
			{
				BenchmarkName = "Perf. Long",
				Benchmark = new SubwayRunningBenchmark(7200f, 6f),
				ProcessorSheetNamePostfix = "perf_long"
			},
			new BenchmarkWrapper
			{
				BenchmarkName = "Memory Standard",
				Benchmark = new SubwayMemoryBenchmark(false, false),
				ProcessorSheetNamePostfix = "memory"
			},
			new BenchmarkWrapper
			{
				BenchmarkName = "Memory All Chars",
				Benchmark = new SubwayMemoryBenchmark(true, true),
				ProcessorSheetNamePostfix = "memory_all"
			}
		};

		public static int SelectedBenchmarkIndex { get; private set; }

		public static List<string> BenchmarkNames
		{
			get
			{
				return Benchmarks.ConvertAll((BenchmarkWrapper bw) => bw.BenchmarkName);
			}
		}

		public static string SelectedBenchmarkName
		{
			get
			{
				return Benchmarks[SelectedBenchmarkIndex].BenchmarkName;
			}
			set
			{
				int num = Benchmarks.FindIndex((BenchmarkWrapper bw) => bw.BenchmarkName.Equals(value));
				if (num < 0)
				{
					num = 0;
				}
				SelectedBenchmarkIndex = num;
			}
		}

		public static SubwayBenchmark SelectedBenchmark
		{
			get
			{
				return Benchmarks[SelectedBenchmarkIndex].Benchmark;
			}
		}

		public static string SelectedBencharmProcessorPostfix
		{
			get
			{
				return Benchmarks[SelectedBenchmarkIndex].ProcessorSheetNamePostfix;
			}
		}
	}
}
