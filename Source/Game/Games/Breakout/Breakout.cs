using System;
using System.Collections.Generic;
using Helpers;
using SFML.Graphics;
using SFML.System;
using TangentEngine;

namespace Brekaout {
	public class BreakoutGraphics {
		public SpriteSheet spriteSheet;

		public GraphicsTrack paddleTrack;
		public GraphicsTrack ballTrack;
		public GraphicsTrack bricksTrack;
		public GraphicsTrack textTrack;
		public GraphicsTrack backgroundTrack;

		public VignetteEffect vignetteEffect;
		public DitherEffect ditherEffect;
		public FadeEffect fadeEffect;

		public BreakoutGraphics() {
			this.spriteSheet = new SpriteSheet(Engine.assets.GetStream("breakout_spritesheet.png"), Engine.assets.GetStream("breakout_spritesheet.txt"));

			this.paddleTrack = new GraphicsTrack("Paddle", 2);
			this.ballTrack = new GraphicsTrack("Ball", 2);
			this.bricksTrack = new GraphicsTrack("Bricks", 2);
			this.textTrack = new GraphicsTrack("Text", 3);
			this.backgroundTrack = new GraphicsTrack("Background", 1);

			Engine.graphics.mixer.AddGraphicsTrack(this.paddleTrack);
			Engine.graphics.mixer.AddGraphicsTrack(this.ballTrack);
			Engine.graphics.mixer.AddGraphicsTrack(this.bricksTrack);
			Engine.graphics.mixer.AddGraphicsTrack(this.textTrack);
			Engine.graphics.mixer.AddGraphicsTrack(this.backgroundTrack);

			this.fadeEffect = new FadeEffect();
			this.fadeEffect.SetDecayFactor(0.98f);
			this.fadeEffect.SetRandomFactor(0.1f);
			this.ballTrack.AddGraphicsEffector(this.fadeEffect);

			this.ditherEffect = new DitherEffect();
			this.backgroundTrack.AddGraphicsEffector(this.ditherEffect);

			this.vignetteEffect = new VignetteEffect();
			Engine.graphics.mixer.AddGraphicsEffector(this.vignetteEffect);
		}
	}

	public class BreakoutAudio {
		public AudioTrack musicTrack;
		public MeasureEffect musicTrackMeasure;
		public SoundPlayer music;

		public AudioTrack brickTrack;

		public AudioTrack paddleTrack;

		public SoftclipEffect softclipEffect;

		public BreakoutAudio() {
			this.softclipEffect = new SoftclipEffect();
			Engine.audio.mixer.AddAudioEffector(this.softclipEffect);

			this.musicTrack = new AudioTrack("Music");

			this.musicTrackMeasure = new MeasureEffect();
			this.musicTrack.AddAudioEffector(this.musicTrackMeasure);

			Engine.audio.GetSoundBufferAsync("breakout_music_01.ogg", buffer => {
				this.music = new SoundPlayer(buffer);
				//this.music.Play();
				this.musicTrack.AddAudioProvider(this.music);
			});

			this.brickTrack = new AudioTrack("Bricks");
			this.paddleTrack = new AudioTrack("Paddle");

			Engine.audio.mixer.AddAudioTrack(this.musicTrack);
			Engine.audio.mixer.AddAudioTrack(this.brickTrack);
			Engine.audio.mixer.AddAudioTrack(this.paddleTrack);
		}
	}

	public class BreakoutBackground {
		public RenTexSprite background;

		public BreakoutBackground() {
			this.background = new RenTexSprite(Engine.graphics.resolutionWidth / 16, Engine.graphics.resolutionHeight / 16, Engine.window.contextSettings);
			this.background.ScaleToCamera();
			this.background.SetViewToCamera();
			this.background.SetSmooth(true);
			this.background.Clear();

			Breakout.graphics.backgroundTrack.AddGraphicsProvider(this.background);
		}

		public void Update() {
			float audioStrength = Breakout.audio.musicTrackMeasure.peak;
			byte colorStrength = (byte) (audioStrength * 255f);

			this.background.Clear(new Color(colorStrength, colorStrength, colorStrength, 96));
		}
	}

	public class BreakoutPaddle {
		public TexSprite paddle;

		public BreakoutPaddle() {
			this.paddle = new TexSprite(Breakout.graphics.spriteSheet.texture, Breakout.graphics.spriteSheet.GetTextureCoords("paddle"));
			this.paddle.SetOrigin(Origins.MIDDLE);
			this.paddle.MoveTo(new Vector2f(Engine.camera.gameWidth / 2f, Engine.camera.gameHeight - this.paddle.GetTextureRectSize().Y));

			Breakout.graphics.paddleTrack.AddGraphicsProvider(this.paddle);
		}

		public void Update() {
			if (Engine.input.Key("Breakout", (KeyButton.A, ButtonState.HELD))) {
				this.paddle.MoveBy(new Vector2f(-25, 0));

				if (this.paddle.Position().X < this.paddle.Size().X / 2f) {
					this.paddle.MoveTo(new Vector2f(this.paddle.Size().X / 2f, Engine.camera.gameHeight - this.paddle.GetTextureRectSize().Y));
				}
			}

			if (Engine.input.Key("Breakout", (KeyButton.D, ButtonState.HELD))) {
				this.paddle.MoveBy(new Vector2f(25, 0));

				if (this.paddle.Position().X > Engine.camera.gameWidth - this.paddle.Size().X / 2f) {
					this.paddle.MoveTo(new Vector2f(Engine.camera.gameWidth - this.paddle.Size().X / 2f, Engine.camera.gameHeight - this.paddle.GetTextureRectSize().Y));
				}
			}
		}
	}

	public class BreakoutScore {
		public float score;
		public TextE scoreText;

		public BreakoutScore() {
			this.scoreText = new TextE(Engine.assets.GetStream("Quicksand.ttf"));
			this.scoreText.SetDisplayedString("Score : " + this.score);
			this.scoreText.SetDisplayedStringSize(100);
			this.scoreText.SetOrigin(Origins.MIDDLE);
			this.scoreText.SetDisplayedStringOutlineThickness(4);
			this.scoreText.SetDisplayedStringOutlineColor(Color.Black);
			this.scoreText.MoveTo(new Vector2f(Engine.camera.gameWidth / 2f, 128f));

			Breakout.graphics.textTrack.AddGraphicsProvider(this.scoreText);

			BreakoutBricks.brickDestroyedEvent += this.BrickDestrotedEvent;
		}

		public void BrickDestrotedEvent(BreakoutBrick brick) {
			this.score++;
			this.scoreText.SetDisplayedString("Score : " + this.score);
		}
	}

	public class BreakoutBall {
		public TexSprite ball;
		public Vector2f heading;
		public float speed;

		public MultiSoundPlayer paddleBounceSound;

		public BreakoutBall() {
			this.ball = new TexSprite(Breakout.graphics.spriteSheet.texture, Breakout.graphics.spriteSheet.GetTextureCoords("ball"));
			this.ball.SetOrigin(Origins.MIDDLE);
			this.ball.ScaleBy(0.15f);

			Breakout.graphics.ballTrack.AddGraphicsProvider(this.ball);

			this.paddleBounceSound = new MultiSoundPlayer();
			this.paddleBounceSound.playType = PlayType.RANDOM;
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_01.ogg")));
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_02.ogg")));
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_03.ogg")));
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_04.ogg")));
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_05.ogg")));
			this.paddleBounceSound.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_paddle_06.ogg")));

			Breakout.audio.paddleTrack.AddAudioProvider(this.paddleBounceSound);

			this.speed = 15f;
			this.RespawnBall();
		}

		public void Update() {
			this.ball.MoveBy(this.heading * this.speed);
			this.HandleBoundaryCollisions();

			this.ball.RotateBy(Breakout.audio.musicTrackMeasure.peak * 16);
		}

		public void HandleBoundaryCollisions() {
			if (this.ball.Position().X < 0) {
				this.heading.X *= -1;
			}

			if (this.ball.Position().X > Engine.camera.gameWidth) {
				this.heading.X *= -1;
			}

			if (this.ball.Position().Y < 0) {
				this.heading.Y *= -1;
			}

			if (this.ball.Position().Y > Engine.camera.gameHeight) {
				this.RespawnBall();
			}
		}

		public void RespawnBall() {
			this.ball.MoveTo(Engine.camera.gameSize / 2f);
			this.heading = VectorH.DegreeToVector(RandomH.GetRandom(45f, 135f));
		}

		public void HandlePaddleCollision(BreakoutPaddle paddle) {
			if (this.ball.Collides(paddle.paddle)) {
				Vector2f v = VectorH.VectorBetweenTwoPoints(paddle.paddle.Position(), this.ball.Position());
				v.Normalize();
				this.heading = v;
				this.paddleBounceSound.Play();
			}
		}
	}

	public class BreakoutBricks {
		public static event Action<BreakoutBrick>? brickDestroyedEvent;

		public List<BreakoutBrick> breakoutBricks;
		public MultiSoundPlayer brickSounds;

		public BreakoutBricks() {
			this.breakoutBricks = new List<BreakoutBrick>();

			this.brickSounds = new MultiSoundPlayer();
			this.brickSounds.SetVolume(0.4f);
			this.brickSounds.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_brick_01.ogg")));
			this.brickSounds.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_brick_02.ogg")));
			this.brickSounds.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_brick_03.ogg")));
			this.brickSounds.AddAudioPlayer(new SoundPlayer(Engine.audio.GetSoundBuffer("breakout_brick_04.ogg")));

			Breakout.audio.brickTrack.AddAudioProvider(this.brickSounds);

			this.CreateBricks();
		}

		public void CreateBricks() {
			float bricksWidthCount = 16;
			float bricksHeightCount = 16;

			float widthStart = Engine.camera.gameWidth / 16f;
			float widthEnd = Engine.camera.gameWidth - widthStart;
			float brickWidth = (widthEnd - widthStart) / bricksWidthCount;

			float heightStart = Engine.camera.gameHeight / 16f;
			float heightEnd = Engine.camera.gameHeight / 2f;
			float brickHeight = (heightEnd - heightStart) / bricksHeightCount;

			for (int i = 0; i < bricksWidthCount + 1; i++) {
				for (int j = 0; j < bricksHeightCount + 1; j++) {
					Vector2f pos = new Vector2f(
						TweenH.Linear(i, 0, bricksWidthCount, widthStart, widthEnd),
						TweenH.Linear(j, 0, bricksHeightCount, heightStart, heightEnd));
					Vector2f size = new Vector2f(brickWidth, brickHeight);
					this.breakoutBricks.Add(new BreakoutBrick(pos, size));
				}
			}
		}

		public void HandleBallCollision(BreakoutBall ball) {
			for (int i = this.breakoutBricks.Count - 1; i >= 0; i--) {
				if (this.breakoutBricks[i].HandleBallCollision(ball)) {
					brickDestroyedEvent?.Invoke(this.breakoutBricks[i]);
					this.brickSounds.Play();
					this.breakoutBricks[i].RemoveSelf();
					this.breakoutBricks.Remove(this.breakoutBricks[i]);
				}
			}
		}
	}

	public class BreakoutBrick {
		public TexSprite brick;

		public BreakoutBrick(Vector2f position, Vector2f size) {
			this.brick = new TexSprite(Breakout.graphics.spriteSheet.texture, Breakout.graphics.spriteSheet.GetTextureCoords("brick"));
			this.brick.SetOrigin(Origins.MIDDLE);
			this.brick.MoveTo(position);
			this.brick.ScaleTo(size);

			Breakout.graphics.bricksTrack.AddGraphicsProvider(this.brick);
		}

		public bool HandleBallCollision(BreakoutBall ball) {
			return this.brick.Collides(ball.ball);
		}

		public void RemoveSelf() {
			Breakout.graphics.bricksTrack.RemoveGraphicsProvider(this.brick);
		}
	}

	public class Breakout : GameE {
		public static BreakoutGraphics graphics;
		public static BreakoutAudio audio;

		public BreakoutPaddle paddle;
		public BreakoutBall ball;
		public BreakoutBricks bricks;
		public BreakoutScore score;
		public BreakoutBackground background;

		public override void Start() {
			Engine.input.PushContext("Breakout");

			graphics = new BreakoutGraphics();
			audio = new BreakoutAudio();

			this.paddle = new BreakoutPaddle();
			this.ball = new BreakoutBall();
			this.bricks = new BreakoutBricks();
			this.score = new BreakoutScore();
			this.background = new BreakoutBackground();
		}

		public override void Update() {
			this.paddle.Update();
			this.ball.Update();
			this.ball.HandlePaddleCollision(this.paddle);
			this.bricks.HandleBallCollision(this.ball);
			this.background.Update();
		}

		public override void Stop() {
			Engine.input.PopContext("Breakout");
		}
	}
}