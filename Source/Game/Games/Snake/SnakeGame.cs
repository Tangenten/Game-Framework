using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helpers;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using TangentEngine;

namespace Snake {
	public abstract class SnakePart {
		public Vector2f position;
		public float heading; //degrees
		public float size;

		public SnakePart() { }

		public SnakePart(Vector2f pos, float head, float size) {
			this.position = pos;
			this.heading = head;
			this.size = size;
		}
	}

	public class SnakeBody : SnakePart {
		public SnakePart chasing;
		public Color color;

		public SnakeBody(SnakePart snakePart) {
			this.chasing = snakePart;

			Vector2f posToChase = this.chasing.position + -VectorH.DegreeToVector(this.chasing.heading) * this.chasing.size;
			this.position = posToChase;
			this.heading = VectorH.VectorToDegree(this.chasing.position - this.position);
			this.size = this.chasing.size / 1.10f;

			this.color = RandomH.GetRandomColor(new Color(45, 145, 64, 255), 25);
		}

		public void Update() {
			Vector2f posToChase = this.chasing.position + -VectorH.DegreeToVector(this.chasing.heading) * this.chasing.size;

			Vector2f direction = posToChase - this.position;
			float length = direction.Magnitude();

			this.heading = VectorH.VectorToDegree(posToChase - this.position);
			this.position += VectorH.DegreeToVector(this.heading) * (length * 0.35f);
		}

		public void Render() {
			RectangleShape rectangleShape = new RectangleShape(new Vector2f(this.size, this.size / 2f));
			rectangleShape.Origin = rectangleShape.Size / 2f;
			rectangleShape.Position = this.position;
			rectangleShape.Rotation = this.heading;
			rectangleShape.FillColor = this.color;

			Engine.graphics.Draw(rectangleShape);
		}
	}

	public class SnakeHead : SnakePart {
		public float speed;

		public SnakeHead() {
			this.position = new Vector2f(1920 / 2f, 1080 / 2f);
			this.heading = RandomH.GetRandom(0f, 360f);
			this.size = 128f;
			this.speed = 1000;
		}

		public void Update() {
			if (Engine.input.Key((KeyButton.A, ButtonState.HELD))) {
				this.heading = MathH.Mod(this.heading - 5, 360f);
				this.speed *= 1.03f;
			}

			if (Engine.input.Key((KeyButton.D, ButtonState.HELD))) {
				this.heading = MathH.Mod(this.heading + 5, 360f);
				this.speed *= 1.03f;
			}

			TweenH.SmoothToTarget(ref this.speed, 500, 0.05f);
			this.position += VectorH.DegreeToVector(this.heading) * this.speed * Engine.time.deltaGameTime;
		}

		public void Render() {
			CircleShape circleShape = new CircleShape(this.size / 2f);
			circleShape.Origin = new Vector2f(circleShape.Radius, circleShape.Radius);
			circleShape.Position = this.position;
			circleShape.FillColor = new Color(9, 74, 10, 255);

			Engine.graphics.Draw(circleShape);
		}
	}

	public class Snake {
		public SnakeHead snakeHead;
		public List<SnakeBody> snakeBodies;

		public Snake() {
			this.snakeHead = new SnakeHead();
			this.snakeBodies = new List<SnakeBody>();

			this.snakeBodies.Add(new SnakeBody(this.snakeHead));
			this.AddSnakePart();
			this.AddSnakePart();
			this.AddSnakePart();
			this.AddSnakePart();
		}

		public void Update() {
			this.snakeHead.Update();

			for (int i = 0; i < this.snakeBodies.Count; i++) {
				this.snakeBodies[i].Update();
			}
		}

		public void Render() {
			this.snakeHead.Render();

			for (int i = 0; i < this.snakeBodies.Count; i++) {
				this.snakeBodies[i].Render();
			}
		}

		public void AddSnakePart() {
			this.snakeBodies.Add(new SnakeBody(this.snakeBodies[^1]));
		}
	}

	public class Snack {
		public static SpriteSheet foodSpriteSheet = new SpriteSheet(
			Engine.assets.GetStream("Engine.Source.Game.Games.Snake.Assets.Sprites.Snacks.food-sheet.png"),
			Engine.assets.GetStream("Engine.Source.Game.Games.Snake.Assets.Sprites.Snacks.food-sheet.txt"));

		public Vector2f position;
		public float size;
		public RenTexSprite sprite;

		public Snack(Vector2f pos) {
			this.position = pos;
			this.size = 32f;

			this.sprite = new RenTexSprite(foodSpriteSheet.texture, foodSpriteSheet.GetRandomTextureCoords());
			this.sprite.SetOrigin(Origins.MIDDLE);
			this.sprite.ScaleTo(new Vector2f(256f, 256f));
		}

		public void Update() {
			this.sprite.RotateTo(OscH.SinOsc(-25f, 25f, 2f, 0f, Engine.time.elapsedGameTime));
		}

		public void Render() {
			this.sprite.MoveTo(this.position);
			Engine.graphics.Draw(this.sprite);
		}

		public bool CollisionCheck(SnakePart snakePart) {
			RectangleShape rectangleShapeSnack = new RectangleShape(new Vector2f(this.size * 2, this.size * 2));
			rectangleShapeSnack.Origin = rectangleShapeSnack.Size / 2f;
			rectangleShapeSnack.Position = this.position;

			RectangleShape rectangleShapeSnakePart = new RectangleShape(new Vector2f(snakePart.size * 2, snakePart.size * 2));
			rectangleShapeSnakePart.Origin = rectangleShapeSnakePart.Size / 2f;
			rectangleShapeSnakePart.Position = snakePart.position;

			return rectangleShapeSnack.GetGlobalBounds().Intersects(rectangleShapeSnakePart.GetGlobalBounds());
		}
	}

	public class Snacks {
		public List<Snack> snacks;
		public static event Action<Snack>? snackEatEvent;

		public Snacks() {
			this.snacks = new List<Snack>();
		}

		public void Update(Snake snake) {
			if (this.snacks.Count == 0) {
				this.snacks.Add(new Snack(RandomH.GetRandomVector(0, Engine.camera.gameWidth, 0, Engine.camera.gameHeight)));
			}

			for (int i = this.snacks.Count - 1; i >= 0; i--) {
				this.snacks[i].Update();
				if (this.snacks[i].CollisionCheck(snake.snakeHead)) {
					snackEatEvent?.Invoke(this.snacks[i]);
					this.snacks.RemoveAt(i);
					snake.AddSnakePart();
				}
			}
		}

		public void Render() {
			for (int i = 0; i < this.snacks.Count; i++) {
				this.snacks[i].Render();
			}
		}
	}

	public class SnakeAudio {
		public List<Sound> snackSounds;
		public Sound currentMusic;

		public SnakeAudio() {
			Snacks.snackEatEvent += this.PlaySnackSound;

			this.snackSounds = new List<Sound>();

			Task.Run(() => {
				this.snackSounds.Add(Engine.audio.GetSound("Engine.Source.Game.Games.Snake.Assets.Sounds.Snacks.sound_01.ogg"));
				this.snackSounds.Add(Engine.audio.GetSound("Engine.Source.Game.Games.Snake.Assets.Sounds.Snacks.sound_02.ogg"));
				this.snackSounds.Add(Engine.audio.GetSound("Engine.Source.Game.Games.Snake.Assets.Sounds.Snacks.sound_03.ogg"));
			});

			this.currentMusic = new Sound();
		}

		public void Update() { }

		public void PlaySnackSound(Snack snack) {
			if (this.snackSounds.Count > 0) {
				Sound snackSound = this.snackSounds.GetRandomElement();
				snackSound.Volume = RandomH.GetRandom(50f, 60f);
				snackSound.Pitch = RandomH.GetRandom(0.9f, 1.5f);
				snackSound.Play();
			}
		}
	}

	public class SnakeChompText {
		public static List<string> messages = new List<string> {"Chomp !", "Yum !", "Nutritious !", "Sweet !", "Sssss...", "Crunch !"};

		public Text text;

		public SnakeChompText(Snack snack) {
			this.text = new Text(messages.GetRandomElement(), new Font(Engine.assets.GetStream("Engine.Source.Engine.Assets.Fonts.Quicksand.ttf")));
			this.text.CharacterSize = 96;
			this.text.FillColor = RandomH.GetRandomColor();
			this.text.Origin = new Vector2f(this.text.GetLocalBounds().Width / 2f, this.text.GetLocalBounds().Height / 2f);
			this.text.Position = snack.position;
		}

		public void Update() {
			Vector2f newPos = this.text.Position;
			newPos.Y -= 10;

			this.text.Position = newPos;

			Color newCol = this.text.FillColor;
			if (newCol.A <= 5) {
				newCol.A = 0;
			} else {
				newCol.A -= 5;
			}

			this.text.FillColor = newCol;

			this.text.Rotation = OscH.SinOsc(-25f, 25f, 4f, 0f, Engine.time.elapsedGameTime);
		}

		public void Render() {
			Engine.graphics.Draw(this.text);
		}

		public bool CheckAlive() {
			return this.text.FillColor.A > 0;
		}
	}

	public class SnakeGui {
		public List<SnakeChompText> snakeChomps;

		public Text snacksEatenText;
		public int snacksEaten;

		public SnakeGui() {
			Snacks.snackEatEvent += this.SnackEatEvent;

			this.snakeChomps = new List<SnakeChompText>();

			this.snacksEatenText = new Text("", new Font(Engine.assets.GetStream("Engine.Source.Engine.Assets.Fonts.Quicksand.ttf")));
			this.snacksEatenText.DisplayedString = "Snacks Eaten: " + this.snacksEaten;
			this.snacksEatenText.CharacterSize = 64;
			this.snacksEatenText.Origin = new Vector2f(this.snacksEatenText.GetLocalBounds().Width / 2f, this.snacksEatenText.GetLocalBounds().Height / 2f);
			this.snacksEatenText.Position = new Vector2f(Engine.camera.gameWidth / 2f, 128);
		}

		public void Update() {
			for (int i = this.snakeChomps.Count - 1; i >= 0; i--) {
				this.snakeChomps[i].Update();

				if (!this.snakeChomps[i].CheckAlive()) {
					this.snakeChomps.RemoveAt(i);
				}
			}
		}

		public void Render() {
			for (int i = 0; i < this.snakeChomps.Count; i++) {
				this.snakeChomps[i].Render();
			}

			Engine.graphics.Draw(this.snacksEatenText);
		}

		public void SnackEatEvent(Snack snack) {
			this.snacksEaten++;
			this.snacksEatenText.DisplayedString = "Snacks Eaten: " + this.snacksEaten;

			this.snakeChomps.Add(new SnakeChompText(snack));
		}
	}

	public class SnakeBackground {
		public RenTexSprite backgroundSprite;

		public SnakeBackground() {
			this.backgroundSprite = new RenTexSprite(new Texture(Engine.assets.GetStream("Engine.Source.Game.Games.Snake.Assets.Backgrounds.background-002.jpg")));
			this.backgroundSprite.ScaleTo(Engine.camera.gameSize);
			this.backgroundSprite.SetSmooth(true);
		}

		public void Update() { }

		public void Render() {
			Engine.graphics.Draw(this.backgroundSprite);
		}
	}

	public class SnakeGame : GameE {
		public Snake snake;
		public Snacks snacks;
		public SnakeAudio snakeAudio;
		public SnakeGui snakeGui;
		public SnakeBackground snakeBackground;

		public override void Start() {
			this.snake = new Snake();
			this.snacks = new Snacks();
			this.snakeAudio = new SnakeAudio();
			this.snakeGui = new SnakeGui();
			this.snakeBackground = new SnakeBackground();
		}

		public override void Update() {
			this.snake.Update();
			this.snacks.Update(this.snake);
			this.snakeGui.Update();
			this.snakeAudio.Update();

			this.snakeBackground.Render();
			this.snacks.Render();
			this.snake.Render();
			this.snakeGui.Render();
		}

		public override void Stop() { }
	}
}