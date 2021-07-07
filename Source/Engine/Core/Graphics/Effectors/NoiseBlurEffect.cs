using SFML.Graphics;

namespace TangentEngine {
	public class NoiseBlurEffect : GraphicEffector {
		private RenTexSprite noiseBlurSprite;

		private Shader noiseBlurShader;

		public NoiseBlurEffect(int zOrder = -1, int downSize = 1) {
			this.zOrder = zOrder;
			this.downSize = downSize;

			this.noiseBlurSprite = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.noiseBlurSprite.SetViewToCamera();
			this.noiseBlurSprite.ScaleToCamera();

			this.noiseBlurShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.NoiseBlur.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.noiseBlurSprite.Clear(Color.Transparent);
			this.noiseBlurSprite.SetViewToCamera();
			this.noiseBlurSprite.ScaleToCamera();

			this.noiseBlurShader.SetUniform("inputTexture", renTexSprite.renderTexture.Texture);
			//this.noiseBlurShader.SetUniform("time", (float) Engine.time.elapsedRealTime);

			this.noiseBlurSprite.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.noiseBlurShader));
			renTexSprite.Draw(this.noiseBlurSprite, new RenderStates(BlendMode.Alpha));
		}
	}
}