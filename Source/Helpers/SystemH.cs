using System;
using System.IO;
using System.Runtime;
using System.Threading;

namespace Helpers {
	public static class SystemH {
		private static Mutex globalLock;

		public static void ForceGarbageCollection() {
			GC.Collect();
		}

		public static long GetGarbageSize() {
			return GC.GetTotalAllocatedBytes();
		}

		public static long GetMemorySize() {
			return GC.GetTotalMemory(false);
		}

		public static void SetGcLowLatency() {
			GCSettings.LatencyMode = GCLatencyMode.LowLatency;
		}

		public static void SetGcInteractive() {
			GCSettings.LatencyMode = GCLatencyMode.Interactive;
		}

		public static bool TryCreateUniqueMutex(in string guid) {
			globalLock = new Mutex(false, guid);
			return globalLock.WaitOne(0, false);
		}

		public static void LogExceptionsIntoFile() {
			AppDomain.CurrentDomain.UnhandledException += WriteExceptionIntoFile;
		}

		public static void WriteExceptionIntoFile(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = e.ExceptionObject as Exception;
			Console.WriteLine(ex.Message);

			string filePath = $"{Directory.GetCurrentDirectory()}/ExceptionLog.txt";
			using (StreamWriter writer = new StreamWriter(filePath, true)) {
				writer.WriteLine("-----------------------------------------------------------------------------");
				writer.WriteLine($"Date : {DateTime.Now}");
				writer.WriteLine();

				while (ex != null) {
					writer.WriteLine(ex.GetType().FullName);
					writer.WriteLine($"Message : {ex.Message}");
					writer.WriteLine($"StackTrace : {ex.StackTrace}");

					ex = ex.InnerException;
				}
			}
		}
	}
}