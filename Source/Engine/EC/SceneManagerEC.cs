using System;
using System.Collections.Generic;
using Helpers;
using TangentEngine;

namespace EC {
	public class SceneManagerEc {
		private SceneEc scene;

		public SceneManagerEc() {
			this.scene = null;
		}

		public void Start() { }

		public void Stop() {
			this.scene.Start();
		}

		public void Update() {
			this.ConsoleCommand(Engine.console.currentConsoleCommand);
			this.scene?.Update();
		}

		public void ConsoleCommand(in string[] input) {
			string opCode = input[0];

			switch (input.Length) {
				case 1: {
					switch (opCode.ToUpper()) {
						case "INSPECT_SCENE": {
							Engine.inspector.BuildInspectionGui(this.scene);
							break;
						}
					}

					break;
				}
				case 2: {
					string value = input[1];
					switch (opCode.ToUpper()) {
						case "SET_SCENE": {
							this.SetScene(value.ToUpper());
							break;
						}
					}

					break;
				}
			}
		}

		public void SetScene(SceneEc scene) {
			scene?.Stop();

			this.scene = scene;
			this.scene.Start();
		}

		public void SetScene(string sceneClassName) {
			try {
				this.SetScene(ReflectionH.GetObjectByClassName<SceneEc>(sceneClassName));
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		public EntityEc CreateEntity() {
			return this.scene.CreateEntity();
		}

		public ComponentEc AddComponent(in ComponentEc component) {
			return this.scene.AddComponent(component);
		}

		public void RemoveComponent(in ComponentEc component) {
			this.scene.RemoveComponent(component);
		}

		public long GetComponentKey(Type type) {
			return this.scene.GetComponentKey(type);
		}

		public long GetComponentKey<T>() where T : ComponentEc {
			return this.scene.GetComponentKey(typeof(T));
		}

		public List<EntityEc> GetEntities(long componentKey) {
			return this.scene.GetEntities(componentKey);
		}

		public void RemoveEntity(EntityEc entity) {
			this.scene.RemoveEntity(entity);
		}
	}
}