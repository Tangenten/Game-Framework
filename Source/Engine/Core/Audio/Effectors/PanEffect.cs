using System.Numerics;

namespace TangentEngine {
	public class PanEffect : AudioEffector {
		public (float, float) pan;
		public Vector<float> panSection;

		public PanEffect(float leftPan = 1f, float rightPan = 1f) {
			this.SetPan((leftPan, rightPan));
		}

		public void SetPan((float, float) pan) {
			this.pan = pan;

			int simdLength = Vector<float>.Count;
			float[] panArr = new float[simdLength];
			for (int i = 0; i < panArr.Length; i += 2) {
				panArr[i + 0] = this.pan.Item1;
				panArr[i + 1] = this.pan.Item2;
			}

			this.panSection = new Vector<float>(panArr);
		}

		public override void Process(ref float[] samples) {
			int simdLength = Vector<float>.Count;

			int i = 0;
			for (i = 0; i <= samples.Length - simdLength; i += simdLength) {
				Vector<float> vectorSection = new Vector<float>(samples, i);
				(vectorSection * panSection).CopyTo(samples, i);
			}

			for (; i < samples.Length; i += 2) {
				samples[i + 0] *= this.pan.Item1;
				samples[i + 1] *= this.pan.Item2;
			}
		}
	}
}