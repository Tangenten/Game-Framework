using System;

namespace TangentEngine {
	public class LowpassEffect : AudioEffector {
		public float frequency;

		private float y0;
		private float y1;
		private float C;

		public LowpassEffect(float freq = 2000f) {
			this.SetFrequency(freq);
		}

		public void SetFrequency(float freq) {
			this.frequency = freq;
			this.BuildC();
		}

		private void BuildC() {
			this.C = MathF.Exp(-2.0f * MathF.PI * (this.frequency / Engine.audio.sampleRate));
		}

		public override void Process(ref float[] samples) {
			for (int i = 0; i < samples.Length; i += 2) {
				this.y0 = samples[i + 0] + this.C * (this.y0 - samples[i + 0]);
				this.y1 = samples[i + 1] + this.C * (this.y1 - samples[i + 1]);

				samples[i + 0] = this.y0;
				samples[i + 1] = this.y1;
			}
		}
	}
}