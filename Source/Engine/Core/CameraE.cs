using SFML.Graphics;
using SFML.System;

namespace TangentEngine {
	public class CameraE {
		public int gameHeight;
		public int gameWidth;
		public Vector2f gameSize;

		private RenderWindow renderWindow;
		public View view;

		private bool debugAlterView;
		private Vector2f prevMousePosition;

		public CameraE(RenderWindow renderWindow) {
			this.gameWidth = 3840;
			this.gameHeight = 2160;
			this.gameSize = new Vector2f(this.gameWidth, this.gameHeight);

			this.renderWindow = renderWindow;
			this.view = new View(new FloatRect(0f, 0f, this.gameWidth, this.gameHeight));

			this.renderWindow.SetView(this.view);
		}

		public void Update() {
			this.AlterView();
		}

		[ConsoleCommand("DEBUG_CAMERA")]
		public void SetDebugAlterView(bool value) {
			this.debugAlterView = value;
		}

		[ConsoleCommand("CAMERA_POS")]
		public Vector2f CameraPosition() {
			return this.view.Center;
		}

		[ConsoleCommand("CAMERA_SIZE")]
		public Vector2f CameraSize() {
			return this.view.Size;
		}

		[ConsoleCommand("CAMERA_BOUNDARIES")]
		public FloatRect CameraBoundaries() {
			FloatRect floatRect = new FloatRect();
			floatRect.Left = this.renderWindow.MapPixelToCoords(new Vector2i(0, 0)).X;
			floatRect.Top = this.renderWindow.MapPixelToCoords(new Vector2i(0, 0)).Y;
			floatRect.Width = this.renderWindow.MapPixelToCoords(new Vector2i(Engine.window.windowWidth, Engine.window.windowHeight)).X;
			floatRect.Height = this.renderWindow.MapPixelToCoords(new Vector2i(Engine.window.windowWidth, Engine.window.windowHeight)).Y;
			return floatRect;
		}

		[ConsoleCommand("RESET_CAMERA")]
		public void ResetView() {
			this.view = new View(new FloatRect(0f, 0f, this.gameWidth, this.gameHeight));
			this.renderWindow.SetView(this.view);
		}

		public void StretchBy(Vector2f stretch) {
			this.view = new View(new FloatRect(0f, 0f, this.gameWidth * stretch.X, this.gameHeight * stretch.Y));
			this.renderWindow.SetView(this.view);
		}

		public void StretchTo(Vector2f coordSize) {
			this.view = new View(new FloatRect(0f, 0f, coordSize.X, coordSize.Y));
			this.renderWindow.SetView(this.view);
		}

		public void ZoomBy(float zoomBy) {
			this.view.Zoom(1f + -zoomBy);
			this.renderWindow.SetView(this.view);
		}

		public void ZoomTo(float zoomTo) {
			this.view.Zoom(1f / (this.view.Size.X / this.gameWidth) * zoomTo);
			this.renderWindow.SetView(this.view);
		}

		public void ZoomAt(Vector2f coords, float zoom) {
			Vector2f pixel = this.MapCoordsToPixel(coords);
			Vector2f beforeCoord = this.MapPixelToCoords(pixel);

			this.view.Zoom(1f + zoom);
			this.renderWindow.SetView(this.view);

			Vector2f afterCoord = this.MapPixelToCoords(pixel);
			Vector2f offsetCoords = beforeCoord - afterCoord;
			this.view.Move(offsetCoords);
			this.renderWindow.SetView(this.view);
		}

		public void RotateBy(float rotate) {
			this.view.Rotate(this.view.Rotation + (rotate % 360f - this.view.Rotation) % 360f);
			this.renderWindow.SetView(this.view);
		}

		public void RotateTo(float rotate) {
			this.view.Rotate((rotate % 360f - this.view.Rotation) % 360f);
			this.renderWindow.SetView(this.view);
		}

		public void RotateByAt(Vector2f coords, float rotate) {
			Vector2f pixel = this.MapCoordsToPixel(coords);
			Vector2f beforeCoord = this.MapPixelToCoords(pixel);

			this.view.Rotate(this.view.Rotation + (rotate % 360f - this.view.Rotation) % 360f);
			this.renderWindow.SetView(this.view);

			Vector2f afterCoord = this.MapPixelToCoords(pixel);
			Vector2f offsetCoords = beforeCoord - afterCoord;
			this.view.Move(offsetCoords);
			this.renderWindow.SetView(this.view);
		}

		public void MoveViewBy(Vector2f moveBy) {
			this.view.Move(moveBy);
			this.renderWindow.SetView(this.view);
		}

		public void MoveViewTo(Vector2f coords) {
			this.renderWindow.Position = (Vector2i) coords;
			this.renderWindow.SetView(this.view);
		}

		public Vector2f MapPixelToCoords(Vector2f pixel) {
			return this.renderWindow.MapPixelToCoords((Vector2i) pixel);
		}

		public Vector2f MapCoordsToPixel(Vector2f coords) {
			return (Vector2f) this.renderWindow.MapCoordsToPixel(coords);
		}

		// Adds Black Bars to force 16-9 aspect ratio
		public void SetAspectRatio() {
			float windowRatio = Engine.window.windowWidth / (float) Engine.window.windowHeight;
			float viewRatio = this.gameWidth / (float) this.gameHeight;
			float sizeX = 1f;
			float sizeY = 1f;
			float posX = 0f;
			float posY = 0f;

			bool horizontalSpacing = !(windowRatio < viewRatio);

			if (horizontalSpacing) {
				sizeX = viewRatio / windowRatio;
				posX = (1f - sizeX) / 2f;
			} else {
				sizeY = windowRatio / viewRatio;
				posY = (1f - sizeY) / 2f;
			}

			this.view.Viewport = new FloatRect(posX, posY, sizeX, sizeY);
			this.renderWindow.SetView(this.view);
		}

		private void AlterView() {
			if (this.debugAlterView) {
				if (Engine.input.mouseWheelVerticalMoved) {
					if (Engine.input.mouseWheelVerticalTicks > 0)
						this.ZoomAt(Engine.input.GetMouseGamePosition(), -0.05f);
					else
						this.ZoomAt(Engine.input.GetMouseGamePosition(), 0.05f);
				}

				if (Engine.input.Mouse((MouseButton.Middle, ButtonState.DOWN))) {
					this.prevMousePosition = Engine.input.GetMouseGamePosition();
				} else if (Engine.input.Mouse((MouseButton.Middle, ButtonState.HELD))) {
					this.MoveViewBy(-(Engine.input.GetMouseGamePosition() - this.prevMousePosition));
					this.prevMousePosition = Engine.input.GetMouseGamePosition();
				}

				if (Engine.input.Mouse((MouseButton.Right, ButtonState.DOWN))) {
					this.prevMousePosition = Engine.input.GetMouseWindowPosition();
				} else if (Engine.input.Mouse((MouseButton.Right, ButtonState.HELD))) {
					this.RotateByAt(Engine.input.GetMouseGamePosition(), Engine.input.GetMouseWindowPosition().Y - this.prevMousePosition.Y);
					this.prevMousePosition = Engine.input.GetMouseWindowPosition();
				}
			}
		}

		public void Reset() {
			this.ResetView();
		}
	}
}