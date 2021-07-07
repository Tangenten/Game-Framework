using System.Collections.Generic;
using Helpers;

namespace TangentEngine {
	public class MultiSoundPlayer : AudioProvider {
		private List<SoundPlayer> players;
		private int playerIndex;
		private int playerCount;

		public PlayType playType;
		private PlayState playState;

		private float volume;
		private float pitch;

		public MultiSoundPlayer() {
			this.players = new List<SoundPlayer>();
			this.playerIndex = -1;
			this.playerCount = 0;
			this.volume = 1f;
			this.pitch = 1f;

			this.playType = PlayType.ROUND_ROBIN;
			this.playState = PlayState.STOPPED;
		}

		public void SetVolume(float volume) {
			this.volume = volume;
			for (int i = 0; i < this.players.Count; i++) {
				this.players[i].SetVolume(this.volume);
			}
		}

		public void SetPitch(float pitch) {
			this.pitch = pitch;
			for (int i = 0; i < this.players.Count; i++) {
				this.players[i].SetPitch(this.pitch);
			}
		}

		public void AddAudioPlayer(SoundPlayer soundPlayer) {
			soundPlayer.SetVolume(this.volume);
			soundPlayer.SetPitch(this.pitch);
			this.players.Add(soundPlayer);
			this.playerCount++;
		}

		public void Play() {
			if (this.playType == PlayType.ROUND_ROBIN) {
				this.playerIndex = MathH.Mod(this.playerIndex + 1, this.playerCount);
				this.players[this.playerIndex].Play();
			} else if (this.playType == PlayType.RANDOM) {
				this.playerIndex = RandomH.GetRandom(0, this.playerCount - 1);
				this.players[this.playerIndex].Play();
			}

			this.playState = PlayState.PLAYING;
		}

		public void Pause() {
			this.playState = PlayState.PAUSED;
			for (int i = 0; i < this.playerCount; i++) {
				this.players[i].Pause();
			}
		}

		public void Stop() {
			this.playState = PlayState.STOPPED;
			for (int i = 0; i < this.playerCount; i++) {
				this.players[i].Stop();
			}
		}

		public override void Process(ref float[] samples) {
			if (this.playState == PlayState.PLAYING) {
				for (int i = 0; i < this.playerCount; i++) {
					this.players[i].Process(ref samples);
				}
			}

			base.Process(ref samples);
		}
	}

	public enum PlayType {
		ROUND_ROBIN,
		RANDOM
	}
}