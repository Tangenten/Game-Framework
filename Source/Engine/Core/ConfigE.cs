using System;
using System.Collections.Generic;
using System.IO;
using Helpers;

namespace TangentEngine {
	public class ConfigE {
		private string configFilePath;
		private Dictionary<string, string> configDict;

		public ConfigE() {
			this.configFilePath = $"{Environment.CurrentDirectory}\\config.txt";
			FileH.CreateFileIfNotExists(this.configFilePath);
			this.configDict = new Dictionary<string, string>();
			this.ReadFileToDict();
		}

		public void Update() { }

		[ConsoleCommand("SET_CONFIG")]
		public void WriteToConfig(in string key, in string value) {
			lock (this.configDict) {
				this.configDict[key] = value;
				this.WriteDictToFile();
			}
		}

		public bool ConfigKeyExists(in string key) {
			return this.configDict.ContainsKey(key);
		}

		[ConsoleCommand("GET_CONFIG", "Config Value : ")]
		public string ReadConfigValue(in string key) {
			return this.configDict[key];
		}

		public T ReadConfigValue<T>(in string key) {
			return (T) Convert.ChangeType(this.configDict[key], typeof(T));
		}

		[ConsoleCommand("CLEAR_CONFIG")]
		private void ClearConfigFile() {
			this.configDict.Clear();
			this.WriteDictToFile();
		}

		private void WriteDictToFile() {
			using (StreamWriter streamWriter = File.CreateText(this.configFilePath)) {
				foreach (KeyValuePair<string, string> keyValuePair in this.configDict) {
					streamWriter.WriteLine("{0} {1}", keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		private void ReadFileToDict() {
			using (StreamReader streamReader = new StreamReader(this.configFilePath)) {
				this.configDict.Clear();
				string? line = streamReader.ReadLine();
				while (line != null) {
					string[] splitLine = line.Split(" ");
					this.configDict[splitLine[0]] = splitLine[1];
					line = streamReader.ReadLine();
				}
			}
		}
	}
}