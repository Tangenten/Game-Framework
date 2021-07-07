using System.Collections.Generic;
using System.IO;
using Helpers;
using SFML.Graphics;

namespace TangentEngine {
	public class SpriteSheet {
		public Texture texture;
		private Dictionary<string, IntRect> spriteSheet;
		private List<string> spriteNames;

		public SpriteSheet(Stream textureStream, Stream sheetStream) {
			this.texture = new Texture(textureStream);
			this.texture.GenerateMipmap();

			this.spriteSheet = new Dictionary<string, IntRect>();
			this.spriteNames = new List<string>();

			using (StreamReader streamReader = new StreamReader(sheetStream)) {
				string? line;
				while ((line = streamReader.ReadLine()) != null) {
					string[] splitString = line.Split(',');

					string spriteName = splitString[0];
					IntRect spriteTextureCoords = new IntRect(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]));
					this.spriteSheet[spriteName] = spriteTextureCoords;
					this.spriteNames.Add(spriteName);
				}
			}
		}

		public SpriteSheet(Texture texture, Stream sheetStream) {
			this.texture = texture;
			this.texture.GenerateMipmap();

			this.spriteSheet = new Dictionary<string, IntRect>();
			this.spriteNames = new List<string>();

			using (StreamReader streamReader = new StreamReader(sheetStream)) {
				string? line;
				while ((line = streamReader.ReadLine()) != null) {
					string[] splitString = line.Split(',');

					string spriteName = splitString[0];
					IntRect spriteTextureCoords = new IntRect(int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), int.Parse(splitString[4]));
					this.spriteSheet[spriteName] = spriteTextureCoords;
					this.spriteNames.Add(spriteName);
				}
			}
		}

		public IntRect GetTextureCoords(string spriteName) {
			return this.spriteSheet[spriteName];
		}

		public IntRect GetRandomTextureCoords() {
			return this.spriteSheet[this.spriteNames.GetRandomElement()];
		}
	}
}