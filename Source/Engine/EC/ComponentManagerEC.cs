using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EC {
	public class ComponentManagerEc {
		private Dictionary<long, List<ComponentEc>> componentsDictionary;
		private long[] iterationOrder;

		private Dictionary<Type, long> keyDictionary;
		private long keyGen;

		private bool running;
		private List<ComponentEc> toRemove;

		public ComponentManagerEc() {
			this.componentsDictionary = new Dictionary<long, List<ComponentEc>>();
			this.toRemove = new List<ComponentEc>();
			this.iterationOrder = new long[] { };
			this.keyDictionary = new Dictionary<Type, long>();
			this.keyGen = 1;
			this.running = false;
		}

		public long GenerateComponentKey() {
			long key = this.keyGen;
			this.keyGen = this.keyGen + this.keyGen;
			return key;
		}

		public long GetComponentKey(Type type) {
			if (!this.keyDictionary.ContainsKey(type)) {
				this.keyDictionary[type] = this.GenerateComponentKey();
			}

			return this.keyDictionary[type];
		}

		public void Update() {
			for (int i = 0; i < this.iterationOrder.Length; i++) {
				this.running = true;

				List<ComponentEc> components = this.componentsDictionary[this.iterationOrder[i]];
				if (components.Count > 0) {
					components[0].PreCompute();
				}

				Parallel.For(0, components.Count, j => { components[j].Update(); });
				if (components.Count > 0) {
					components[0].PostCompute();
				}

				this.running = false;
				this.ToRem();
			}
		}

		public void SetIterationOrder(long[] order) {
			this.iterationOrder = order;
		}

		public void ToRem() {
			for (int i = 0; i < this.toRemove.Count; i++) {
				this.RemoveComponent(this.toRemove[i]);
			}

			this.toRemove.Clear();
		}

		public ComponentEc AddComponent(ComponentEc component) {
			lock (this.toRemove) {
				long componentKey = component.Key();
				if (!this.componentsDictionary.ContainsKey(componentKey)) {
					this.componentsDictionary[componentKey] = new List<ComponentEc>();
					this.componentsDictionary[componentKey].Add(component);
					component.StartCompute();
				} else {
					this.componentsDictionary[componentKey].Add(component);
					component.Key();
				}

				component.Start();
				return this.componentsDictionary[componentKey][this.componentsDictionary[componentKey].Count - 1];
			}
		}

		public void RemoveComponent(ComponentEc component) {
			lock (this.toRemove) {
				if (this.running) {
					this.toRemove.Add(component);
				} else {
					component.parentEntity.ClearComponent(component.GetType());
					this.componentsDictionary[component.Key()].Remove(component);
					component.Stop();
				}
			}
		}
	}
}