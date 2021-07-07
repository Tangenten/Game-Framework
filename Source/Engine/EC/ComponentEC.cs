using Game;

namespace EC {
	using static GameEc;

	public abstract class ComponentEc {
		internal bool alive;
		internal EntityEc parentEntity;

		public ComponentEc() {
			this.alive = true;
		}

		public virtual void Start() { }

		public virtual void StartCompute() { }

		public virtual void PreCompute() { }

		public virtual void Update() { }

		public virtual void PostCompute() { }

		public virtual void Stop() {
			this.alive = false;
		}

		public long Key() {
			return game.GetComponentKey(this.GetType());
		}

		public static long Key<T>() {
			return game.GetComponentKey(typeof(T));
		}
	}
}