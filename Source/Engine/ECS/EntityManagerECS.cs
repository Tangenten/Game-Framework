using System.Collections.Generic;

namespace ECS {
	public class EntityManagerEcs {
		private Queue<int> availableIndices;
		private List<EntityEcs> entities;
		private int idCreator;
		private int lastIndex;

		public EntityManagerEcs() {
			this.entities = new List<EntityEcs>();
			this.lastIndex = 0;
			this.idCreator = 0;
			this.availableIndices = new Queue<int>();
		}

		public EntityEcs Create() {
			lock (this.entities) {
				EntityEcs entity = new EntityEcs();
				entity.id = this.idCreator++;
				entity.alive = true;

				if (this.availableIndices.Count != 0) {
					entity.index = this.availableIndices.Dequeue();
					this.entities[entity.index] = entity;
				} else {
					entity.index = this.lastIndex++;
					this.entities.Add(entity);
				}

				return this.entities[entity.index];
			}
		}

		public void Remove(int entityIndex) {
			lock (this.entities) {
				this.entities[entityIndex].alive = false;
				this.entities[entityIndex].key = 0;
				this.availableIndices.Enqueue(entityIndex);
			}
		}

		public List<EntityEcs> GetAllByKey(in long entityKey) {
			List<EntityEcs> entities = new List<EntityEcs>();
			for (int i = 0; i < this.lastIndex; i++) {
				if (this.entities[i].alive && this.entities[i].HasComponent(entityKey)) entities.Add(this.entities[i]);
			}

			return entities;
		}

		public EntityEcs Get(in int entityIndex) {
			return this.entities[entityIndex];
		}

		public bool IsAlive(in int entityIndex, in int id) {
			return this.entities[entityIndex].id == id && this.entities[entityIndex].alive;
		}

		public long GetKey(in int entityIndex) {
			return this.entities[entityIndex].key;
		}
	}
}