using Game;

namespace ECS {
	using static GameEcs;

	public class EntityEcs {
		public bool alive;
		public int id;
		public int index;
		public long key;

		public EntityEcs() {
			this.key = 0;
			this.index = 0;
			this.id = 0;
			this.alive = false;
		}

		public void AddComponent(in ComponentEcs component) {
			game.AddComponent(this.index, component);
			this.key |= component.Key();
		}

		public void AddComponent(in long componentKey) {
			this.key |= componentKey;
		}

		public void RemoveComponent(in long componentKey) {
			this.key ^= componentKey;
		}

		public bool HasComponent(in long componentKey) {
			return (componentKey | this.key) == this.key;
		}

		public ComponentEcs GetComponent(in long componentKey) {
			return game.GetComponent(this.index, componentKey);
		}

		public void CacheEntity() {
			game.CacheEntity(this.key, this.index);
		}

		public void RemoveEntity() {
			game.RemoveEntity(this.index);
			this.CacheEntity();
		}
	}
}