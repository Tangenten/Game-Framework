using System;
using System.Collections.Generic;

namespace ECS {
	public abstract class SceneEcs {
		private ComponentManagerEcs componentManager;
		private EntityManagerEcs entityManager;
		private SystemManagerEcs systemManager;

		public SceneEcs() {
			this.entityManager = new EntityManagerEcs();
			this.componentManager = new ComponentManagerEcs();
			this.systemManager = new SystemManagerEcs();
		}

		public virtual void Update() {
			this.systemManager.Update();
		}

		public abstract void Start();
		public abstract void Stop();

		public EntityEcs CreateEntity() {
			return this.entityManager.Create();
		}

		public void RemoveEntity(in int entityIndex) {
			this.entityManager.Remove(entityIndex);
		}

		public EntityEcs GetEntity(in int entityIndex) {
			return this.entityManager.Get(entityIndex);
		}

		public List<EntityEcs> GetEntities(in long entityKey) {
			return this.entityManager.GetAllByKey(entityKey);
		}

		public void AddComponent(in int entityIndex, in ComponentEcs component) {
			this.componentManager.AddComponent(entityIndex, component);
		}

		public ComponentEcs GetComponent(in int entityIndex, in long componentKey) {
			return this.componentManager.GetComponent(entityIndex, componentKey);
		}

		public ref List<ComponentEcs> GetComponentList(in long componentKey) {
			return ref this.componentManager.GetComponentList(componentKey);
		}

		public void AddSystem(in SystemEcs system) {
			this.systemManager.Add(system);
		}

		public void RemoveSystem(in SystemEcs system) {
			this.systemManager.Remove(system);
		}

		public void CacheEntity(in long entityKey, in int entityIndex) {
			this.systemManager.CacheEntity(entityKey, entityIndex);
		}

		public long GetComponentKey(Type type) {
			return this.componentManager.GetComponentKey(type);
		}
	}
}