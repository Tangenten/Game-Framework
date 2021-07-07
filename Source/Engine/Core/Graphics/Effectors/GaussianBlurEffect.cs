using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace TangentEngine {
	public class GaussianBlurEffect : GraphicEffector {
		private RenTexSprite gaussianBlurSprite1;
		private RenTexSprite gaussianBlurSprite2;

		private Shader gaussianBlurShader;

		private float radius = 16f;

		public GaussianBlurEffect(int zOrder = -1, int downSize = 1) {
			this.zOrder = zOrder;
			this.downSize = downSize;

			this.gaussianBlurSprite1 = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.gaussianBlurSprite1.SetViewToCamera();
			this.gaussianBlurSprite1.ScaleToCamera();

			this.gaussianBlurSprite2 = new RenTexSprite(Engine.graphics.resolutionWidth / this.downSize, Engine.graphics.resolutionHeight / this.downSize, Engine.window.contextSettings);
			this.gaussianBlurSprite2.SetViewToCamera();
			this.gaussianBlurSprite2.ScaleToCamera();

			this.gaussianBlurShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.GaussianBlur.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.gaussianBlurSprite1.Clear(Color.Black);
			this.gaussianBlurSprite1.SetViewToCamera();
			this.gaussianBlurSprite1.ScaleToCamera();

			this.gaussianBlurSprite1.Clear(Color.Black);
			this.gaussianBlurSprite1.SetViewToCamera();
			this.gaussianBlurSprite1.ScaleToCamera();

			this.gaussianBlurShader.SetUniform("outputResolution", gaussianBlurSprite1.GetTextureSize());
			this.gaussianBlurShader.SetUniform("radius", this.radius);
			this.gaussianBlurShader.SetUniform("inputTexture", renTexSprite.renderTexture.Texture);
			this.gaussianBlurShader.SetUniform("direction", new Vec2(1f, 0f));

			this.gaussianBlurSprite1.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.gaussianBlurShader));

			this.gaussianBlurShader.SetUniform("inputTexture", this.gaussianBlurSprite1.renderTexture.Texture);
			this.gaussianBlurShader.SetUniform("direction", new Vec2(0f, 1f));

			this.gaussianBlurSprite2.Draw(this.gaussianBlurSprite1, new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.gaussianBlurShader));

			renTexSprite.Draw(this.gaussianBlurSprite2, new RenderStates(BlendMode.None));
		}
	}
}