using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace TangentEngine {
	public class DitherEffect : GraphicEffector {
		private Shader ditherShader;
		private Texture dither;
		public float ditherScale;

		public DitherEffect(int zOrder = -1, int downSize = 1) {
			this.zOrder = zOrder;
			this.downSize = downSize;

			this.ditherShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Dither.frag"));

			this.SetDitherTexture(new Texture(Engine.assets.GetStream("BayerDither8x8.png")));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			renTexSprite.Draw(renTexSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.ditherShader));
		}

		public void SetDitherTexture(Texture texture) {
			this.dither = texture;
			this.dither.Repeated = true;

			this.ditherShader.SetUniform("ditherTexture", this.dither);
			this.ditherShader.SetUniform("ditherTextureResolution", new Vec2(texture.Size.X, texture.Size.Y));
		}

		public void SetScaling(float scale) {
			this.ditherScale = scale;
			this.ditherShader.SetUniform("ditherTextureScaling", this.ditherScale);
		}
	}
}