using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using HPCsharp;
using SFML.Audio;
using SFML.System;

namespace TangentEngine {
	public class AudioE {
		private Dictionary<string, SoundBuffer> soundBufferCache;

		public int bufferSize;
		public int channels;
		public int sampleRate;

		public AudioMixer mixer;

		public AudioE() {
			this.soundBufferCache = new Dictionary<string, SoundBuffer>();

			this.sampleRate = 44100;
			this.channels = 2;
			this.bufferSize = this.sampleRate / 16;

			// Check that BufferSize is even, needed if we use 2 channels
			if (this.bufferSize % 2 == 1) {
				this.bufferSize++;
			}

			this.mixer = new AudioMixer(this.channels, this.sampleRate);
			this.StartMixer();
		}

		public void StartMixer() {
			this.mixer.Play();
		}

		public void StopMixer() {
			this.mixer.Stop();
		}

		public void SetListenerPosition(in Vector2f position) {
			Listener.Position = new Vector3f(position.X, position.Y, 0f);
		}

		public void SetListenerDirection(in Vector2f direction) {
			Listener.Direction = new Vector3f(direction.X, direction.Y, 0f);
		}

		private void HandleGlobalVolume() { }

		public Sound GetSound(string resourceString) {
			lock (this.soundBufferCache) {
				if (!this.soundBufferCache.ContainsKey(resourceString)) {
					Stream stream = Engine.assets.GetStream(resourceString);
					SoundBuffer soundBuffer = new SoundBuffer(stream);
					this.soundBufferCache[resourceString] = soundBuffer;
				}
			}

			return new Sound(this.soundBufferCache[resourceString]);
		}

		public void GetSoundAsync(string resourceString, Action<Sound> onDoneLoading) {
			Task.Run(() => {
				lock (this.soundBufferCache) {
					if (!this.soundBufferCache.ContainsKey(resourceString)) {
						Stream stream = Engine.assets.GetStream(resourceString);
						SoundBuffer soundBuffer = new SoundBuffer(stream);
						this.soundBufferCache[resourceString] = soundBuffer;
					}
				}

				onDoneLoading.Invoke(new Sound(this.soundBufferCache[resourceString]));
			});
		}

		public SoundBuffer GetSoundBuffer(string resourceString) {
			lock (this.soundBufferCache) {
				if (!this.soundBufferCache.ContainsKey(resourceString)) {
					Stream stream = Engine.assets.GetStream(resourceString);
					SoundBuffer soundBuffer = new SoundBuffer(stream);
					this.soundBufferCache[resourceString] = soundBuffer;
				}
			}

			return this.soundBufferCache[resourceString];
		}

		public void GetSoundBufferAsync(string resourceString, Action<SoundBuffer> onDoneLoading) {
			Task.Run(() => {
				lock (this.soundBufferCache) {
					if (!this.soundBufferCache.ContainsKey(resourceString)) {
						Stream stream = Engine.assets.GetStream(resourceString);
						SoundBuffer soundBuffer = new SoundBuffer(stream);
						this.soundBufferCache[resourceString] = soundBuffer;
					}
				}

				onDoneLoading.Invoke(this.soundBufferCache[resourceString]);
			});
		}

		public void Update() {
			this.HandleGlobalVolume();
		}

		public void Reset() {
			this.soundBufferCache.Clear();
			this.mixer.Reset();
		}
	}

	public class AudioMixer : SoundStream {
		private List<AudioTrack> audioTracks;
		private List<AudioEffector> audioEffectors;
		private float[] mixerSamples;

		public float startTime;
		public float endTime;
		public float bufferTime;

		public Stopwatch stopwatch;

		private bool running;

		public AudioMixer(int channels, int sampleRate) {
			this.audioTracks = new List<AudioTrack>();
			this.audioEffectors = new List<AudioEffector>();
			this.mixerSamples = new float[1];

			this.stopwatch = new Stopwatch();
			this.stopwatch.Start();

			this.Initialize((uint) channels, (uint) sampleRate);
		}

		public float GetTime() {
			return (float) this.stopwatch.Elapsed.TotalSeconds;
		}

		protected override bool OnGetData(out short[] samples) {
			if (this.running) {
				Console.WriteLine("Audio Overrun");
			}

			this.running = true;

			this.startTime = this.GetTime();
			this.bufferTime = Engine.audio.bufferSize / (float) Engine.audio.sampleRate / 2f;
			this.endTime = this.startTime + this.bufferTime;

			if (this.mixerSamples.Length < Engine.audio.bufferSize) {
				this.mixerSamples = new float[Engine.audio.bufferSize];
			} else {
				this.mixerSamples.FillGenericSse(0f);
			}

			for (int i = 0; i < this.audioTracks.Count; i++) {
				this.audioTracks[i].Process(ref this.mixerSamples);
			}

			for (int i = 0; i < this.audioEffectors.Count; i++) {
				this.audioEffectors[i].Process(ref this.mixerSamples);
			}

			samples = new short[Engine.audio.bufferSize];
			for (int i = 0; i < samples.Length; i++) {
				samples[i] = (short) (Math.Clamp(this.mixerSamples[i], -1f, 1f) * short.MaxValue);
			}

			this.running = false;
			return true;
		}

		protected override void OnSeek(Time timeOffset) { }

		public void AddAudioTrack(AudioTrack audioTrack) {
			this.audioTracks.Add(audioTrack);
		}

		public void AddAudioEffector(AudioEffector effector) {
			this.audioEffectors.Add(effector);
		}

		public bool AudioTrackExists(int index) {
			return this.audioTracks.Count < index;
		}

		public AudioTrack GetAudioTrack(int index) {
			return this.audioTracks[index];
		}

		public bool AudioTrackExists(string audioTrackName) {
			for (int i = 0; i < this.audioTracks.Count; i++) {
				if (this.audioTracks[i].name == audioTrackName) {
					return true;
				}
			}

			return false;
		}

		public AudioTrack GetAudioTrack(string audioTrackName) {
			for (int i = 0; i < this.audioTracks.Count; i++) {
				if (this.audioTracks[i].name == audioTrackName) {
					return this.audioTracks[i];
				}
			}

			return null;
		}

		public void Reset() {
			this.audioTracks.Clear();
			this.audioEffectors.Clear();
		}
	}

	public class AudioTrack {
		private List<AudioProvider> audioProviders;
		private List<AudioEffector> audioEffectors;

		public string name;

		private float[] audioTrackSamples;

		public AudioTrack(string name = "") {
			this.audioProviders = new List<AudioProvider>();
			this.audioEffectors = new List<AudioEffector>();
			this.audioTrackSamples = new float[1];

			this.name = name;
		}

		public void Process(ref float[] samples) {
			if (this.audioTrackSamples.Length < samples.Length) {
				this.audioTrackSamples = new float[samples.Length];
			} else {
				this.audioTrackSamples.FillGenericSse(0f);
			}

			for (int i = 0; i < this.audioProviders.Count; i++) {
				this.audioProviders[i].Process(ref this.audioTrackSamples);
			}

			for (int i = 0; i < this.audioEffectors.Count; i++) {
				this.audioEffectors[i].Process(ref this.audioTrackSamples);
			}

			int simdLength = Vector<float>.Count;

			int j = 0;
			for (j = 0; j <= samples.Length - simdLength; j += simdLength) {
				Vector<float> vectorSection = new Vector<float>(samples, j);
				Vector<float> vectorSection2 = new Vector<float>(this.audioTrackSamples, j);
				(vectorSection + vectorSection2).CopyTo(samples, j);
			}

			for (; j < samples.Length; j++) {
				samples[j] += this.audioTrackSamples[j];
			}
		}

		public void AddAudioProvider(AudioProvider audioProvider) {
			this.audioProviders.Add(audioProvider);
		}

		public void AddAudioEffector(AudioEffector audioEffector) {
			this.audioEffectors.Add(audioEffector);
		}
	}

	public abstract class AudioEffector {
		public abstract void Process(ref float[] samples);
	}

	public abstract class AudioProvider {
		private List<AudioEffector> audioEffectors;

		public AudioProvider() {
			this.audioEffectors = new List<AudioEffector>();
		}

		public virtual void Process(ref float[] samples) {
			for (int i = 0; i < this.audioEffectors.Count; i++) {
				this.audioEffectors[i].Process(ref samples);
			}
		}

		public void AddAudioEffector(AudioEffector effector) {
			this.audioEffectors.Add(effector);
		}
	}
}