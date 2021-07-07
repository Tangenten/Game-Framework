using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Fasterflect;
using Helpers;
using SFML.Graphics;
using SFML.System;

namespace TangentEngine {
	public class ConsoleE {
		private Text consoleInputText;

		private Text consoleOutputText;
		private List<string> consoleOutputTextList;
		private int consoleOutputSelectedIndex;

		private float consoleSize;
		private RectangleShape consoleRect;
		private RectangleShape consoleTextRect;

		private RenTexSprite consoleRenTexSprite;
		private GraphicsTrack consoleGraphicsTrack;

		private bool showConsole;
		private double caretCounter;
		private float backspaceCounter;

		public string[] currentConsoleCommand;

		public ConsoleE() {
			this.consoleRenTexSprite = new RenTexSprite(Engine.graphics.resolutionWidth, Engine.graphics.resolutionHeight);
			this.consoleRenTexSprite.SetOrigin(Origins.MIDDLE);

			this.consoleGraphicsTrack = new GraphicsTrack("Console");
			this.consoleGraphicsTrack.AddGraphicsProvider(this.consoleRenTexSprite);
			this.consoleGraphicsTrack.active = false;
			Engine.graphics.mixer.AddGraphicsTrackAfter(this.consoleGraphicsTrack);

			this.consoleSize = this.consoleRenTexSprite.GetTextureSize().Y / 4f;
			this.consoleRect = new RectangleShape(new Vector2f(this.consoleRenTexSprite.GetTextureSize().X, this.consoleSize));
			this.consoleRect.FillColor = new Color(64, 64, 64, 127);

			this.consoleTextRect = new RectangleShape(new Vector2f(this.consoleRenTexSprite.GetTextureSize().X, 48));
			this.consoleTextRect.Position = new Vector2f(0f, this.consoleSize);
			this.consoleTextRect.FillColor = new Color(96, 96, 96, 127);

			this.consoleInputText = new Text();
			this.consoleInputText.Font = new Font(Engine.assets.GetStream("Engine.Source.Engine.Assets.Fonts.Quicksand.ttf"));
			this.consoleInputText.DisplayedString = "";
			this.consoleInputText.Position = new Vector2f(0f, this.consoleSize - 12);
			this.consoleInputText.FillColor = new Color(255, 255, 255, 255);
			this.consoleInputText.OutlineColor = new Color(255, 255, 255, 255);
			this.consoleInputText.CharacterSize = 48;

			this.consoleOutputText = new Text();
			this.consoleOutputText.Font = new Font(Engine.assets.GetStream("Engine.Source.Engine.Assets.Fonts.Quicksand.ttf"));
			this.consoleOutputText.DisplayedString = "";
			this.consoleOutputText.Position = new Vector2f(0f, 0);
			this.consoleOutputText.FillColor = new Color(255, 255, 255, 255);
			this.consoleOutputText.OutlineColor = new Color(255, 255, 255, 255);
			this.consoleOutputText.CharacterSize = 48;

			this.consoleOutputTextList = new List<string>(5);

			this.currentConsoleCommand = new[] {""};
		}

		public bool CheckConsoleCommand(in string opCode, int valCount, out List<string> values) {
			values = new List<string>();

			if (opCode.ToUpper() == this.currentConsoleCommand[0].ToUpper()) {
				if (this.currentConsoleCommand.Length == valCount + 1) {
					for (int i = 1; i < this.currentConsoleCommand.Length; i++) {
						values.Add(this.currentConsoleCommand[i]);
					}

					return true;
				}
			}

			return false;
		}

		private List<object> alreadyVisited = new List<object>(); // to avoid infinite recursion

		private void SearchAllObjectsForAttributes(object obj) {
			if (alreadyVisited.Contains(obj) || obj is Pointer || obj is null) return;
			alreadyVisited.Add(obj);

			foreach (MethodInfo methodInfo in obj.GetType().Methods()) {
				foreach (object customAttribute in methodInfo.GetCustomAttributes(true)) {
					if (customAttribute.GetType() == typeof(ConsoleCommandAttribute)) {
						if (((string) customAttribute.TryGetValue("command")).ToUpper() == this.currentConsoleCommand[0].ToUpper()) {
							object? returnVal = null;

							if (methodInfo.GetParameters().Length == 0) {
								returnVal = methodInfo.Invoke(obj, null);
							} else {
								IList<ParameterInfo>? parameters = methodInfo.Parameters();
								if (this.currentConsoleCommand.Length - 1 == parameters.Count) {
									try {
										object?[]? convertedParameters = new object?[parameters.Count];
										for (int i = 0; i < parameters.Count; i++) {
											convertedParameters[i] = Convert.ChangeType(this.currentConsoleCommand[i + 1], parameters[i].ParameterType, CultureInfo.InvariantCulture);
										}

										returnVal = methodInfo.Invoke(obj, convertedParameters);
									} catch (Exception e) { }
								}
							}

							if (returnVal != null) {
								Engine.console.WriteToOutputList((string) customAttribute.TryGetValue("outputFormat") + Convert.ToString(returnVal));
							}
						}
					}
				}
			}

			foreach (FieldInfo fieldInfo in obj.GetType().GetFields()) {
				try {
					object member = fieldInfo.GetValue(obj);
					this.SearchAllObjectsForAttributes(member);
				} catch (Exception e) { }
			}
		}

		public void ExecuteAttributesConsoleCommands() {
			this.alreadyVisited.Clear();
			this.SearchAllObjectsForAttributes(Engine.audio);
			this.SearchAllObjectsForAttributes(Engine.assets);
			this.SearchAllObjectsForAttributes(Engine.camera);
			this.SearchAllObjectsForAttributes(Engine.console);
			this.SearchAllObjectsForAttributes(Engine.coroutines);
			this.SearchAllObjectsForAttributes(Engine.game);
			this.SearchAllObjectsForAttributes(Engine.graphics);
			this.SearchAllObjectsForAttributes(Engine.time);
			this.SearchAllObjectsForAttributes(Engine.gui);
			this.SearchAllObjectsForAttributes(Engine.input);
			this.SearchAllObjectsForAttributes(Engine.inspector);
			this.SearchAllObjectsForAttributes(Engine.time);
			this.SearchAllObjectsForAttributes(Engine.window);
		}

		public void EnterConsoleCommand(in string command) {
			this.WriteToOutputList(command);
			this.currentConsoleCommand = command.Split(" ");
			this.ExecuteAttributesConsoleCommands();

			List<string> values;
			if (Engine.console.CheckConsoleCommand("INSPECT_GAME", 0, out values)) {
				try {
					Engine.inspector.BuildInspectionGui(Engine.game);
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			} else if (Engine.console.CheckConsoleCommand("SET_GAME", 1, out values)) {
				try {
					Engine.SetGame(ReflectionH.GetObjectByClassName<GameE>(values[0]));
				} catch (Exception e) {
					Console.WriteLine(e.Message);
				}
			} else if (Engine.console.CheckConsoleCommand("CALL_METHOD", 1, out values)) {
				ReflectionH.RunStaticMethodByName(values[0]);
			}

			this.ClearConsoleInput();
		}

		public void WriteToOutputList(in string text) {
			this.consoleOutputTextList.Add(text);
			if (this.consoleOutputTextList.Count == 5) this.consoleOutputTextList.RemoveAt(0);
		}

		private void InsertLetter(in string letter) {
			if (letter.Length == 1)
				this.consoleInputText.DisplayedString += letter;
		}

		private void RemoveLetter() {
			if (this.consoleInputText.DisplayedString.Length > 0)
				this.consoleInputText.DisplayedString = this.consoleInputText.DisplayedString.Remove(this.consoleInputText.DisplayedString.Length - 1, 1);
		}

		private void ClearConsoleInput() {
			this.consoleInputText.DisplayedString = "";
		}

		private void ToggleConsole() {
			this.showConsole = !this.showConsole;
			this.consoleGraphicsTrack.active = this.showConsole;
		}

		private void HandleKeyInput() {
			if (Engine.input.Key((KeyButton.Backspace, ButtonState.DOWN))) {
				this.RemoveLetter();
				this.caretCounter = 0.5f;
				this.consoleOutputSelectedIndex = -1;
			} else if (Engine.input.Key((KeyButton.Backspace, ButtonState.HELD))) {
				if (this.backspaceCounter > 0.40f) {
					this.RemoveLetter();
					this.caretCounter = 0.5f;
					this.consoleOutputSelectedIndex = -1;
					this.backspaceCounter = 0.365f;
				}

				this.backspaceCounter += Engine.time.deltaRealTime;
			} else if (Engine.input.Key((KeyButton.Backspace, ButtonState.UP))) {
				this.backspaceCounter = 0f;
			}

			if (Engine.input.Key((KeyButton.Delete, ButtonState.DOWN))) {
				this.ClearConsoleInput();
				this.consoleOutputSelectedIndex = -1;
			}

			if (Engine.input.Key((KeyButton.Enter, ButtonState.DOWN))) {
				this.EnterConsoleCommand(this.consoleInputText.DisplayedString);
				this.consoleOutputSelectedIndex = -1;
			}

			if (Engine.input.Key((KeyButton.Up, ButtonState.DOWN))) {
				if (this.consoleOutputSelectedIndex == -1) {
					this.consoleOutputSelectedIndex = 0;
				}

				this.consoleOutputSelectedIndex = MathH.Mod(this.consoleOutputSelectedIndex - 1, this.consoleOutputTextList.Count);
				this.SetInputTextFromSelectedOutput();
			}

			if (Engine.input.Key((KeyButton.Down, ButtonState.DOWN))) {
				this.consoleOutputSelectedIndex = MathH.Mod(this.consoleOutputSelectedIndex + 1, this.consoleOutputTextList.Count);
				this.SetInputTextFromSelectedOutput();
			}

			if (Engine.input.Key((KeyButton.LControl, ButtonState.HELD), (KeyButton.C, ButtonState.DOWN))) {
				Engine.input.SetClipboard(this.consoleInputText.DisplayedString);
			}

			if (Engine.input.Key((KeyButton.LControl, ButtonState.HELD), (KeyButton.V, ButtonState.DOWN))) {
				this.consoleInputText.DisplayedString += Engine.input.GetClipboard();
			}
		}

		private void SetInputTextFromSelectedOutput() {
			if (this.consoleOutputTextList.Count > 0) {
				this.consoleInputText.DisplayedString = this.consoleOutputTextList[this.consoleOutputSelectedIndex];
			}
		}

		private void HandleUnicodeInput() {
			List<string> letters = Engine.input.textInputList;
			for (int i = 0; i < letters.Count; i++) {
				this.InsertLetter(letters[i]);
				this.caretCounter = 0.5f;
				this.consoleOutputSelectedIndex = -1;
			}
		}

		private void RenderOutputTextList() {
			for (int i = 0; i < this.consoleOutputTextList.Count; i++) {
				this.consoleOutputText.DisplayedString = this.consoleOutputTextList[i];
				this.consoleOutputText.Position = new Vector2f(0, i * ((this.consoleSize - 12) / 4f));
				this.consoleRenTexSprite.Draw(this.consoleOutputText);
			}
		}

		public void Update() {
			this.currentConsoleCommand = new[] {""};

			if (Engine.input.Key(keys: (KeyButton.F12, ButtonState.DOWN))) this.ToggleConsole();

			if (this.showConsole) {
				Engine.input.PushContext("Engine_Console");
				this.HandleKeyInput();
				this.HandleUnicodeInput();
			} else {
				Engine.input.PopContext("Engine_Console");
			}
		}

		public void Render() {
			if (this.showConsole) {
				this.consoleRenTexSprite.Clear();

				this.consoleRenTexSprite.Draw(this.consoleRect);
				this.consoleRenTexSprite.Draw(this.consoleTextRect);
				this.RenderOutputTextList();

				this.caretCounter += Engine.time.deltaRealTime;
				if (this.caretCounter > 0.5) {
					this.InsertLetter("|");
					this.consoleRenTexSprite.Draw(this.consoleInputText);
					this.RemoveLetter();

					if (this.caretCounter > 1f) this.caretCounter = 0f;
				} else {
					this.consoleRenTexSprite.Draw(this.consoleInputText);
				}

				this.consoleRenTexSprite.MoveTo(Engine.camera.CameraPosition());
				this.consoleRenTexSprite.ScaleTo(Engine.camera.CameraSize());
			}
		}
	}

	public class ConsoleCommandAttribute : Attribute {
		public string command;
		public string outputFormat;

		public ConsoleCommandAttribute(string command, string outputFormat = "") {
			this.command = command;
			this.outputFormat = outputFormat;
		}
	}
}