using SFML.Graphics;

namespace TangentEngine {
	public class BrightnessThresholdEffect : GraphicEffector {
		private Shader thresholdShader;

		public float threshold;
		public float strength;

		public BrightnessThresholdEffect(int zOrder = -1, int downSize = 1) : base(zOrder, downSize) {
			this.thresholdShader = new Shader(
				null,
				null,
				Engine.assets.GetStream("Engine.Source.Engine.Core.Graphics.Shaders.BrightnessThreshold.frag"));

			SetThreshold(1f);
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			renTexSprite.Draw(renTexSprite, new RenderStates(BlendMode.None, Transform.Identity, null, this.thresholdShader));
		}

		public void SetThreshold(float length) {
			this.threshold = length;
			this.thresholdShader.SetUniform("threshold", this.threshold);
		}

		public void SetStrength(float strength) {
			this.strength = strength;
			this.thresholdShader.SetUniform("strength", this.strength);
		}
	}
}