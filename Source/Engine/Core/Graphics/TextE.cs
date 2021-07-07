using System.IO;
using Helpers;
using SFML.Graphics;
using SFML.System;

namespace TangentEngine {
	public class TextE : GraphicProvider {
		private Text text;
		private Font font;

		public TextE(Stream fontFileStream) {
			this.font = new Font(fontFileStream);
			this.text = new Text("", this.font);
		}

		public void SetDisplayedString(in string text) {
			this.text.DisplayedString = text;
		}

		public void AddDisplayedString(in string text) {
			this.text.DisplayedString += text;
		}

		public string GetDisplayedString() {
			return this.text.DisplayedString;
		}

		public void SetDisplayedStringSize(int size) {
			this.text.CharacterSize = (uint) size;
		}

		public int GetDisplayedStringSize(int size) {
			return (int) this.text.CharacterSize;
		}

		public void SetDisplayedStringFillColor(Color color) {
			this.text.FillColor = color;
		}

		public Color GetDisplayedStringFillColor() {
			return this.text.FillColor;
		}

		public void SetDisplayedStringOutlineColor(Color color) {
			this.text.OutlineColor = color;
		}

		public void SetDisplayedStringOutlineThickness(int width) {
			this.text.OutlineThickness = width;
		}

		public Color GetDisplayedStringOutlineColor() {
			return this.text.OutlineColor;
		}

		public Vector2f GetPosition() {
			return this.text.Position;
		}

		public Vector2f Size() {
			return new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height);
		}

		public void MoveTo(Vector2f position) {
			this.text.Position = position;
		}

		public void MoveBy(Vector2f position) {
			this.text.Position += position;
		}

		public void ScaleTo(Vector2f size) {
			this.text.Scale = new Vector2f(size.X / this.text.GetLocalBounds().Width, size.Y / this.text.GetLocalBounds().Height);
		}

		public void ScaleTo(FloatRect rect) {
			this.text.Origin = new Vector2f(0f, 0f);
			this.text.Position = new Vector2f(rect.Left, rect.Top);
			this.ScaleTo(new Vector2f(rect.Width - rect.Left, rect.Height - rect.Top));
		}

		public void ScaleBy(Vector2f scale) {
			this.text.Scale += scale;
		}

		public void ScaleBy(float scale) {
			this.text.Scale = this.text.Scale.Multiply(new Vector2f(scale, scale));
		}

		public void ScaleBy(FloatRect rect) {
			FloatRect currRect = this.text.GetGlobalBounds();
			currRect.Left += rect.Left;
			currRect.Width += rect.Width;
			currRect.Top += rect.Top;
			currRect.Height += rect.Height;
			this.ScaleTo(currRect);
		}

		public void RotateTo(float degrees) {
			this.text.Rotation = degrees;
		}

		public void RotateBy(float degrees) {
			this.text.Rotation += degrees;
		}

		public void SetOrigin(Origins origin) {
			this.text.Origin = origin switch {
				Origins.TOPLEFT => new Vector2f(0, 0),
				Origins.MIDDLE => new Vector2f(this.text.GetLocalBounds().Width / 2f, this.text.GetLocalBounds().Height / 2f)
			};
		}

		public bool Collides(Vector2f point) {
			return this.text.GetGlobalBounds().Contains(point.X, point.Y);
		}

		public bool Collides(FloatRect rect) {
			return this.text.GetGlobalBounds().Intersects(rect);
		}

		public bool Collides(Shape rectangleShape) {
			return this.text.GetGlobalBounds().Intersects(rectangleShape.GetGlobalBounds());
		}

		public bool Collides(SpriteE sprite) {
			return this.text.GetGlobalBounds().Intersects(sprite.sprite.GetGlobalBounds());
		}

		public bool Collides(TextE text) {
			return this.text.GetGlobalBounds().Intersects(text.text.GetGlobalBounds());
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			renTexSprite.Draw(this.text);
		}
	}
}