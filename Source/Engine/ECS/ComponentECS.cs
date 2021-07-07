using Game;

namespace ECS {
	using static GameEcs;

	public abstract class ComponentEcs {
		public long Key() {
			return game.GetComponentKey(this.GetType());
		}

		public static long Key<T>() {
			return game.GetComponentKey(typeof(T));
		}
	}
}