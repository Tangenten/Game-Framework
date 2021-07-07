using SFML.Graphics;

namespace TangentEngine {
	public class GrainEffect : GraphicEffector {
		private Shader grainShader;

		public GrainEffect(int zOrder = -1, int downSize = 1) {
			this.zOrder = zOrder;
			this.downSize = downSize;

			this.grainShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.Grain.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.grainShader.SetUniform("time", Engine.time.elapsedRealTime);

			renTexSprite.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.grainShader));
		}
	}
}