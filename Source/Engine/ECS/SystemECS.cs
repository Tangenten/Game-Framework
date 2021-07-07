using System.Collections.Generic;
using TangentEngine;

namespace ECS {
	public abstract class SystemEcs {
		internal List<int> cachedEntities;
		internal long key;
		internal bool running;
		private float secondCounter;
		internal List<int> toAdd;
		internal List<int> toRemove;

		public SystemEcs() {
			this.key = 0;
			this.cachedEntities = new List<int>();
			this.toRemove = new List<int>();
			this.toAdd = new List<int>();
			this.running = false;
		}

		public void Run() {
			this.running = true;
			if ((this.secondCounter += Engine.time.deltaRealTime) > 10f) {
				this.SortEntities();
				this.secondCounter = 0;
			}

			this.Update();
			this.running = false;

			this.AddEntities();
			this.RemoveEntities();
		}

		public void SortEntities() {
			this.cachedEntities.Sort();
		}

		public virtual void Update() { }

		internal void RemoveEntities() {
			for (int i = 0; i < this.toRemove.Count; i++) {
				this.cachedEntities.Remove(this.toRemove[i]);
			}

			this.toRemove.Clear();
		}

		internal void AddEntities() {
			for (int i = 0; i < this.toAdd.Count; i++) {
				this.cachedEntities.Add(this.toAdd[i]);
			}

			this.toAdd.Clear();
		}

		private void CacheIntoList(in long entityKey, in long cacheKey, in int entityIndex, ref List<int> list) {
			lock (this.toRemove) {
				if (this.cachedEntities.Contains(entityIndex) && !this.toRemove.Contains(entityIndex)) {
					if ((entityKey & cacheKey) == cacheKey) { } else {
						if (this.running)
							this.toRemove.Add(entityIndex);
						else
							list.Remove(entityIndex);
					}
				} else {
					if ((entityKey & cacheKey) == cacheKey) {
						if (this.running)
							this.toAdd.Add(entityIndex);
						else
							list.Add(entityIndex);
					}
				}
			}
		}

		public void CacheEntity(in long entityKey, in int entityIndex) {
			this.CacheIntoList(entityKey, this.key, entityIndex, ref this.cachedEntities);
		}
	}
}