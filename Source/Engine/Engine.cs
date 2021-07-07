using SFML.System;

namespace TangentEngine {
	public static class Engine {
		public static AssetsE assets;
		public static AudioE audio;
		public static CameraE camera;
		public static ConfigE config;
		public static ConsoleE console;
		public static CoroutinesE coroutines;
		public static GUIE gui;
		public static GameE game;
		public static GraphicsE graphics;
		public static InputE input;
		public static InspectorE inspector;
		public static TimeE time;
		public static WindowE window;

		public static void Init(Vector2i windowPos, Vector2i windowSize) {
			assets = new AssetsE();
			config = new ConfigE();
			time = new TimeE();
			coroutines = new CoroutinesE();
			window = new WindowE(windowPos, windowSize);
			gui = new GUIE(window.gui);
			camera = new CameraE(window.renderWindow);
			graphics = new GraphicsE(window.renderWindow);
			audio = new AudioE();
			input = new InputE(window.renderWindow);
			console = new ConsoleE();
			inspector = new InspectorE();
		}

		public static void Reset() {
			assets.Reset();
			audio.Reset();
			camera.Reset();
			coroutines.Reset();
			graphics.Reset();
			input.Reset();
			inspector.Reset();
			gui.Reset();
			time.Reset();
		}

		public static void Run() {
			while (window.IsOpen()) {
				Update();
			}
		}

		public static void SetGame(GameE game) {
			if (Engine.game != null) {
				Engine.game.Stop();
				Reset();
			}

			Engine.game = game;
			Engine.game.Start();
		}

		public static void Update() {
			input.Update();
			time.Update();
			graphics.Clear();
			window.Update();
			console.Update();
			gui.Update();
			camera.Update();
			assets.Update();
			inspector.Update();
			config.Update();
			game.Update();
			audio.Update();
			coroutines.ResumeAsyncUpdate();
			console.Render();
			graphics.Update();
			gui.Display();
			graphics.Display();
			coroutines.PauseAsyncUpdate();
		}
	}
}