using System;
using System.Collections.Generic;
using SFML.Audio;

namespace TangentEngine {
	public class SoundPlayer : AudioProvider {
		private float[] samples;
		private List<SoundBank> soundBanks;

		public PlayState playState;

		private float volume;
		private float pitch;

		public SoundPlayer(SoundBuffer soundBuffer) {
			this.soundBanks = new List<SoundBank>();
			this.pitch = 1f;
			this.volume = 1f;

			// Convert to Stereo if mono
			if (soundBuffer.ChannelCount == 1 && Engine.audio.channels == 2) {
				int sampleLength = soundBuffer.Samples.Length * 2;
				this.samples = new float[sampleLength];

				short[] soundBufferSamples = soundBuffer.Samples;
				for (int i = 0; i < sampleLength / 2; i++) {
					this.samples[i * 2 + 0] = soundBufferSamples[i] / (float) short.MaxValue;
					this.samples[i * 2 + 1] = soundBufferSamples[i] / (float) short.MaxValue;
				}
			} else {
				int sampleLength = soundBuffer.Samples.Length;
				this.samples = new float[sampleLength];

				short[] soundBufferSamples = soundBuffer.Samples;
				for (int i = 0; i < sampleLength; i++) {
					this.samples[i] = soundBufferSamples[i] / (float) short.MaxValue;
				}
			}
		}

		public void SetVolume(float volume) {
			this.volume = volume;
			for (int i = 0; i < this.soundBanks.Count; i++) {
				this.soundBanks[i].volume = this.volume;
			}
		}

		public void SetPitch(float pitch) {
			this.pitch = pitch;
			for (int i = 0; i < this.soundBanks.Count; i++) {
				this.soundBanks[i].pitch = this.pitch;
			}
		}

		public void Play() {
			this.playState = PlayState.PLAYING;
			SoundBank soundBank = new SoundBank(this.samples);
			soundBank.volume = this.volume;
			soundBank.pitch = this.pitch;
			soundBank.Play();
			this.soundBanks.Add(soundBank);
		}

		public void Pause() {
			this.playState = PlayState.PAUSED;
			for (int i = 0; i < this.soundBanks.Count; i++) {
				this.soundBanks[i].Pause();
			}
		}

		public void Stop() {
			this.playState = PlayState.STOPPED;
			this.soundBanks.Clear();
		}

		public override void Process(ref float[] samples) {
			if (this.playState == PlayState.PLAYING) {
				for (int i = this.soundBanks.Count - 1; i >= 0; i--) {
					this.soundBanks[i].Process(ref samples);

					if (this.soundBanks[i].playState == PlayState.STOPPED) {
						this.soundBanks.Remove(this.soundBanks[i]);
					}
				}

				if (this.soundBanks.Count == 0) {
					this.Stop();
				}
			}

			base.Process(ref samples);
		}
	}

	public class SoundBank {
		private float[] samples;
		private int sampleLength;
		private int samplePos;
		private float sampleStartTime;

		public PlayState playState;

		public float volume;
		public float pitch;

		public SoundBank(float[] samples) {
			this.samples = samples;
			this.sampleLength = this.samples.Length;
			this.playState = PlayState.STOPPED;
			this.samplePos = 0;
			this.volume = 1f;
			this.pitch = 1f;
		}

		public void Process(ref float[] samples) {
			if (this.playState == PlayState.PLAYING) {
				float currentTime = Engine.audio.mixer.startTime;
				float timeInc = 1f / Engine.audio.sampleRate / 2f;

				for (int i = 0; i < samples.Length; i += 2) {
					if (currentTime >= this.sampleStartTime) {
						int leftSample = (int) Math.Round((double) this.samplePos, MidpointRounding.ToEven);
						int rightSample = leftSample + 1;

						samples[i + 0] += this.samples[leftSample] * this.volume;
						samples[i + 1] += this.samples[rightSample] * this.volume;

						this.samplePos += (int) (2f * this.pitch);
						if (this.samplePos >= this.sampleLength) {
							this.Stop();
							break;
						}
					}

					currentTime += timeInc * 2f;
				}
			}
		}

		public void Play() {
			this.playState = PlayState.PLAYING;
			this.sampleStartTime = Engine.audio.mixer.GetTime() + Engine.audio.mixer.bufferTime;
		}

		public void Pause() {
			this.playState = PlayState.PAUSED;
		}

		public void Stop() {
			this.playState = PlayState.STOPPED;
			this.samplePos = 0;
		}
	}

	public enum PlayState {
		PLAYING,
		PAUSED,
		STOPPED
	}
}