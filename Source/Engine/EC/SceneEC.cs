using System;
using System.Collections.Generic;

namespace EC {
	public abstract class SceneEc {
		private ComponentManagerEc componentManager;
		private EntityManagerEc entityManager;

		public SceneEc() {
			this.entityManager = new EntityManagerEc();
			this.componentManager = new ComponentManagerEc();
		}

		public virtual void Update() {
			this.componentManager.Update();
		}

		public virtual void Start() { }

		public virtual void Stop() { }

		public EntityEc CreateEntity() {
			return this.entityManager.CreateEntity();
		}

		public ComponentEc AddComponent(in ComponentEc component) {
			return this.componentManager.AddComponent(component);
		}

		public void RemoveComponent(in ComponentEc component) {
			this.componentManager.RemoveComponent(component);
		}

		public void SetUpdateOrder(long[] types) {
			this.componentManager.SetIterationOrder(types);
		}

		public long GetComponentKey(Type type) {
			return this.componentManager.GetComponentKey(type);
		}

		public List<EntityEc> GetEntities(in long componentKey) {
			return this.entityManager.GetEntities(componentKey);
		}

		public void RemoveEntity(EntityEc entity) {
			this.entityManager.RemoveEntity(entity);
		}
	}
}