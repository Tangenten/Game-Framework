using Brekaout;
using GPUParticles;
using Helpers;
using SFML.System;
using Snake;
using TangentEngine;

namespace Program {
	internal class Program {
		private static void Main(string[] args) {
			SystemH.LogExceptionsIntoFile();

			if (SystemH.TryCreateUniqueMutex("qwpeojdfvlknbjnerbfksjdfnlvbnirdnsdfl")) {
				Engine.Init(new Vector2i(0, 0), new Vector2i(1920, 1080));
				Engine.SetGame(new SnakeGame());
				Engine.Run();
			}
		}
	}
}
