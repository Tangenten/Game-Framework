using SFML.Graphics;
using SFML.System;

namespace TangentEngine {
	public class TexSprite : SpriteE {
		public Texture texture;

		public TexSprite() { }

		public TexSprite(Texture texture) : this() {
			this.textureWidth = (int) texture.Size.X;
			this.textureHeight = (int) texture.Size.Y;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.texture = texture;

			this.sprite = new Sprite(this.texture);
			this.sprite.Texture.GenerateMipmap();
		}

		public TexSprite(Texture texture, IntRect texCoords) : this() {
			this.textureWidth = (int) texture.Size.X;
			this.textureHeight = (int) texture.Size.Y;
			this.textureSize = new Vector2f(this.textureWidth, this.textureHeight);

			this.texture = texture;

			this.sprite = new Sprite(this.texture);
			this.sprite.Texture.GenerateMipmap();
			this.sprite.TextureRect = texCoords;
		}

		public bool Collides(TexSprite texSprite) {
			return this.sprite.GetGlobalBounds().Intersects(texSprite.sprite.GetGlobalBounds());
		}

		public bool Collides(RenTexSprite renTexSprite) {
			return this.sprite.GetGlobalBounds().Intersects(renTexSprite.sprite.GetGlobalBounds());
		}

		public override void Process(ref RenTexSprite renTexSprite) {
			renTexSprite.Draw(this);
		}
	}
}