using System;

namespace Helpers {
	public static class OscH {
		public static float SinOsc(float minAmp, float maxAmp, float frequency, float phase, float increment) {
			return (MathF.Sin((increment + phase * MathF.PI) * frequency) + 1f) / 2f * (maxAmp - minAmp) + minAmp;
		}

		public static float PulseOsc(float minAmp, float maxAmp, float frequency, float phase, float increment, float pulse = 0.5f) {
			float val = SinOsc(-1f, 1f, frequency, phase, increment);
			if (val < -1f + pulse * 2f) {
				val = -1f;
			} else {
				val = 1f;
			}

			return val * (maxAmp - minAmp) + minAmp;
		}
	}
}