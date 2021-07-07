using System;
using SFML.System;

namespace TangentEngine {
	public class TimeE {
		private float averageFps;
		private int avgFpsFrameCounter;
		private float avgFpsTimeCounter;
		private float maxFps;
		private float minFps;
		private bool printFps;

		public float deltaGameTime;
		public float deltaGameTimeScalar;
		public float elapsedGameTime;

		public float deltaRealTime;
		public float elapsedRealTime;
		public int elapsedFrames;

		private Clock clock;

		public TimeE() {
			this.clock = new Clock();
			this.deltaGameTime = 0;
			this.elapsedGameTime = 0;
			this.elapsedFrames = 0;
			this.deltaGameTimeScalar = 1f;

			this.printFps = true;
			this.minFps = int.MaxValue;
			this.maxFps = 0;
		}

		public void Update() {
			this.deltaRealTime = this.clock.Restart().AsSeconds();
			this.elapsedRealTime += this.deltaRealTime;

			this.deltaGameTime = Math.Clamp(this.deltaRealTime, 0.000001f, 0.1f) * this.deltaGameTimeScalar;
			this.elapsedGameTime += this.deltaGameTime;
			this.elapsedFrames += 1;

			this.PrintFrameRate();
		}

		[ConsoleCommand("GET_FPS", "Average Fps : ")]
		public float FrameRate() {
			return this.averageFps;
		}

		[ConsoleCommand("SET_TIME_SCALE")]
		public void SetTimeScalar(float scalar) {
			this.deltaGameTimeScalar = scalar;
		}

		[ConsoleCommand("PRINT_FPS")]
		public void SetPrintFps(bool value) {
			this.printFps = value;
		}

		private void PrintFrameRate() {
			this.avgFpsTimeCounter += this.deltaRealTime;
			this.avgFpsFrameCounter++;

			float fps = 1f / this.deltaRealTime;

			if (fps < this.minFps) {
				this.minFps = fps;
			}

			if (fps > this.maxFps) {
				this.maxFps = fps;
			}

			if (this.avgFpsTimeCounter > 1f) {
				this.averageFps = this.avgFpsFrameCounter / this.avgFpsTimeCounter;
				this.avgFpsTimeCounter = 0f;
				this.avgFpsFrameCounter = 0;
				if (this.printFps)
					Console.WriteLine($"- AVG FPS: {this.FrameRate():#.00} - MIN FPS: {this.minFps:#.00} - MAX FPS: {this.maxFps:#.00} -");

				this.minFps = int.MaxValue;
				this.maxFps = 0;
			}
		}

		public void Reset() {
			this.elapsedFrames = 0;
			this.elapsedRealTime = 0;
			this.deltaGameTimeScalar = 1f;
		}
	}
}