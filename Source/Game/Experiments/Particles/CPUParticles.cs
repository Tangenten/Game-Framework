using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Helpers;
using SFML.Graphics;
using SFML.System;
using TangentEngine;

namespace CPUParticles {
	public class CpuParticlesGame : GameE {
		private int length2D;
		private Particle[] particles;

		private RenderTexture renderTexture;

		private Vertex[] vertices;

		public override void Start() {
			this.renderTexture = new RenderTexture(1920, 1080);

			int length1D = 1024;
			this.length2D = (int) Math.Pow(length1D, 2);

			this.particles = new Particle[this.length2D];
			this.vertices = new Vertex[this.length2D];
			for (int i = 0; i < this.length2D; i++) {
				float xPos = RandomH.GetRandom(0f, 1920f);
				float yPos = RandomH.GetRandom(0f, 1080f);
				float xVel = 0;
				float yVel = 0;

				this.particles[i] = new Particle(new Vector2(xPos, yPos), new Vector2(xVel, yVel));
				this.vertices[i].Color = new Color((byte) TweenH.Linear(xPos, 0, 1920, 0, 255), (byte) TweenH.Linear(yPos, 0, 1920, 0, 255), 0);
			}
		}

		public override void Update() {
			this.renderTexture.Clear();

			Vector2 middle = new Vector2(Engine.input.GetMouseWindowPosition().X, Engine.input.GetMouseWindowPosition().Y);

			Parallel.For(0, this.length2D, i => {
				ref Vector2 pos = ref this.particles[i].Position;
				ref Vector2 vel = ref this.particles[i].Velocity;

				Vector2 dirToMiddle = middle - pos;
				float length = dirToMiddle.LengthSquared();

				dirToMiddle = Vector2.Normalize(dirToMiddle);

				vel += dirToMiddle * 0.01f * length;
				vel *= 0.99f;
				pos += vel * 0.005f;

				this.vertices[i].Position.X = pos.X;
				this.vertices[i].Position.Y = pos.Y;
			});

			this.renderTexture.Draw(this.vertices, PrimitiveType.Points, new RenderStates(BlendMode.Alpha));
			Engine.graphics.Draw(this.renderTexture, new RenderStates(BlendMode.Alpha), new Vector2f(Engine.camera.gameWidth, Engine.camera.gameHeight));
		}

		public override void Stop() { }

		[StructLayout(LayoutKind.Auto)]
		public struct Particle {
			public Vector2 Position;
			public Vector2 Velocity;

			public Particle(in Vector2 pos, in Vector2 vel) {
				this.Position = pos;
				this.Velocity = vel;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Magnitude(in Vector2 v) {
			return (float) Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}
	}
}