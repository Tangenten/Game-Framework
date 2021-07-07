using System;
using System.Numerics;
using HPCsharp;

namespace TangentEngine {
	public class SoftclipEffect : AudioEffector {
		private Vector<float> min;
		private Vector<float> max;

		public SoftclipEffect() {
			int simdLength = Vector<float>.Count;
			float[] floatArr = new float[simdLength];

			floatArr.FillGenericSse(-1f);
			min = new Vector<float>(floatArr);
			floatArr.FillGenericSse(1f);
			max = new Vector<float>(floatArr);
		}

		public override void Process(ref float[] samples) {
			int simdLength = Vector<float>.Count;

			int i = 0;
			for (i = 0; i <= samples.Length - simdLength; i += simdLength) {
				Vector<float> vectorSection = new Vector<float>(samples, i);
				vectorSection = Vector.Min(Vector.Max(vectorSection, min), max);
				(1.5f * vectorSection - 0.5f * vectorSection * vectorSection * vectorSection).CopyTo(samples, i);
			}

			for (; i < samples.Length; i++) {
				samples[i] = Math.Clamp(samples[i], -1f, 1f);
				samples[i] = 1.5f * samples[i] - 0.5f * samples[i] * samples[i] * samples[i];
			}
		}
	}
}