using System;
using System.Collections.Generic;
using Helpers;
using TangentEngine;

namespace ECS {
	public class SceneManagerEcs {
		private SceneEcs scene;

		public SceneManagerEcs() {
			this.scene = null;
		}

		public void Start() {
			//this.SetScene();
		}

		public void Stop() {
			this.scene.Stop();
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

		public EntityEcs CreateEntity() {
			return this.scene.CreateEntity();
		}

		public void RemoveEntity(in int entityIndex) {
			this.scene.RemoveEntity(entityIndex);
		}

		public List<EntityEcs> GetEntities(in long entityKey) {
			return this.scene.GetEntities(entityKey);
		}

		public void AddComponent(in int entityIndex, in ComponentEcs component) {
			this.scene.AddComponent(entityIndex, component);
		}

		public ComponentEcs GetComponent(in int entityIndex, in long componentKey) {
			return this.scene.GetComponent(entityIndex, componentKey);
		}

		public ref List<ComponentEcs> GetComponentList(in long key) {
			return ref this.scene.GetComponentList(key);
		}

		public void AddSystem(in SystemEcs system) {
			this.scene.AddSystem(system);
		}

		public void RemoveSystem(in SystemEcs system) {
			this.scene.RemoveSystem(system);
		}

		public EntityEcs GetEntity(in int entityIndex) {
			return this.scene.GetEntity(entityIndex);
		}

		public void CacheEntity(in long entityKey, in int entityIndex) {
			this.scene.CacheEntity(entityKey, entityIndex);
		}

		public void SetScene(SceneEcs scene) {
			scene?.Stop();

			this.scene = scene;
			this.scene.Start();
		}

		public void SetScene(string sceneClassName) {
			try {
				this.SetScene(ReflectionH.GetObjectByClassName<SceneEcs>(sceneClassName));
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		public long GetComponentKey(Type type) {
			return this.scene.GetComponentKey(type);
		}

		public long GetComponentKey<T>() where T : ComponentEcs {
			return this.scene.GetComponentKey(typeof(T));
		}
	}
}