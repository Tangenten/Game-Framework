using EC;
using TangentEngine;

namespace Game {
	public class GameEc : GameE {
		public static SceneManagerEc game;

		public GameEc(in SceneEc scene) {
			game = new SceneManagerEc();
			game.SetScene(scene);
		}

		public override void Start() {
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