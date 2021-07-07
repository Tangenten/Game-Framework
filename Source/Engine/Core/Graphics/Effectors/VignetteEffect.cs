using SFML.Graphics;

namespace TangentEngine {
	public class VignetteEffect : GraphicEffector {
		private RenTexSprite vignetteSprite;
		private Shader vignetteShader;
		public float length;
		public float strength;

		public VignetteEffect(int zOrder = -1, int downSize = 1) : base(zOrder, downSize) {
			this.vignetteSprite = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.vignetteSprite.SetViewToCamera();
			this.vignetteSprite.ScaleToCamera();

			this.vignetteShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.Vignette.frag"));

			this.RenderVignette();
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.vignetteSprite.SetViewToCamera();
			this.vignetteSprite.ScaleToCamera();

			renTexSprite.Draw(this.vignetteSprite, new RenderStates(BlendMode.Alpha));
		}

		public void SetLength(float length) {
			this.length = length;
			this.vignetteShader.SetUniform("length", this.length);
			this.RenderVignette();
		}

		public void SetStrength(float strength) {
			this.strength = strength;
			this.vignetteShader.SetUniform("strength", this.strength);
			this.RenderVignette();
		}

		private void RenderVignette() {
			this.vignetteSprite.Clear(Color.Black);
			this.vignetteSprite.Draw(this.vignetteSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.vignetteShader));
			this.vignetteSprite.Display();
		}
	}
}