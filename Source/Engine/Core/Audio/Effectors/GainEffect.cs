using System.Numerics;

namespace TangentEngine {
	public class GainEffect : AudioEffector {
		public float gain;

		public GainEffect(float gain = 1f) {
			this.gain = gain;
		}

		public override void Process(ref float[] samples) {
			int simdLength = Vector<float>.Count;

			int i = 0;
			for (i = 0; i <= samples.Length - simdLength; i += simdLength) {
				Vector<float> vectorSection = new Vector<float>(samples, i);
				(vectorSection * this.gain).CopyTo(samples, i);
			}

			for (; i < samples.Length; i++) {
				samples[i] *= this.gain;
			}
		}
	}
}