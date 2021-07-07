using System.IO;

namespace Helpers {
	public static class FileH {
		public static void CreateFileIfNotExists(in string filePath) {
			if (!File.Exists(filePath)) {
				using (FileStream fileStream = File.Create(filePath)) { }
			}
		}
	}
}