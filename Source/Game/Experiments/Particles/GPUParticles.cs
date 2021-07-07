using System;
using System.Threading.Tasks;
using Helpers;
using OpenTK.Graphics.OpenGL4;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using TangentEngine;
using PrimitiveType = SFML.Graphics.PrimitiveType;

namespace GPUParticles {
	public class GpuParticlesGame : GameE {
		private RenderStates finalRenderStates;

		private RenderTexture finalRenderTexture;

		[InspectNumerical(0.00001f, 0.5f, 0.00001)]
		private float posStrength = 0.1f;

		private RenderStates renderStates;

		private RenderTexture renderTexture1;
		private RenderTexture renderTexture2;

		private RenderTexture temp;

		[InspectNumerical(0.0000001f, 0.5f, 0.00001)]
		private float velStrength = 0.01f;

		private VertexBuffer vertices;

		public override void Start() {
			int length1D = 2048;
			int length2D = (int) Math.Pow(length1D, 2);

			Vertex[] verts = new Vertex[length2D];
			float[] initalValues = new float[length2D * 4];
			Parallel.For(0, length1D, i => {
				for (int j = 0; j < length1D; j++) {
					float xPos = TweenH.Linear(i, 0f, length1D, -1f, 1f);
					float yPos = TweenH.Linear(j, 0f, length1D, -1f, 1f);

					float xVel = 0f;
					float yVel = 0f;

					initalValues[(j * length1D + i) * 4 + 0] = xPos;
					initalValues[(j * length1D + i) * 4 + 1] = yPos;
					initalValues[(j * length1D + i) * 4 + 2] = xVel;
					initalValues[(j * length1D + i) * 4 + 3] = yVel;

					verts[i * length1D + j] = new Vertex(new Vector2f(i, j));
				}
			});

			this.vertices = new VertexBuffer((uint) length2D, PrimitiveType.Points, VertexBuffer.UsageSpecifier.Static);
			this.vertices.Update(verts);

			this.renderTexture1 = new RenderTexture((uint) length1D, (uint) length1D);

			Texture.Bind(this.renderTexture1.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba16f,
				(int) this.renderTexture1.Size.X,
				(int) this.renderTexture1.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				initalValues);
			Texture.Bind(null);

			this.renderTexture2 = new RenderTexture((uint) length1D, (uint) length1D);

			Texture.Bind(this.renderTexture2.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba16f,
				(int) this.renderTexture2.Size.X,
				(int) this.renderTexture2.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				IntPtr.Zero);
			Texture.Bind(null);

			this.renderStates = new RenderStates(BlendMode.None);
			this.renderStates.Shader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Game.Experiments.Particles.Assets.UpdateParticles.frag"));
			this.renderStates.Shader.SetUniform("resolution", new Vec2(length1D, length1D));
			this.renderStates.Shader.SetUniform("texture", Shader.CurrentTexture);

			this.finalRenderTexture = new RenderTexture(1920, 1080);
			this.finalRenderTexture.Clear(new Color(0, 0, 0, 0));

			Texture.Bind(this.finalRenderTexture.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba16f,
				(int) this.finalRenderTexture.Size.X,
				(int) this.finalRenderTexture.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				IntPtr.Zero);
			Texture.Bind(null);

			this.finalRenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
			this.finalRenderStates.Shader = new Shader(
				Engine.assets.GetStream("Engine.Source.Game.Experiments.Particles.Assets.RenderParticles.vert"),
				null,
				null);
			this.finalRenderStates.Shader.SetUniform("resolution", new Vec2(length1D, length1D));
			this.finalRenderStates.Shader.SetUniform("texture", this.renderTexture2.Texture);

			Engine.console.EnterConsoleCommand("INSPECT_GAME");
		}

		public override void Update() {
			Vec2 v = new Vec2(new Vector2f(
				TweenH.Linear(Engine.input.GetMouseWindowPosition().X, 0f, 1920f, -1f, 1f),
				TweenH.Linear(Engine.input.GetMouseWindowPosition().Y, 1080f, 0, -1f, 1f)
			));

			this.renderStates.Shader.SetUniform("mouse", v);
			this.renderStates.Shader.SetUniform("velStrength", this.velStrength);
			this.renderStates.Shader.SetUniform("posStrength", this.posStrength);

			this.renderTexture1.Display();
			this.renderTexture2.Draw(new Sprite(this.renderTexture1.Texture), this.renderStates);
			this.renderTexture2.Display();

			Engine.graphics.Draw(this.vertices, this.finalRenderStates);

			this.temp = this.renderTexture1;
			this.renderTexture1 = this.renderTexture2;
			this.renderTexture2 = this.temp;
		}

		public override void Stop() { }
	}
}