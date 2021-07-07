using System.Collections.Generic;

namespace EC {
	public class EntityManagerEc {
		private List<EntityEc> entities;

		public EntityManagerEc() {
			this.entities = new List<EntityEc>();
		}

		public EntityEc CreateEntity() {
			lock (this.entities) {
				this.entities.Add(new EntityEc());
				return this.entities[^1];
			}
		}

		public void RemoveEntity(in EntityEc entity) {
			entity.RemoveAllComponents();
			lock (this.entities) {
				this.entities.Remove(entity);
			}
		}

		public List<EntityEc> GetEntities(in long componentKey) {
			List<EntityEc> entities = new List<EntityEc>();
			for (int i = 0; i < this.entities.Count; i++) {
				if (this.entities[i].HasComponent(componentKey)) {
					entities.Add(this.entities[i]);
				}
			}

			return entities;
		}
	}
}