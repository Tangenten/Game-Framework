using Helpers;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace TangentEngine {
	public class RenTexSprite : SpriteE {
		public RenderTexture renderTexture;

		public RenTexSprite() { }

		public RenTexSprite(int width, int height) : this() {
			this.textureWidth = width;
			this.textureHeight = height;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.renderTexture = new RenderTexture((uint) this.textureWidth, (uint) this.textureHeight);

			this.sprite = new Sprite(this.renderTexture.Texture);
			this.sprite.Position = new Vector2f(0f, 0f);
		}

		public RenTexSprite(int width, int height, ContextSettings contextSettings) {
			this.textureWidth = width;
			this.textureHeight = height;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.renderTexture = new RenderTexture((uint) this.textureWidth, (uint) this.textureHeight, contextSettings);

			this.sprite = new Sprite(this.renderTexture.Texture);
			this.sprite.Position = new Vector2f(0f, 0f);
		}

		public RenTexSprite(Color[,] colors) : this() {
			Sprite s = new Sprite(new Texture(new Image(colors)));
			s.Texture.GenerateMipmap();

			this.textureWidth = (int) s.Texture.Size.X;
			this.textureHeight = (int) s.Texture.Size.Y;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.renderTexture = new RenderTexture((uint) this.textureWidth, (uint) this.textureHeight);

			this.renderTexture.Draw(s);
			this.renderTexture.Display();

			this.sprite = new Sprite(this.renderTexture.Texture);
		}

		public RenTexSprite(Texture texture) : this() {
			Sprite s = new Sprite(texture);
			s.Texture.GenerateMipmap();

			this.textureWidth = (int) texture.Size.X;
			this.textureHeight = (int) texture.Size.Y;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.renderTexture = new RenderTexture((uint) this.textureWidth, (uint) this.textureHeight);

			this.renderTexture.Draw(s);
			this.renderTexture.Display();

			this.sprite = new Sprite(this.renderTexture.Texture);
		}

		public RenTexSprite(Texture texture, IntRect texCoords) : this() {
			Sprite s = new Sprite(texture);
			s.Texture.GenerateMipmap();
			s.TextureRect = texCoords;

			this.textureWidth = texCoords.Width;
			this.textureHeight = texCoords.Height;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.renderTexture = new RenderTexture((uint) this.textureWidth, (uint) this.textureHeight);

			this.renderTexture.Draw(s);
			this.renderTexture.Display();

			this.sprite = new Sprite(this.renderTexture.Texture);
		}

		public void Clear() {
			this.renderTexture.Clear(new Color(0, 0, 0, 0));
		}

		public void Clear(Color color) {
			this.renderTexture.Clear(color);
		}

		public void SetSmooth(bool smooth) {
			this.renderTexture.Texture.Smooth = true;
		}

		public bool Collides(TexSprite texSprite) {
			return this.sprite.GetGlobalBounds().Intersects(texSprite.sprite.GetGlobalBounds());
		}

		public bool Collides(RenTexSprite renTexSprite) {
			return this.sprite.GetGlobalBounds().Intersects(renTexSprite.sprite.GetGlobalBounds());
		}

		public void ScaleToCamera() {
			this.SetOrigin(Origins.MIDDLE);
			this.MoveTo(Engine.camera.CameraPosition());
			this.ScaleTo(Engine.camera.CameraSize());
		}

		public void SetViewToCamera() {
			this.renderTexture.SetView(Engine.camera.view);
		}

		public void ResetView() {
			this.renderTexture.SetView(this.renderTexture.DefaultView);
		}

		public void DrawOntoTextureUV(Vector2f uvPos, Vector2f uvSize, Color color, Origins origin = Origins.TOPLEFT) {
			RectangleShape rectangleShape = new RectangleShape();
			rectangleShape.Position = uvPos.Multiply(this.GetTextureSize());
			rectangleShape.Size = uvSize.Multiply(this.GetTextureSize());
			rectangleShape.FillColor = color;

			switch (origin) {
				case Origins.TOPLEFT:
					break;
				case Origins.MIDDLE:
					rectangleShape.Origin = rectangleShape.Size / 2f;
					break;
			}

			this.renderTexture.Draw(rectangleShape);
		}

		public void DrawOntoTextureUV(Vector2f uvPos, Vector2f uvSize, RenTexSprite sprite, Origins origin = Origins.TOPLEFT) {
			RectangleShape rectangleShape = new RectangleShape();
			rectangleShape.Position = uvPos.Multiply(this.GetTextureSize());
			rectangleShape.Size = uvSize.Multiply(this.GetTextureSize());
			rectangleShape.Texture = sprite.renderTexture.Texture;

			switch (origin) {
				case Origins.TOPLEFT:
					break;
				case Origins.MIDDLE:
					rectangleShape.Origin = rectangleShape.Size / 2f;
					break;
			}

			this.renderTexture.Draw(rectangleShape);
		}

		public void DrawOntoTextureUV(Vector2f uvPos, Vector2f uvSize, RenTexSprite sprite, RenderStates renderStates, Origins origin = Origins.TOPLEFT) {
			RectangleShape rectangleShape = new RectangleShape();
			rectangleShape.Position = uvPos.Multiply(this.GetTextureSize());
			rectangleShape.Size = uvSize.Multiply(this.GetTextureSize());
			rectangleShape.Texture = sprite.renderTexture.Texture;

			switch (origin) {
				case Origins.TOPLEFT:
					break;
				case Origins.MIDDLE:
					rectangleShape.Origin = rectangleShape.Size / 2f;
					break;
			}

			this.renderTexture.Draw(rectangleShape, renderStates);
		}

		public void DrawOntoTextureCoords(Vector2f coordPos, Vector2f coordSize, Color color, Origins origin = Origins.TOPLEFT) {
			Vector2f texTopLeft = this.sprite.InverseTransform.TransformPoint(coordPos);
			Vector2f texSize = coordSize.Divide(Engine.camera.gameSize.Divide(this.GetTextureSize()));

			RectangleShape rectangleShape = new RectangleShape();
			rectangleShape.Position = texTopLeft;
			rectangleShape.Size = texSize;
			rectangleShape.Rotation = this.sprite.Rotation;
			rectangleShape.FillColor = color;

			switch (origin) {
				case Origins.TOPLEFT:
					break;
				case Origins.MIDDLE:
					rectangleShape.Origin = rectangleShape.Size / 2f;
					break;
			}

			this.renderTexture.Draw(rectangleShape);
		}

		public void Draw(Drawable drawable) {
			this.renderTexture.Draw(drawable);
		}

		public void Draw(Drawable drawable, RenderStates renderStates) {
			this.renderTexture.Draw(drawable, renderStates);
		}

		public void Draw(in TexSprite texSprite) {
			this.renderTexture.Draw(texSprite.sprite, new RenderStates(texSprite.blendMode, Transform.Identity, null, texSprite.shader));
		}

		public void Draw(RenTexSprite renTexSprite) {
			this.renderTexture.Draw(renTexSprite.sprite, new RenderStates(renTexSprite.blendMode, Transform.Identity, null, renTexSprite.shader));
		}

		public void Draw(RenTexSprite renTexSprite, RenderStates renderStates) {
			this.renderTexture.Draw(renTexSprite.sprite, renderStates);
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			this.renderTexture.Display();
			renTexSprite.Display();
			renTexSprite.Draw(this);
		}

		public void Display() {
			this.renderTexture.Display();
		}
	}
}