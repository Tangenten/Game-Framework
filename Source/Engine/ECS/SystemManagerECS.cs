using System.Collections.Generic;
using TangentEngine;

namespace ECS {
	public class SystemManagerEcs {
		private List<SystemEcs> systems;

		public SystemManagerEcs() {
			this.systems = new List<SystemEcs>();
		}

		public void Add(in SystemEcs system) {
			this.systems.Add(system);
		}

		public void Remove(in SystemEcs system) {
			this.systems.Remove(system);
		}

		public void CacheEntity(in long entityKey, in int entityIndex) {
			for (int i = 0; i < this.systems.Count; i++) {
				this.systems[i].CacheEntity(entityKey, entityIndex);
			}
		}

		public void Update() {
			this.ConsoleCommand(Engine.console.currentConsoleCommand);

			for (int i = 0; i < this.systems.Count; i++) {
				this.systems[i].Run();
			}
		}

		public void ConsoleCommand(in string[] input) {
			string opCode = input[0];

			switch (input.Length) {
				case 1: {
					break;
				}
				case 2: {
					string value = input[1];
					switch (opCode.ToUpper()) {
						case "INSPECT_SYSTEM": {
							this.Inspection(value.ToUpper());
							break;
						}
					}

					break;
				}
			}
		}

		public void Inspection(string stringInput) {
			foreach (SystemEcs system in this.systems) {
				if (system.GetType().Name.ToUpper() == stringInput.ToUpper()) {
					Engine.inspector.BuildInspectionGui(system);
					break;
				}
			}
		}
	}
}