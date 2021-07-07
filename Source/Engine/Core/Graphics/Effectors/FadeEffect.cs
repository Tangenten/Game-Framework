using SFML.Graphics;

namespace TangentEngine {
	public class FadeEffect : GraphicEffector {
		private RenTexSprite fadeSprite;
		private RenTexSprite fadeSprite2;

		private Shader fadeShader;

		private bool swapper;
		private int iterations;
		private float decayFactor;
		private float randomFactor;

		public FadeEffect(int zOrder = -1, int downSize = 1) : base(zOrder, downSize) {
			this.fadeSprite = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.fadeSprite.SetViewToCamera();
			this.fadeSprite.ScaleToCamera();
			this.fadeSprite.Clear(Color.Transparent);

			this.fadeSprite2 = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.fadeSprite2.SetViewToCamera();
			this.fadeSprite2.ScaleToCamera();
			this.fadeSprite2.Clear(Color.Transparent);

			this.fadeShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Fade.frag"));

			this.iterations = 1;
			this.decayFactor = 0.95f;
			this.randomFactor = 0f;
		}

		public void SetIterations(int iterations) {
			this.iterations = iterations;
		}

		public void SetDecayFactor(float decayFactor) {
			this.decayFactor = decayFactor;
			this.fadeShader.SetUniform("decayFactor", this.decayFactor);
		}

		public void SetRandomFactor(float randomFactor) {
			this.randomFactor = randomFactor;
			this.fadeShader.SetUniform("randomFactor", this.randomFactor);
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.fadeSprite.SetViewToCamera();
			this.fadeSprite.ScaleToCamera();

			this.fadeSprite2.SetViewToCamera();
			this.fadeSprite2.ScaleToCamera();

			for (int i = 0; i < this.iterations; i++) {
				this.fadeShader.SetUniform("time", Engine.time.elapsedRealTime + i);

				if (this.swapper) {
					this.fadeShader.SetUniform("inputTexture", renTexSprite.GetTexture());
					this.fadeSprite.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, null));
					this.fadeSprite.Display();

					this.fadeShader.SetUniform("inputTexture", this.fadeSprite.GetTexture());
					this.fadeSprite2.Draw(this.fadeSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.fadeShader));
					this.fadeSprite2.Display();

					renTexSprite.Draw(this.fadeSprite2, new RenderStates(BlendMode.Alpha));
					renTexSprite.Display();
				} else {
					this.fadeShader.SetUniform("inputTexture", renTexSprite.GetTexture());
					this.fadeSprite2.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, null));
					this.fadeSprite2.Display();

					this.fadeShader.SetUniform("inputTexture", this.fadeSprite2.GetTexture());
					this.fadeSprite.Draw(this.fadeSprite2, new RenderStates(BlendMode.None, Transform.Identity, null, this.fadeShader));
					this.fadeSprite.Display();

					renTexSprite.Draw(this.fadeSprite, new RenderStates(BlendMode.Alpha));
					renTexSprite.Display();
				}

				this.swapper = !this.swapper;
			}
		}
	}
}