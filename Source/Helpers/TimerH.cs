using System;
using System.Diagnostics;
using System.Threading;

namespace Helpers {
	public static class TimerH {
		private static ThreadLocal<Stopwatch> stopwatch = new ThreadLocal<Stopwatch>(() => new Stopwatch());

		public static void StartTimer() {
			stopwatch.Value.Start();
		}

		public static void PrintTimer() {
			if (stopwatch.Value.IsRunning)
				Console.WriteLine(stopwatch.Value.Elapsed.TotalSeconds);
			else
				Console.WriteLine("Stopwatch not running");
		}

		public static void RestartTimer() {
			if (stopwatch.Value.IsRunning) {
				stopwatch.Value.Stop();
				Console.WriteLine(stopwatch.Value.Elapsed.TotalSeconds);
				stopwatch.Value.Restart();
			} else {
				Console.WriteLine("Stopwatch not running");
			}
		}

		public static void StopTimer() {
			if (stopwatch.Value.IsRunning) {
				stopwatch.Value.Stop();
				Console.WriteLine(stopwatch.Value.Elapsed.TotalSeconds);
				stopwatch.Value.Reset();
			} else {
				Console.WriteLine("Stopwatch not running");
			}
		}
	}
}