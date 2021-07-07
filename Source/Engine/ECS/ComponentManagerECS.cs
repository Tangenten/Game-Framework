using System;
using System.Collections.Generic;
using Helpers;

namespace ECS {
	public class ComponentManagerEcs {
		private Dictionary<long, ComponentArray> componentsDictionary;
		private Dictionary<Type, long> keyDictionary;
		private long keyGen;

		public ComponentManagerEcs() {
			this.componentsDictionary = new Dictionary<long, ComponentArray>();
			this.keyDictionary = new Dictionary<Type, long>();
			this.keyGen = 1;
		}

		public long GenerateComponentKey() {
			long key = this.keyGen;
			this.keyGen = this.keyGen + this.keyGen;
			return key;
		}

		public long GetComponentKey(Type type) {
			if (!this.keyDictionary.ContainsKey(type)) this.keyDictionary[type] = this.GenerateComponentKey();
			return this.keyDictionary[type];
		}

		public void AddComponent(in int entityIndex, in ComponentEcs component) {
			if (!this.componentsDictionary.ContainsKey(component.Key())) this.componentsDictionary[component.Key()] = new ComponentArray();

			this.componentsDictionary[component.Key()].AddComponent(entityIndex, component);
		}

		public ComponentEcs GetComponent(in int entityIndex, in long componentKey) {
			return this.componentsDictionary[componentKey].GetComponent(entityIndex);
		}

		public ref List<ComponentEcs> GetComponentList(in long componentKey) {
			return ref this.componentsDictionary[componentKey].GetComponentList();
		}
	}

	public class ComponentArray {
		private List<ComponentEcs> components;

		public ComponentArray() {
			this.components = new List<ComponentEcs>();
			this.components.Resize(64);
		}

		public void AddComponent(in int entityIndex, in ComponentEcs component) {
			lock (this.components) {
				if (entityIndex >= this.components.Count) {
					this.components.Resize(this.components.Count * 2);
				}

				this.components[entityIndex] = component;
			}
		}

		public ComponentEcs GetComponent(in int entityIndex) {
			return this.components[entityIndex];
		}

		public ref List<ComponentEcs> GetComponentList() {
			return ref this.components;
		}
	}
}