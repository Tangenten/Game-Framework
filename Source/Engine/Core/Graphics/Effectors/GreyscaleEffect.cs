using SFML.Graphics;

namespace TangentEngine {
	public class GreyscaleEffect : GraphicEffector {
		private Shader greyscaleShader;

		public GreyscaleEffect(int zOrder = -1, int downSize = 1) {
			this.zOrder = zOrder;
			this.downSize = downSize;

			this.greyscaleShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.Greyscale.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.greyscaleShader.SetUniform("inputTexture", renTexSprite.renderTexture.Texture);

			renTexSprite.Draw(renTexSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.greyscaleShader));
		}
	}
}