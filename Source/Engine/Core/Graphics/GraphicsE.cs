using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace TangentEngine {
	public class GraphicsE {
		private RenderWindow renderWindow;
		private Dictionary<string, Texture> textureCache;

		public int resolutionWidth;
		public int resolutionHeight;
		public Vector2f resolutionSize;

		public GraphicsMixer mixer;
		private Color clearColor;

		public GraphicsE(in RenderWindow renderWindow) {
			this.renderWindow = renderWindow;

			this.resolutionWidth = 1920;
			this.resolutionHeight = 1080;
			this.resolutionSize = new Vector2f(this.resolutionWidth, this.resolutionHeight);

			this.clearColor = Color.Black;

			this.textureCache = new Dictionary<string, Texture>();

			this.mixer = new GraphicsMixer();
		}

		public void Update() {
			this.mixer.Update(ref this.renderWindow);
		}

		public Texture GetTexture(string resourceString) {
			lock (this.textureCache) {
				if (!this.textureCache.ContainsKey(resourceString)) {
					Stream stream = Engine.assets.GetStream(resourceString);
					Texture texture = new Texture(stream);
					this.textureCache[resourceString] = texture;
				}
			}

			return this.textureCache[resourceString];
		}

		public void GetTextureAsync(string resourceString, Action<Texture> onDoneLoading) {
			Task.Run(() => {
				lock (this.textureCache) {
					if (!this.textureCache.ContainsKey(resourceString)) {
						Stream stream = Engine.assets.GetStream(resourceString);
						Texture texture = new Texture(stream);
						this.textureCache[resourceString] = texture;
					}
				}

				onDoneLoading.Invoke(this.textureCache[resourceString]);
			});
		}

		[ConsoleCommand("SET_VERTICAL_SYNC")]
		public void SetVerticalSync(bool value) {
			this.renderWindow.SetVerticalSyncEnabled(value);
		}

		[ConsoleCommand("SET_FRAME_RATE")]
		public void SetFrameRateLimit(int frameRate) {
			this.renderWindow.SetFramerateLimit((uint) frameRate);
		}

		public void Clear() {
			this.renderWindow.Clear(this.clearColor);
		}

		public void SetClearColor(Color color) {
			this.clearColor = color;
		}

		public void Display() {
			this.renderWindow.Display();
		}

		public void SetMouseCursor(Cursor cursor) {
			this.renderWindow.SetMouseCursor(cursor);
		}

		public void Reset() {
			this.textureCache.Clear();
			this.mixer.Reset();
		}

		public void Draw(in Drawable drawable) {
			this.renderWindow.Draw(drawable);
		}

		public void Draw(in Drawable drawable, in RenderStates renderStates) {
			this.renderWindow.Draw(drawable, renderStates);
		}

		public void Draw(Vertex[] vertices, PrimitiveType type) {
			this.renderWindow.Draw(vertices, type);
		}

		public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states) {
			this.renderWindow.Draw(vertices, type, states);
		}

		public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type) {
			this.renderWindow.Draw(vertices, start, count, type);
		}

		public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type, RenderStates states) {
			this.renderWindow.Draw(vertices, start, count, type, states);
		}

		public void Draw(RenderTexture renderTexture, Vector2f scaleToSize, Vector2f position = new Vector2f(), Vector2f origin = new Vector2f(), float rotation = 0f) {
			renderTexture.Display();

			Sprite sprite = new Sprite(renderTexture.Texture);
			sprite.Origin = origin;
			sprite.Position = position;
			sprite.Rotation = rotation;
			sprite.Scale = new Vector2f(scaleToSize.X / sprite.Texture.Size.X, scaleToSize.Y / sprite.Texture.Size.Y);

			this.Draw(sprite);
		}

		public void Draw(RenderTexture renderTexture, RenderStates renderStates, Vector2f scaleToSize, Vector2f position = new Vector2f(), Vector2f origin = new Vector2f(), float rotation = 0f) {
			renderTexture.Display();

			Sprite sprite = new Sprite(renderTexture.Texture);
			sprite.Origin = origin;
			sprite.Position = position;
			sprite.Rotation = rotation;
			sprite.Scale = new Vector2f(scaleToSize.X / sprite.Texture.Size.X, scaleToSize.Y / sprite.Texture.Size.Y);

			this.Draw(sprite, renderStates);
		}

		public void Draw(RenTexSprite renTexSprite) {
			this.renderWindow.Draw(renTexSprite.sprite, new RenderStates(renTexSprite.blendMode, renTexSprite.transform, null, renTexSprite.shader));
		}

		public void Draw(TexSprite texSprite) {
			this.renderWindow.Draw(texSprite.sprite, new RenderStates(texSprite.blendMode, texSprite.transform, null, texSprite.shader));
		}
	}

	public class GraphicsMixer {
		private List<GraphicsTrack> graphicsTracksAfter;

		private List<GraphicsTrack> graphicsTracks;
		private List<GraphicEffector> graphicEffectors;

		public RenTexSprite masterSprite;

		public GraphicsMixer() {
			this.masterSprite = new RenTexSprite(Engine.window.windowWidth, Engine.window.windowHeight, Engine.window.contextSettings);
			this.masterSprite.SetViewToCamera();
			this.masterSprite.ScaleToCamera();

			this.graphicsTracks = new List<GraphicsTrack>();
			this.graphicsTracksAfter = new List<GraphicsTrack>();
			this.graphicEffectors = new List<GraphicEffector>();
		}

		public void Update(ref RenderWindow renderWindow) {
			this.masterSprite.Clear(Color.Transparent);
			this.masterSprite.SetViewToCamera();
			this.masterSprite.ScaleToCamera();

			for (int i = 0; i < this.graphicsTracks.Count; i++) {
				if (this.graphicsTracks[i].active) {
					this.graphicsTracks[i].Process(ref this.masterSprite);
					this.masterSprite.Display();
				}
			}

			for (int i = 0; i < this.graphicEffectors.Count; i++) {
				if (this.graphicEffectors[i].active) {
					this.graphicEffectors[i].Process(ref this.masterSprite);
					this.masterSprite.Display();
				}
			}

			for (int i = 0; i < this.graphicsTracksAfter.Count; i++) {
				if (this.graphicsTracksAfter[i].active) {
					this.graphicsTracksAfter[i].Process(ref this.masterSprite);
					this.masterSprite.Display();
				}
			}

			renderWindow.Draw(this.masterSprite.sprite, new RenderStates(BlendMode.Alpha));
		}

		public void AddGraphicsTrack(GraphicsTrack graphicsTrack) {
			this.graphicsTracks.Add(graphicsTrack);
			this.graphicsTracks.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
		}

		public void AddGraphicsTrackAfter(GraphicsTrack graphicsTrack) {
			this.graphicsTracksAfter.Add(graphicsTrack);
			this.graphicsTracksAfter.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
		}

		public void AddGraphicsEffector(GraphicEffector graphicEffector) {
			this.graphicEffectors.Add(graphicEffector);
			this.graphicEffectors.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
		}

		public void Reset() {
			this.graphicsTracks.Clear();
			this.graphicEffectors.Clear();
		}
	}

	public class GraphicsTrack {
		public bool active;
		public string name;
		public int zOrder;
		public int downsize;

		private RenTexSprite trackSprite;

		private List<GraphicProvider> graphicProviders;
		private List<GraphicEffector> graphicEffectors;

		public GraphicsTrack(string name = "", int zOrder = -1) {
			this.trackSprite = new RenTexSprite(Engine.window.windowWidth, Engine.window.windowHeight, Engine.window.contextSettings);
			this.trackSprite.SetViewToCamera();
			this.trackSprite.ScaleToCamera();

			this.graphicProviders = new List<GraphicProvider>();
			this.graphicEffectors = new List<GraphicEffector>();

			this.active = true;
			this.name = name;
			this.zOrder = zOrder;
		}

		public void Process(ref RenTexSprite renTexSprite) {
			this.trackSprite.Clear(Color.Transparent);
			this.trackSprite.SetViewToCamera();
			this.trackSprite.ScaleToCamera();

			for (int i = 0; i < this.graphicProviders.Count; i++) {
				if (this.graphicProviders[i].active) {
					this.graphicProviders[i].Process(ref this.trackSprite);
				}
			}

			this.trackSprite.Display();

			for (int i = 0; i < this.graphicEffectors.Count; i++) {
				if (this.graphicEffectors[i].active) {
					this.graphicEffectors[i].Process(ref this.trackSprite);
					this.trackSprite.Display();
				}
			}

			renTexSprite.Draw(this.trackSprite);
		}

		public void AddGraphicsProvider(GraphicProvider graphicProvider) {
			this.graphicProviders.Add(graphicProvider);
			this.graphicProviders.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
		}

		public void AddGraphicsEffector(GraphicEffector graphicEffector) {
			this.graphicEffectors.Add(graphicEffector);
			this.graphicEffectors.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
		}

		public void RemoveGraphicsProvider(GraphicProvider graphicProvider) {
			this.graphicProviders.Remove(graphicProvider);
		}

		public void RemoveGraphicsEffector(GraphicEffector graphicEffector) {
			this.graphicEffectors.Remove(graphicEffector);
		}
	}

	public abstract class GraphicProvider {
		public bool active;
		public int zOrder;

		public GraphicProvider(int zOrder = -1) {
			this.active = true;
			this.zOrder = zOrder;
		}

		public abstract void Process(ref RenTexSprite renTexSprite);
	}

	public abstract class GraphicEffector {
		public bool active;
		public int zOrder;
		public int downSize;

		public GraphicEffector(int zOrder = -1, int downSize = 1) {
			this.active = true;
			this.zOrder = zOrder;
			this.downSize = downSize;
		}

		public abstract void Process(ref RenTexSprite renTexSprite);
	}
}