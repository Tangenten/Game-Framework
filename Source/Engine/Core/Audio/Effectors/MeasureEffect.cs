using System;

namespace TangentEngine {
	public class MeasureEffect : AudioEffector {
		public bool measurePeak;
		public float peak;
		public float peakDropoff;

		public bool measureRms;
		public float rms;
		private RingBuffer<float> rmsBuffer;

		public MeasureEffect() {
			this.peakDropoff = 1f;
			this.rmsBuffer = new RingBuffer<float>(Engine.audio.sampleRate / 16);
			this.measurePeak = true;
			this.measureRms = false;
		}

		public override void Process(ref float[] samples) {
			if (this.measurePeak) {
				float peakStart = 0f;

				for (int i = 0; i < samples.Length; i++) {
					float sampleAbs = MathF.Abs(samples[i]);
					if (sampleAbs > peakStart) {
						peakStart = sampleAbs;
					}
				}

				this.peak = Math.Clamp(this.peak - this.peakDropoff, 0f, 1f);
				if (peakStart > this.peak) {
					this.peak = peakStart;
				}
			}

			if (this.measureRms) {
				this.rmsBuffer.PushData(samples);
				Span<float> rmsData = this.rmsBuffer.GetAllData();
				float rmsSum = 0f;
				for (int i = 0; i < rmsData.Length; i++) {
					rmsSum += MathF.Abs(rmsData[i]);
				}

				this.rms = rmsSum / rmsData.Length;
			}
		}
	}
}