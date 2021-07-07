using ECS;
using TangentEngine;

namespace Game {
	public class GameEcs : GameE {
		public static SceneManagerEcs game;

		public override void Start() {
			game = new SceneManagerEcs();
			game.Start();
		}

		public override void Update() {
			game.Update();
		}

		public override void Stop() {
			game.Stop();
		}
	}
}