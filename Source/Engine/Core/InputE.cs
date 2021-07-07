using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helpers;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace TangentEngine {
	public class InputE {
		private Dictionary<KeyButton, bool> keyDownDict;
		private Dictionary<KeyButton, bool> keyHeldDict;
		private Dictionary<KeyButton, bool> keyUpDict;

		private Dictionary<MouseButton, bool> mouseDownDict;
		private Dictionary<MouseButton, bool> mouseHeldDict;
		private Dictionary<MouseButton, bool> mouseUpDict;

		private Dictionary<string, (KeyButton, ButtonState)[]> keyBindingDict;
		private Dictionary<string, (MouseButton, ButtonState)[]> mouseBindingDict;

		private Vector2f mouseCurrPos;
		private Vector2f mouseDir;
		private Vector2f mouseDirNormal;
		private Vector2f mouseDownPos;
		private Vector2f mouseScrollPos;
		private Vector2f mouseUpPos;

		private bool mouseInsideWindow;
		private bool windowHasFocus;

		private List<string> contextQueue;
		private string currentContext;

		public bool mouseWheelHorizontalMoved;
		public bool mouseWheelVerticalMoved;
		public int mouseWheelHorizontalTicks;
		public int mouseWheelVerticalTicks;

		public List<string> textInputList;
		private Regex validInput;

		public InputE(RenderWindow renderWindow) {
			this.keyDownDict = new Dictionary<KeyButton, bool>();
			this.keyHeldDict = new Dictionary<KeyButton, bool>();
			this.keyUpDict = new Dictionary<KeyButton, bool>();

			this.mouseDownDict = new Dictionary<MouseButton, bool>();
			this.mouseHeldDict = new Dictionary<MouseButton, bool>();
			this.mouseUpDict = new Dictionary<MouseButton, bool>();

			this.keyBindingDict = new Dictionary<string, (KeyButton, ButtonState)[]>();
			this.mouseBindingDict = new Dictionary<string, (MouseButton, ButtonState)[]>();

			this.textInputList = new List<string>();

			this.validInput = new Regex("^[a-zA-Z0-9`!@#$%^&*()_+|\\-=\\\\{}\\[\\]:\"\";'<>?,./ ]*$");

			this.contextQueue = new List<string>();
			this.currentContext = "";

			renderWindow.MouseWheelScrolled += this.MouseWheelScrolled;
			renderWindow.MouseButtonReleased += this.MouseButtonReleased;
			renderWindow.MouseButtonPressed += this.MouseButtonPressed;
			renderWindow.MouseMoved += this.MouseMoved;
			renderWindow.MouseLeft += this.MouseLeft;
			renderWindow.MouseEntered += this.MouseEntered;

			renderWindow.KeyReleased += this.KeyReleased;
			renderWindow.KeyPressed += this.KeyPressed;

			renderWindow.JoystickButtonPressed += this.JoystickButtonPressed;
			renderWindow.JoystickButtonReleased += this.JoystickButtonReleased;
			renderWindow.JoystickMoved += this.JoystickMoved;
			renderWindow.JoystickConnected += this.JoystickConnected;
			renderWindow.JoystickDisconnected += this.JoystickDisconnected;

			renderWindow.GainedFocus += this.GainedFocus;
			renderWindow.LostFocus += this.LostFocus;

			renderWindow.TextEntered += this.TextEntered;
		}

		public bool Key(params (KeyButton, ButtonState)[] keys) {
			if (keys.Length == 0) {
				return false;
			}

			foreach ((KeyButton button, ButtonState state) in keys) {
				switch (state) {
					case ButtonState.DOWN:
						if (this.keyDownDict.ContainsKey(button)) {
							if (!this.keyDownDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
					case ButtonState.HELD:
						if (this.keyHeldDict.ContainsKey(button)) {
							if (!this.keyHeldDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
					case ButtonState.UP:
						if (this.keyUpDict.ContainsKey(button)) {
							if (!this.keyUpDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
				}
			}

			return true;
		}

		public bool Key(string context, params (KeyButton, ButtonState)[] keys) {
			return this.CompareContext(context) && this.Key(keys);
		}

		public bool Mouse(params (MouseButton, ButtonState)[] buttons) {
			if (buttons.Length == 0) {
				return false;
			}

			foreach ((MouseButton button, ButtonState state) in buttons) {
				switch (state) {
					case ButtonState.DOWN:
						if (this.mouseDownDict.ContainsKey(button)) {
							if (!this.mouseDownDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
					case ButtonState.HELD:
						if (this.mouseHeldDict.ContainsKey(button)) {
							if (!this.mouseHeldDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
					case ButtonState.UP:
						if (this.mouseUpDict.ContainsKey(button)) {
							if (!this.mouseUpDict[button]) {
								return false;
							}
						} else {
							return false;
						}

						break;
				}
			}

			return true;
		}

		public bool Mouse(in string context, params (MouseButton, ButtonState)[] buttons) {
			return this.CompareContext(context) && this.Mouse(buttons);
		}

		public void SetKeyBinding(in string binding, params (KeyButton, ButtonState)[] keys) {
			this.keyBindingDict[binding] = keys;
		}

		public bool GetKeyBinding(in string binding) {
			return this.Key(this.keyBindingDict[binding]);
		}

		public bool GetKeyBinding(in string context, in string binding) {
			return this.Key(context, this.keyBindingDict[binding]);
		}

		public void RemoveKeyBinding(in string binding) {
			this.keyBindingDict.Remove(binding);
		}

		public void ClearKeyBindings() {
			this.keyBindingDict.Clear();
		}

		public void SetMouseBinding(in string binding, params (MouseButton, ButtonState)[] keys) {
			this.mouseBindingDict[binding] = keys;
		}

		public bool GetMouseBinding(in string binding) {
			return this.Mouse(this.mouseBindingDict[binding]);
		}

		public bool GetMouseBinding(in string context, in string binding) {
			return this.Mouse(context, this.mouseBindingDict[binding]);
		}

		public void RemoveMouseBinding(in string binding) {
			this.mouseBindingDict.Remove(binding);
		}

		public void ClearMouseBindings() {
			this.mouseBindingDict.Clear();
		}

		public bool AnyKeyPressed() {
			return this.keyDownDict.Keys.Count == 0 && this.keyUpDict.Keys.Count == 0 && this.keyHeldDict.Keys.Count == 0;
		}

		public bool AnyMouseButtonPressed() {
			return this.mouseDownDict.Keys.Count == 0 && this.mouseUpDict.Keys.Count == 0 && this.mouseHeldDict.Keys.Count == 0;
		}

		public Vector2f GetMouseWindowPosition() {
			return this.mouseCurrPos;
		}

		public Vector2f GetMouseGamePosition() {
			return Engine.camera.MapPixelToCoords(this.mouseCurrPos);
		}

		public string GetClipboard() {
			return Clipboard.Contents;
		}

		public void SetClipboard(in string content) {
			Clipboard.Contents = content;
		}

		public string GetContext() {
			return this.currentContext;
		}

		public bool CompareContext(in string context) {
			return this.currentContext == context;
		}

		public void PushContextAfterSeconds(string context, float seconds) {
			Engine.coroutines.AddCoroutine(new Coroutine(Coroutine.WaitSeconds(seconds), () => { this.PushContext(context); }));
		}

		public void PopContextAfterSeconds(string context, float seconds) {
			Engine.coroutines.AddCoroutine(new Coroutine(Coroutine.WaitSeconds(seconds), () => { this.PopContext(context); }));
		}

		public void PushContextAfterAllButtonsReleased(string context) {
			Engine.coroutines.AddCoroutine(new Coroutine(
				Coroutine.AfterCondition(() => { return !this.AnyKeyPressed() && !this.AnyMouseButtonPressed(); }), () => { this.PushContext(context); }));
		}

		public void PopContextAfterAllButtonsReleased(string context) {
			Engine.coroutines.AddCoroutine(new Coroutine(
				Coroutine.AfterCondition(() => { return !this.AnyKeyPressed() && !this.AnyMouseButtonPressed(); }), () => { this.PopContext(context); }));
		}

		public void PushContext(in string context) {
			if (!this.contextQueue.Contains(context)) {
				this.currentContext = context;
				this.contextQueue.Add(context);
			}
		}

		public void PopContext(in string context) {
			this.contextQueue.Remove(context);
			if (this.contextQueue.Count > 0) {
				this.currentContext = this.contextQueue[^1];
			} else {
				this.currentContext = "";
			}
		}

		public void Update() {
			this.keyDownDict.Clear();
			this.keyUpDict.Clear();

			this.mouseDownDict.Clear();
			this.mouseUpDict.Clear();

			this.textInputList.Clear();

			this.mouseDir = new Vector2f(0f, 0f);

			this.mouseWheelHorizontalMoved = false;
			this.mouseWheelHorizontalTicks = 0;
			this.mouseWheelVerticalMoved = false;
			this.mouseWheelVerticalTicks = 0;
		}

		private void KeyPressed(object? sender, KeyEventArgs e) {
			this.keyDownDict[(KeyButton) e.Code] = true;
			this.keyHeldDict[(KeyButton) e.Code] = true;
		}

		private void KeyReleased(object? sender, KeyEventArgs e) {
			this.keyUpDict[(KeyButton) e.Code] = true;
			this.keyHeldDict[(KeyButton) e.Code] = false;
		}

		private void TextEntered(object? sender, TextEventArgs e) {
			if (this.validInput.IsMatch(e.Unicode)) this.textInputList.Add(e.Unicode);
		}

		private void MouseButtonPressed(object? sender, MouseButtonEventArgs e) {
			Vector2f v = new Vector2f(e.X, e.Y);

			this.mouseDownPos = v;
			this.mouseDownDict[(MouseButton) e.Button] = true;
			this.mouseHeldDict[(MouseButton) e.Button] = true;
		}

		private void MouseButtonReleased(object? sender, MouseButtonEventArgs e) {
			Vector2f v = new Vector2f(e.X, e.Y);

			this.mouseUpPos = v;
			this.mouseUpDict[(MouseButton) e.Button] = true;
			this.mouseHeldDict[(MouseButton) e.Button] = false;
		}

		private void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e) {
			Vector2f v = new Vector2f(e.X, e.Y);

			this.mouseScrollPos = v;
			if (e.Wheel == (Mouse.Wheel) MouseWheel.HorizontalWheel) {
				this.mouseWheelHorizontalMoved = true;
				this.mouseWheelHorizontalTicks = (int) e.Delta;
			} else {
				this.mouseWheelVerticalMoved = true;
				this.mouseWheelVerticalTicks = (int) e.Delta;
			}
		}

		private void MouseMoved(object? sender, MouseMoveEventArgs e) {
			Vector2f v = new Vector2f(e.X, e.Y);

			this.mouseDir = this.mouseCurrPos - v;
			this.mouseDirNormal = VectorH.NormalizeVector(this.mouseDir);
			this.mouseCurrPos = v;
		}

		private void JoystickDisconnected(object? sender, JoystickConnectEventArgs e) { }

		private void JoystickConnected(object? sender, JoystickConnectEventArgs e) { }

		private void JoystickMoved(object? sender, JoystickMoveEventArgs e) { }

		private void JoystickButtonReleased(object? sender, JoystickButtonEventArgs e) { }

		private void JoystickButtonPressed(object? sender, JoystickButtonEventArgs e) { }

		private void MouseLeft(object? sender, EventArgs e) {
			this.mouseInsideWindow = false;
		}

		private void MouseEntered(object? sender, EventArgs e) {
			this.mouseInsideWindow = true;
		}

		private void LostFocus(object? sender, EventArgs e) {
			this.windowHasFocus = false;
		}

		private void GainedFocus(object? sender, EventArgs e) {
			this.windowHasFocus = true;
		}

		public void Reset() {
			this.contextQueue.Clear();
			this.currentContext = "";
			this.keyBindingDict.Clear();
			this.mouseBindingDict.Clear();
		}
	}

	public enum ButtonState {
		DOWN,
		HELD,
		UP
	}

	public enum KeyButton {
		Unknown = -1, // 0xFFFFFFFF
		A = 0,
		B = 1,
		C = 2,
		D = 3,
		E = 4,
		F = 5,
		G = 6,
		H = 7,
		I = 8,
		J = 9,
		K = 10, // 0x0000000A
		L = 11, // 0x0000000B
		M = 12, // 0x0000000C
		N = 13, // 0x0000000D
		O = 14, // 0x0000000E
		P = 15, // 0x0000000F
		Q = 16, // 0x00000010
		R = 17, // 0x00000011
		S = 18, // 0x00000012
		T = 19, // 0x00000013
		U = 20, // 0x00000014
		V = 21, // 0x00000015
		W = 22, // 0x00000016
		X = 23, // 0x00000017
		Y = 24, // 0x00000018
		Z = 25, // 0x00000019
		Num0 = 26, // 0x0000001A
		Num1 = 27, // 0x0000001B
		Num2 = 28, // 0x0000001C
		Num3 = 29, // 0x0000001D
		Num4 = 30, // 0x0000001E
		Num5 = 31, // 0x0000001F
		Num6 = 32, // 0x00000020
		Num7 = 33, // 0x00000021
		Num8 = 34, // 0x00000022
		Num9 = 35, // 0x00000023
		Escape = 36, // 0x00000024
		LControl = 37, // 0x00000025
		LShift = 38, // 0x00000026
		LAlt = 39, // 0x00000027
		LSystem = 40, // 0x00000028
		RControl = 41, // 0x00000029
		RShift = 42, // 0x0000002A
		RAlt = 43, // 0x0000002B
		RSystem = 44, // 0x0000002C
		Menu = 45, // 0x0000002D
		LBracket = 46, // 0x0000002E
		RBracket = 47, // 0x0000002F

		[Obsolete("Deprecated: Use Semicolon instead.")]
		SemiColon = 48, // 0x00000030
		Semicolon = 48, // 0x00000030
		Comma = 49, // 0x00000031
		Period = 50, // 0x00000032
		Quote = 51, // 0x00000033
		Slash = 52, // 0x00000034

		[Obsolete("Deprecated: Use Backslash instead.")]
		BackSlash = 53, // 0x00000035
		Backslash = 53, // 0x00000035
		Tilde = 54, // 0x00000036
		Equal = 55, // 0x00000037

		[Obsolete("Deprecated: Use Hyphen instead.")]
		Dash = 56, // 0x00000038
		Hyphen = 56, // 0x00000038
		Space = 57, // 0x00000039
		Enter = 58, // 0x0000003A

		[Obsolete("Deprecated: Use Enter instead.")]
		Return = 58, // 0x0000003A

		[Obsolete("Deprecated: Use Backspace instead.")]
		BackSpace = 59, // 0x0000003B
		Backspace = 59, // 0x0000003B
		Tab = 60, // 0x0000003C
		PageUp = 61, // 0x0000003D
		PageDown = 62, // 0x0000003E
		End = 63, // 0x0000003F
		Home = 64, // 0x00000040
		Insert = 65, // 0x00000041
		Delete = 66, // 0x00000042
		Add = 67, // 0x00000043
		Subtract = 68, // 0x00000044
		Multiply = 69, // 0x00000045
		Divide = 70, // 0x00000046
		Left = 71, // 0x00000047
		Right = 72, // 0x00000048
		Up = 73, // 0x00000049
		Down = 74, // 0x0000004A
		Numpad0 = 75, // 0x0000004B
		Numpad1 = 76, // 0x0000004C
		Numpad2 = 77, // 0x0000004D
		Numpad3 = 78, // 0x0000004E
		Numpad4 = 79, // 0x0000004F
		Numpad5 = 80, // 0x00000050
		Numpad6 = 81, // 0x00000051
		Numpad7 = 82, // 0x00000052
		Numpad8 = 83, // 0x00000053
		Numpad9 = 84, // 0x00000054
		F1 = 85, // 0x00000055
		F2 = 86, // 0x00000056
		F3 = 87, // 0x00000057
		F4 = 88, // 0x00000058
		F5 = 89, // 0x00000059
		F6 = 90, // 0x0000005A
		F7 = 91, // 0x0000005B
		F8 = 92, // 0x0000005C
		F9 = 93, // 0x0000005D
		F10 = 94, // 0x0000005E
		F11 = 95, // 0x0000005F
		F12 = 96, // 0x00000060
		F13 = 97, // 0x00000061
		F14 = 98, // 0x00000062
		F15 = 99, // 0x00000063
		Pause = 100, // 0x00000064
		KeyCount = 101 // 0x00000065
	}

	public enum MouseButton {
		Left,
		Right,
		Middle,
		XButton1,
		XButton2,
		ButtonCount
	}

	public enum MouseWheel {
		VerticalWheel,
		HorizontalWheel
	}

	public enum JoystickAxis {
		X,
		Y,
		Z,
		R,
		U,
		V,
		PovX,
		PovY
	}
}