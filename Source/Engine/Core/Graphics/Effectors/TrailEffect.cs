using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace TangentEngine {
	public class TrailEffect : GraphicEffector {
		private RenTexSprite trailSprite;
		private RenTexSprite trailSprite2;

		private Shader trailShader;
		private bool swapper;

		public TrailEffect(int zOrder = -1, int downSize = 1) : base(zOrder, downSize) {
			this.trailSprite = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.trailSprite.SetViewToCamera();
			this.trailSprite.ScaleToCamera();
			this.trailSprite.Clear(Color.Transparent);

			this.trailSprite2 = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.trailSprite2.SetViewToCamera();
			this.trailSprite2.ScaleToCamera();
			this.trailSprite2.Clear(Color.Transparent);

			this.trailShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Fade.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.trailSprite.SetViewToCamera();
			this.trailSprite.ScaleToCamera();

			this.trailSprite2.SetViewToCamera();
			this.trailSprite2.ScaleToCamera();

			this.trailShader.SetUniform("outputResolution", new Vec2(renTexSprite.GetTextureSize().X, renTexSprite.GetTextureSize().Y));

			for (int i = 0; i < 1; i++) {
				this.trailShader.SetUniform("time", Engine.time.elapsedRealTime + i);

				if (this.swapper) {
					this.trailShader.SetUniform("inputTexture", renTexSprite.GetTexture());
					this.trailSprite.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, null));
					this.trailSprite.Display();

					this.trailShader.SetUniform("inputTexture", trailSprite.GetTexture());
					this.trailSprite2.Draw(this.trailSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.trailShader));
					this.trailSprite2.Display();

					renTexSprite.Draw(this.trailSprite2, new RenderStates(BlendMode.None));
					renTexSprite.Display();
				} else {
					this.trailShader.SetUniform("inputTexture", renTexSprite.GetTexture());
					this.trailSprite2.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, null));
					this.trailSprite2.Display();

					this.trailShader.SetUniform("inputTexture", this.trailSprite2.GetTexture());
					this.trailSprite.Draw(this.trailSprite2, new RenderStates(BlendMode.None, Transform.Identity, null, this.trailShader));
					this.trailSprite.Display();

					renTexSprite.Draw(this.trailSprite, new RenderStates(BlendMode.None));
					renTexSprite.Display();
				}

				this.swapper = !this.swapper;
			}
		}
	}
}