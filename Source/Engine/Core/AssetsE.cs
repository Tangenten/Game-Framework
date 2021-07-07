using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TangentEngine {
	public class AssetsE {
		private Dictionary<string, Asset> streamCache;

		public AssetsE() {
			this.streamCache = new Dictionary<string, Asset>();
		}

		public void Update() {
			foreach (KeyValuePair<string, Asset> keyValuePair in this.streamCache) {
				keyValuePair.Value.Update();
			}
		}

		public Stream GetStream(in string resourceName, Action onFileModify = null) {
			lock (this.streamCache) {
				if (!this.streamCache.ContainsKey(resourceName)) {
					this.streamCache[resourceName] = new Asset(resourceName, onFileModify);
				}
			}

			return this.streamCache[resourceName].fileStream;
		}

		public void GetStreamAsync(string resourceName, Action<Stream> onDoneLoading, Action onFileModify = null) {
			Task.Run(() => {
				lock (this.streamCache) {
					if (!this.streamCache.ContainsKey(resourceName)) {
						this.streamCache[resourceName] = new Asset(resourceName, onFileModify);
					}
				}

				onDoneLoading.Invoke(this.streamCache[resourceName].fileStream);
			});
		}

		public void UnloadStream(in string resourceName) {
			lock (this.streamCache) {
				if (this.streamCache.ContainsKey(resourceName)) {
					this.streamCache.Remove(resourceName);
				}
			}
		}

		public void UnloadAllStreams() {
			this.streamCache.Clear();
		}

		public void Reset() {
			this.streamCache.Clear();
		}
	}

	public class Asset {
		private Action? fileModifiedCallback;
		private DateTime fileModifiedDate;

		private string filePath;
		public Stream fileStream;

		// File or Embedded Resource
		private bool isFile;

		public Asset(in string filePath, Action onFileModify = null) {
			this.filePath = filePath;
			this.fileModifiedCallback = onFileModify;
			this.LoadStream();
		}

		public void LoadStream() {
			if (this.filePath.Contains("\\")) {
				this.fileStream = new MemoryStream();
				// While try to load file over and over again, usually a file will get locked for a short while
				// as the file is being saved in another program
				while (true) {
					try {
						File.Open(this.filePath,
							FileMode.Open,
							FileAccess.Read,
							FileShare.ReadWrite).CopyTo(this.fileStream);
						break;
					} catch (Exception e) { }
				}

				this.fileModifiedDate = File.GetLastWriteTime(this.filePath);
				this.isFile = true;
			} else {
				foreach (string manifestResourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
					if (manifestResourceName.EndsWith(this.filePath)) {
						this.fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourceName);
						break;
					}
				}
			}
		}

		public void Update() {
			if (this.isFile) {
				DateTime currFileModified = File.GetLastWriteTime(this.filePath);

				if (currFileModified != this.fileModifiedDate) {
					this.LoadStream();
					this.fileModifiedCallback?.Invoke();
				}
			}
		}
	}
}