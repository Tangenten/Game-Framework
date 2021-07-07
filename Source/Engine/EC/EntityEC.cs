using System;
using System.Collections.Generic;
using Game;

namespace EC {
	using static GameEc;

	public class EntityEc {
		private bool alive;
		private List<ComponentEc> components;
		private long key;

		public EntityEc() {
			this.components = new List<ComponentEc>();
			this.key = 0;
			this.alive = true;
		}

		public void AddComponent(in ComponentEc component) {
			component.parentEntity = this;
			this.components.Add(game.AddComponent(component));
			this.key |= component.Key();
		}

		public void RemoveComponent(in Type component) {
			for (int i = 0; i < this.components.Count; i++) {
				if (this.components[i].GetType() == component) {
					game.RemoveComponent(this.components[i]);
					break;
				}
			}
		}

		public void ClearComponent(in Type component) {
			for (int i = 0; i < this.components.Count; i++) {
				if (this.components[i].GetType() == component) {
					this.components.RemoveAt(i);
					break;
				}
			}

			this.key ^= game.GetComponentKey(component);
		}

		public bool HasComponent(in Type component) {
			return (game.GetComponentKey(component) | this.key) == this.key;
		}

		public bool HasComponent(in long componentKey) {
			return (componentKey | this.key) == this.key;
		}

		public ComponentEc GetComponent(in Type component) {
			for (int i = 0; i < this.components.Count; i++) {
				if (this.components[i].GetType() == component) {
					return this.components[i];
				}
			}

			return null;
		}

		public void RemoveEntity() {
			this.alive = false;
			game.RemoveEntity(this);
		}

		public void RemoveAllComponents() {
			for (int i = 0; i < this.components.Count; i++) {
				game.RemoveComponent(this.components[i]);
			}
		}
	}
}