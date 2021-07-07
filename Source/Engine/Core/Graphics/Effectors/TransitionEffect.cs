using SFML.Graphics;

namespace TangentEngine {
	public class TransitionEffect : GraphicEffector {
		private Shader transitionShader;
		public Texture transition;
		public float threshold;

		public TransitionEffect(int zOrder = -1, int downSize = 1) : base(zOrder, downSize) {
			this.transitionShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Game.Experiments.GraphicsMixer.Assets.GrayScaleTransition.frag"));
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			renTexSprite.Draw(renTexSprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.transitionShader));
		}

		public void SetThreshold(float threshold) {
			this.threshold = threshold;
			this.transitionShader.SetUniform("transitionThreshold", this.threshold);
		}

		public void SetTexture(Texture texture) {
			this.transition = texture;
			this.transition.Smooth = true;
			this.transitionShader.SetUniform("transitionTexture", this.transition);
		}
	}
}