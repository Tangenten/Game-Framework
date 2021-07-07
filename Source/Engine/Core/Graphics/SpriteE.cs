using Helpers;
using SFML.Graphics;
using SFML.System;

namespace TangentEngine {
	public abstract class SpriteE : GraphicProvider {
		public int textureWidth;
		public int textureHeight;
		public Vector2f textureSize;

		public BlendMode blendMode;
		public Shader shader;
		public Transform transform;

		public Sprite sprite;

		public SpriteE() {
			this.shader = null;
			this.blendMode = BlendMode.Alpha;
			this.transform = new Transform(
				1, 0, 0,
				0, 1, 0,
				0, 0, 1
			);
		}

		public Vector2f Position() {
			return this.sprite.Position;
		}

		public Vector2f Size() {
			return new Vector2f(this.sprite.GetGlobalBounds().Width, this.sprite.GetGlobalBounds().Height);
		}

		public void MoveTo(Vector2f position) {
			this.sprite.Position = position;
		}

		public void MoveBy(Vector2f position) {
			this.sprite.Position += position;
		}

		public void ScaleTo(Vector2f size) {
			this.sprite.Scale = new Vector2f(size.X / this.GetTextureRectSize().X, size.Y / this.GetTextureRectSize().Y);
		}

		public void ScaleTo(FloatRect rect) {
			this.sprite.Origin = new Vector2f(0f, 0f);
			this.sprite.Position = new Vector2f(rect.Left, rect.Top);
			this.ScaleTo(new Vector2f(rect.Width - rect.Left, rect.Height - rect.Top));
		}

		public void ScaleBy(Vector2f scale) {
			this.sprite.Scale += scale;
		}

		public void ScaleBy(float scale) {
			this.sprite.Scale = this.sprite.Scale.Multiply(new Vector2f(scale, scale));
		}

		public void ScaleBy(FloatRect rect) {
			FloatRect currRect = this.sprite.GetGlobalBounds();
			currRect.Left += rect.Left;
			currRect.Width += rect.Width;
			currRect.Top += rect.Top;
			currRect.Height += rect.Height;
			this.ScaleTo(currRect);
		}

		public void RotateTo(float degrees) {
			this.sprite.Rotation = degrees;
		}

		public void RotateBy(float degrees) {
			this.sprite.Rotation += degrees;
		}

		public Vector2f GetTextureSize() {
			return (Vector2f) this.sprite.Texture.Size;
		}

		public Vector2f GetTextureRectSize() {
			return new Vector2f(this.sprite.TextureRect.Width, this.sprite.TextureRect.Height);
		}

		public Texture GetTexture() {
			return this.sprite.Texture;
		}

		public void SetOrigin(Origins origin) {
			this.sprite.Origin = origin switch {
				Origins.TOPLEFT => new Vector2f(0, 0),
				Origins.MIDDLE => new Vector2f(this.GetTextureRectSize().X / 2f, this.GetTextureRectSize().Y / 2f),
				_ => this.sprite.Origin
			};
		}

		public bool Collides(Vector2f point) {
			return this.sprite.GetGlobalBounds().Contains(point.X, point.Y);
		}

		public bool Collides(FloatRect rect) {
			return this.sprite.GetGlobalBounds().Intersects(rect);
		}

		public bool Collides(Shape rectangleShape) {
			return this.sprite.GetGlobalBounds().Intersects(rectangleShape.GetGlobalBounds());
		}

		public bool Collides(SpriteE sprite) {
			return this.sprite.GetGlobalBounds().Intersects(sprite.sprite.GetGlobalBounds());
		}
	}

	public enum Origins {
		TOPLEFT,
		MIDDLE
	}
}