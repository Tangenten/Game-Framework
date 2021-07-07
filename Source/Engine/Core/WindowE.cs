using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

namespace TangentEngine {
	public class WindowE {
		public ContextSettings contextSettings;
		public RenderWindow renderWindow;
		public GameWindow gameWindow;
		public Gui gui;

		private string windowTitle;

		public int windowHeight;
		public int windowWidth;
		public Vector2f windowSize;

		public int desktopWidth;
		public int desktopHeight;
		public Vector2f desktopSize;

		public WindowE(Vector2i windowPos, Vector2i windowSize) {
			Toolkit.Init();
			this.gameWindow = new GameWindow();

			this.desktopWidth = (int) VideoMode.DesktopMode.Width;
			this.desktopHeight = (int) VideoMode.DesktopMode.Height;
			this.desktopSize = new Vector2f(this.desktopWidth, this.desktopHeight);

			this.windowWidth = windowSize.X;
			this.windowHeight = windowSize.Y;
			this.windowSize = new Vector2f(this.windowWidth, this.windowHeight);

			this.windowTitle = "Engine";

			this.contextSettings = new ContextSettings();
			this.contextSettings.AntialiasingLevel = 8;

			this.renderWindow = new RenderWindow(new VideoMode((uint) this.windowWidth, (uint) this.windowHeight), this.windowTitle, Styles.Default, this.contextSettings);
			this.renderWindow.SetVerticalSyncEnabled(true);
			this.renderWindow.SetKeyRepeatEnabled(false);
			this.renderWindow.SetMouseCursor(new Cursor(Cursor.CursorType.Cross));
			this.renderWindow.SetActive(true);

			this.renderWindow.Position = windowPos;
			this.renderWindow.Size = new Vector2u((uint) this.windowWidth, (uint) this.windowHeight);

			this.renderWindow.Closed += this.Closed;
			this.renderWindow.Resized += this.Resized;

			Engine.assets.GetStreamAsync("HandsIcon.png", stream => {
				Image icon = new Image(stream);
				this.renderWindow.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);
			});

			this.gui = new Gui(this.renderWindow);

			GL.Enable(EnableCap.Texture2D);
		}

		public void SetSize(Vector2f size) {
			this.renderWindow.Size = (Vector2u) size;
			this.windowSize = new Vector2f(this.windowWidth, this.windowHeight);

			Engine.camera.SetAspectRatio();
		}

		public void SetPosition(Vector2f pos) {
			this.renderWindow.Position = (Vector2i) pos;
			Engine.camera.SetAspectRatio();
		}

		public void SetTitle(in string title) {
			this.windowTitle = title;
			this.renderWindow.SetTitle(title);
		}

		public void SetIcon(Stream stream) {
			Image icon = new Image(stream);
			this.renderWindow.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);
		}

		public void GrabMouseCursor(bool state) {
			this.renderWindow.SetMouseCursorGrabbed(state);
		}

		public bool HasFocus() {
			return this.renderWindow.HasFocus();
		}

		public bool IsOpen() {
			return this.renderWindow.IsOpen;
		}

		public void Close() {
			this.renderWindow.Close();
		}

		public void Update() {
			this.DispatchEvents();
		}

		private void DispatchEvents() {
			this.renderWindow.DispatchEvents();
		}

		private void Closed(object? sender, EventArgs e) {
			this.renderWindow.Close();
		}

		private void Resized(object? sender, SizeEventArgs e) {
			this.windowWidth = (int) e.Width;
			this.windowHeight = (int) e.Height;
			this.windowSize = new Vector2f(this.windowWidth, this.windowHeight);

			Engine.camera.SetAspectRatio();
		}
	}
}