using System;
using System.Collections.Generic;
using System.Reflection;
using ColorMine.ColorSpaces;
using Fasterflect;
using Helpers;
using JM.LinqFaster;
using SFML.Graphics;
using SFML.System;
using TGUI;

namespace TangentEngine {
	public class InspectorE {
		private Dictionary<string, object> methodCaller;
		private List<Action> valueUpdater;

		public InspectorE() {
			this.methodCaller = new Dictionary<string, object>();
			this.valueUpdater = new List<Action>();
		}

		public void Update() {
			foreach ((string key, object value) in this.methodCaller) {
				value.TryCallMethod(key, false, null);
			}

			this.methodCaller.Clear();

			foreach (Action action in this.valueUpdater) {
				action.Invoke();
			}
		}

		public void Reset() {
			this.methodCaller.Clear();
			this.valueUpdater.Clear();
		}

		private List<object> alreadyVisited = new List<object>(); // to avoid infinite recursion

		private object[] customAttributes = {
			typeof(InspectNumericalAttribute),
			typeof(InspectBoolAttribute),
			typeof(InspectColorAttribute),
			typeof(InspectEnumAttribute),
			typeof(InspectMethodAttribute),
			typeof(InspectStringAttribute),
			typeof(InspectVector2fAttribute),
			typeof(InspectNumericalTupleAttribute)
		};

		private void SearchAllObjectsForAttributes(object obj) {
			if (alreadyVisited.Contains(obj) || obj is Pointer) return;
			alreadyVisited.Add(obj);

			foreach (MethodInfo methodInfo in obj.GetType().Methods()) {
				if (methodInfo.GetCustomAttributes(true).AnyF(x => this.customAttributes.ContainsF(x.GetType()))) {
					Console.WriteLine(methodInfo);
					this.BuildInspectionGui(obj);
					break;
				}
			}

			foreach (FieldInfo fieldInfo in obj.GetType().Fields()) {
				if (fieldInfo.GetCustomAttributes(true).AnyF(x => this.customAttributes.ContainsF(x.GetType()))) {
					Console.WriteLine(fieldInfo);
					this.BuildInspectionGui(obj);
					break;
				}
			}

			foreach (FieldInfo fieldInfo in obj.GetType().GetFields()) {
				try {
					object member = fieldInfo.GetValue(obj);
					this.SearchAllObjectsForAttributes(member);
				} catch (Exception e) { }
			}
		}

		[ConsoleCommand("INSPECT_ALL")]
		public void BuildInspectionGuiAll() {
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

		public void BuildInspectionGui(object obj) {
			// CHILD WINDOW SETUP AND LAYOUT SETUP ---------------------------------------------------------------------------------
			ChildWindow childWindow = new ChildWindow(obj.GetType().Name);
			childWindow.SizeLayout = new Layout2d("30%", "40%");
			childWindow.Resizable = true;
			childWindow.Renderer.MinimumResizableBorderWidth = 16f;
			childWindow.Renderer.Opacity = 0.85f;
			childWindow.ShowWithEffect(ShowAnimationType.Fade, Time.FromSeconds(0.15f));

			childWindow.Closed += (sender, args) => {
				childWindow.HideWithEffect(ShowAnimationType.Fade, Time.FromSeconds(0.05f));
				childWindow.AnimationFinished += (o, animation) => { Engine.gui.Remove(childWindow); };
			};

			childWindow.MouseEntered += (sender, args) => { Engine.input.PushContext("Engine_Inspector"); };
			childWindow.MouseLeft += (sender, args) => { Engine.input.PopContext("Engine_Inspector"); };

			VerticalLayout verticalLayout = new VerticalLayout();
			verticalLayout.Size = childWindow.Size;
			verticalLayout.SizeLayout = new Layout2d("100%", "100%");

			// METHOD INSPECTOR --------------------------------------------------------------------------------------------------
			foreach (MethodInfo methodInfo in obj.GetType().Methods()) {
				foreach (Attribute customAttribute in methodInfo.GetCustomAttributes()) {
					if (customAttribute.GetType() == typeof(InspectMethodAttribute)) {
						Button button = new Button(methodInfo.Name);
						button.Clicked += (sender, f) => { methodInfo.Invoke(obj, null); };

						verticalLayout.Add(button);
					}
				}
			}

			// FIELDS INSPECTOR --------------------------------------------------------------------------------------------------
			foreach (FieldInfo fieldInfo in obj.GetType().Fields()) {
				foreach (Attribute customAttribute in fieldInfo.GetCustomAttributes()) {
					if (customAttribute.GetType() == typeof(InspectNumericalAttribute)) {
						double startValue = this.GetInfoNumber(obj, fieldInfo);

						Slider slider = new Slider((float) this.GetAttributeNumber(customAttribute, "min"), (float) this.GetAttributeNumber(customAttribute, "max"));
						slider.Value = (float) startValue;
						slider.Step = (float) this.GetAttributeNumber(customAttribute, "step");

						Label label = new Label($"{fieldInfo.Name}: {startValue:#.000}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						bool shouldCallMethod = true;
						slider.ValueChanged += (sender, i) => {
							this.SetInfoNumber(obj, fieldInfo, i.Value);

							label.Text = $"{fieldInfo.Name}: {i.Value:#.000}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						label.DoubleClicked += (sender, s) => {
							this.SetInfoNumber(obj, fieldInfo, startValue);

							label.Text = $"{fieldInfo.Name}: {startValue:#.000}";
							slider.Value = (float) startValue;
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							double currValue = this.GetInfoNumber(obj, fieldInfo);
							Engine.input.SetClipboard(currValue.ToString("#.000").Replace(",", "."));
						};

						Action valueUpdaterAction = () => {
							double currValue = this.GetInfoNumber(obj, fieldInfo);
							shouldCallMethod = false;
							slider.Value = (float) currValue;
							shouldCallMethod = true;
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(slider);
					} else if (customAttribute.GetType() == typeof(InspectBoolAttribute)) {
						bool startValue = (bool) fieldInfo.GetValue(obj);

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						label.DoubleClicked += (sender, s) => {
							fieldInfo.SetValue(obj, startValue);
							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						label.RightMousePressed += (sender, f) => {
							bool currValue = (bool) fieldInfo.GetValue(obj);
							Engine.input.SetClipboard(currValue.ToString());
						};

						Button button = new Button(fieldInfo.GetValue(obj).ToString());
						button.Clicked += (sender, f) => {
							bool state = (bool) fieldInfo.GetValue(obj);
							fieldInfo.SetValue(obj, !state);
							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							button.Text = fieldInfo.GetValue(obj).ToString();
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						Action valueUpdaterAction = () => {
							bool currValue = (bool) fieldInfo.GetValue(obj);
							label.Text = $"{fieldInfo.Name}: {currValue}";
							button.Text = currValue.ToString();
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(button);
					} else if (customAttribute.GetType() == typeof(InspectStringAttribute)) {
						string startValue = (string) fieldInfo.GetValue(obj);

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						EditBox editBox = new EditBox();
						editBox.Alignment = HorizontalAlignment.Center;
						editBox.Text = startValue;
						editBox.ReturnKeyPressed += (sender, s) => {
							fieldInfo.SetValue(obj, s.Value);
							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						label.DoubleClicked += (sender, s) => {
							fieldInfo.SetValue(obj, startValue);
							editBox.Text = startValue;
							label.Text = $"{fieldInfo.Name}: {startValue}";
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							string currValue = (string) fieldInfo.GetValue(obj);
							Engine.input.SetClipboard(currValue);
						};

						string prevValue = startValue;

						Action valueUpdaterAction = () => {
							string currValue = (string) fieldInfo.GetValue(obj);
							if (currValue != prevValue) {
								prevValue = currValue;
								editBox.Text = currValue;
								label.Text = $"{fieldInfo.Name}: {currValue}";
							}
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(editBox);
					} else if (customAttribute.GetType() == typeof(InspectNumericalTupleAttribute)) {
						(double, double) startValue = this.GetInfoNumberTuple(obj, fieldInfo);

						Slider slider1 = new Slider((float) this.GetAttributeNumber(customAttribute, "min"), (float) this.GetAttributeNumber(customAttribute, "max"));
						slider1.Value = (float) startValue.Item1;
						slider1.Step = (float) this.GetAttributeNumber(customAttribute, "step");

						Slider slider2 = new Slider((float) this.GetAttributeNumber(customAttribute, "min"), (float) this.GetAttributeNumber(customAttribute, "max"));
						slider2.Value = (float) startValue.Item2;
						slider2.Step = (float) this.GetAttributeNumber(customAttribute, "step");

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						bool shouldCallMethod = true;
						slider1.ValueChanged += (sender, i) => {
							(double, double) f = this.GetInfoNumberTuple(obj, fieldInfo);
							this.SetInfoNumberTuple(obj, fieldInfo, ((double) i.Value, f.Item2));

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						slider2.ValueChanged += (sender, i) => {
							(double, double) f = this.GetInfoNumberTuple(obj, fieldInfo);

							this.SetInfoNumberTuple(obj, fieldInfo, (f.Item1, (double) i.Value));

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						label.DoubleClicked += (sender, s) => {
							this.SetInfoNumberTuple(obj, fieldInfo, startValue);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							slider1.Value = (float) startValue.Item1;
							slider2.Value = (float) startValue.Item2;
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							(double, double) currValue = this.GetInfoNumberTuple(obj, fieldInfo);
							Engine.input.SetClipboard(
								$"({currValue.Item1.ToString("#.000").Replace(",", ".")},{currValue.Item2.ToString("#.000").Replace(",", ".")})"
							);
						};

						Action valueUpdaterAction = () => {
							(double, double) currValue = this.GetInfoNumberTuple(obj, fieldInfo);
							shouldCallMethod = false;
							slider1.Value = (float) currValue.Item1;
							slider2.Value = (float) currValue.Item2;
							shouldCallMethod = true;
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(slider1);
						verticalLayout.Add(slider2);
					} else if (customAttribute.GetType() == typeof(InspectEnumAttribute)) {
						object? startValue = fieldInfo.GetValue(obj);

						Type enumUnderlyingType = Enum.GetUnderlyingType(fieldInfo.FieldType);
						Array enumValues = Enum.GetValues(fieldInfo.FieldType);

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						label.DoubleClicked += (sender, s) => {
							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							fieldInfo.SetValue(obj, startValue);
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							object? currValue = fieldInfo.GetValue(obj);
							Engine.input.SetClipboard(currValue.ToString());
						};

						HorizontalLayout horizontalLayout = new HorizontalLayout();
						horizontalLayout.Size = childWindow.Size;
						horizontalLayout.SizeLayout = new Layout2d("100%", "100%");

						foreach (object? enumValue in enumValues) {
							Button button = new Button(enumValue.ToString());
							button.Clicked += (sender, f) => {
								fieldInfo.SetValue(obj, enumValue);
								label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
								if (customAttribute.TryGetValue("method") != "") {
									this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
								}
							};
							horizontalLayout.Add(button);
						}

						Action valueUpdaterAction = () => {
							object? currValue = fieldInfo.GetValue(obj);
							label.Text = $"{fieldInfo.Name}: {currValue}";
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(horizontalLayout);
					} else if (customAttribute.GetType() == typeof(InspectColorAttribute)) {
						Color startValueRGB = (Color) fieldInfo.GetValue(obj);
						Hsv startValueHSV = ColorH.RgbToHsv(startValueRGB);

						Canvas canvas = new Canvas();

						float hue = (float) startValueHSV.H;
						float sat = (float) startValueHSV.S;
						float val = (float) startValueHSV.V;
						float alpha = startValueRGB.A;

						Vector2f mouseClickPos = new Vector2f(0f, 0f);

						void RenderCanvas() {
							childWindow.Remove(verticalLayout);
							childWindow.Add(verticalLayout);
							for (int i = 0; i < verticalLayout.Widgets.Count; i++) {
								if (verticalLayout.Widgets[i] == canvas) {
									verticalLayout.Remove((uint) i);
									verticalLayout.Insert((uint) i, canvas);

									break;
								}
							}

							Color topLeft = ColorH.HsvToRgb(hue, 0f, 1f);
							topLeft.A = (byte) alpha;

							Color topRight = ColorH.HsvToRgb(hue, 1f, 1f);
							topRight.A = (byte) alpha;

							Color bottomLeft = ColorH.HsvToRgb(hue, 0f, 0f);
							bottomLeft.A = (byte) alpha;

							Color bottomRight = ColorH.HsvToRgb(hue, 1f, 0f);
							bottomRight.A = (byte) alpha;

							VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4);
							vertexArray.Append(new Vertex(new Vector2f(0f, 0f), topLeft));
							vertexArray.Append(new Vertex(new Vector2f(canvas.Size.X, 0f), topRight));
							vertexArray.Append(new Vertex(new Vector2f(canvas.Size.X, canvas.Size.Y), bottomRight));
							vertexArray.Append(new Vertex(new Vector2f(0f, canvas.Size.Y), bottomLeft));

							canvas.Clear(new Color(0, 0, 0, 255));
							canvas.Draw(vertexArray, new RenderStates(BlendMode.Alpha));

							RectangleShape rectangleShape = new RectangleShape(new Vector2f(12f, 12f));
							rectangleShape.Origin = rectangleShape.Size / 2f;
							rectangleShape.Position = new Vector2f(
								TweenH.Linear(mouseClickPos.X, 0f, 1f, 0f, canvas.Size.X),
								TweenH.Linear(mouseClickPos.Y, 0f, 1f, 0f, canvas.Size.Y));
							rectangleShape.Rotation = 45f;
							rectangleShape.FillColor = new Color(0, 0, 0, 196);
							canvas.Draw(rectangleShape, new RenderStates(BlendMode.Alpha));
						}

						Color GetCanvasColor() {
							float mouseXPos = Engine.input.GetMouseWindowPosition().X;
							float mouseYPos = Engine.input.GetMouseWindowPosition().Y;

							float canvasTopLeftXPos = canvas.AbsolutePosition.X;
							float canvasTopLeftYPos = canvas.AbsolutePosition.Y;

							float canvasBottomRightXPos = canvas.AbsolutePosition.X + canvas.Size.X;
							float canvasBottomRightYPos = canvas.AbsolutePosition.Y + canvas.Size.Y;

							float h = hue;
							float s = TweenH.Linear(mouseXPos, canvasTopLeftXPos, canvasBottomRightXPos, 0f, 1f);
							float v = TweenH.Linear(mouseYPos, canvasBottomRightYPos, canvasTopLeftYPos, 0f, 1f);
							float a = alpha;

							mouseClickPos = new Vector2f(
								TweenH.Linear(mouseXPos, canvasTopLeftXPos, canvasBottomRightXPos, 0f, 1f),
								TweenH.Linear(mouseYPos, canvasTopLeftYPos, canvasBottomRightYPos, 0f, 1f));

							Color col = ColorH.HsvToRgb(h, s, v);
							col.A = (byte) a;

							RenderCanvas();

							return col;
						}

						bool MouseInsideCanvas(Vector2f windowPos) {
							FloatRect floatRect = new FloatRect(canvas.AbsolutePosition.X, canvas.AbsolutePosition.Y, canvas.Size.X, canvas.Size.Y);
							return floatRect.Contains(windowPos.X, windowPos.Y);
						}

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						bool shouldCallMethod = true;

						Slider sliderHue = new Slider(0, 360);
						sliderHue.Value = (float) startValueHSV.H;
						sliderHue.Step = 1;
						sliderHue.ValueChanged += (sender, i) => {
							hue = i.Value;
							Hsv currCol = ColorH.RgbToHsv((Color) fieldInfo.GetValue(obj));
							currCol.H = hue;
							Color newCol = ColorH.HsvToRgb(currCol);
							newCol.A = (byte) alpha;
							fieldInfo.SetValue(obj, newCol);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}

							RenderCanvas();
						};

						Slider sliderAlpha = new Slider(0, 255);
						sliderAlpha.Value = startValueRGB.A;
						sliderAlpha.Step = 1;
						sliderAlpha.ValueChanged += (sender, i) => {
							alpha = i.Value;
							Hsv currCol = ColorH.RgbToHsv((Color) fieldInfo.GetValue(obj));
							Color newCol = ColorH.HsvToRgb(currCol);
							newCol.A = (byte) alpha;
							fieldInfo.SetValue(obj, newCol);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}

							RenderCanvas();
						};

						label.DoubleClicked += (sender, s) => {
							fieldInfo.SetValue(obj, startValueRGB);

							label.Text = $"{fieldInfo.Name}: {startValueRGB}";
							sliderHue.Value = (float) startValueHSV.H;
							sliderAlpha.Value = startValueRGB.A;
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							Color currValueRGB = (Color) fieldInfo.GetValue(obj);
							Hsv currValueHSV = ColorH.RgbToHsv(currValueRGB);

							Engine.input.SetClipboard(
								$"({currValueRGB.R},{currValueRGB.G},{currValueRGB.B},{currValueRGB.A})");
						};

						childWindow.SizeChanged += (sender, f) => { RenderCanvas(); };
						childWindow.MousePressed += (sender, args) => {
							if (MouseInsideCanvas(Engine.input.GetMouseWindowPosition())) {
								fieldInfo.SetValue(obj, GetCanvasColor());

								label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
								if (customAttribute.TryGetValue("method") != "") {
									this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
								}
							}
						};
						RenderCanvas();

						Action valueUpdaterAction = () => {
							Color currValueRGB = (Color) fieldInfo.GetValue(obj);
							Hsv currValueHSV = ColorH.RgbToHsv(currValueRGB);

							shouldCallMethod = false;
							sliderHue.Value = (float) currValueHSV.H;
							sliderAlpha.Value = currValueRGB.A;
							shouldCallMethod = true;
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(sliderHue);
						verticalLayout.Add(sliderAlpha);
						verticalLayout.Add(canvas);
					} else if (customAttribute.GetType() == typeof(InspectVector2fAttribute)) {
						Vector2f startValue = (Vector2f) fieldInfo.GetValue(obj);

						Slider slider1 = new Slider((float) this.GetAttributeNumber(customAttribute, "min"), (float) this.GetAttributeNumber(customAttribute, "max"));
						slider1.Value = startValue.X;
						slider1.Step = (float) this.GetAttributeNumber(customAttribute, "step");

						Slider slider2 = new Slider((float) this.GetAttributeNumber(customAttribute, "min"), (float) this.GetAttributeNumber(customAttribute, "max"));
						slider2.Value = startValue.Y;
						slider2.Step = (float) this.GetAttributeNumber(customAttribute, "step");

						Label label = new Label($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
						label.HorizontalAlignment = HorizontalAlignment.Center;

						bool shouldCallMethod = true;

						slider1.ValueChanged += (sender, i) => {
							Vector2f f = (Vector2f) fieldInfo.GetValue(obj);
							f.X = i.Value;
							fieldInfo.SetValue(obj, f);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						slider2.ValueChanged += (sender, i) => {
							Vector2f f = (Vector2f) fieldInfo.GetValue(obj);
							f.Y = i.Value;
							fieldInfo.SetValue(obj, f);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							if (customAttribute.TryGetValue("method") != "" && shouldCallMethod) {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};

						label.DoubleClicked += (sender, s) => {
							fieldInfo.SetValue(obj, startValue);

							label.Text = $"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}";
							slider1.Value = startValue.X;
							slider2.Value = startValue.Y;
							if (customAttribute.TryGetValue("method") != "") {
								this.methodCaller[(string) customAttribute.TryGetValue("method")] = obj;
							}
						};
						label.RightMousePressed += (sender, f) => {
							Vector2f currValue = (Vector2f) fieldInfo.GetValue(obj);

							Engine.input.SetClipboard(
								$"({currValue.X.ToString("#.000").Replace(",", ".")},{currValue.Y.ToString("#.000").Replace(",", ".")})"
							);
						};

						Action valueUpdaterAction = () => {
							Vector2f currValue = (Vector2f) fieldInfo.GetValue(obj);

							shouldCallMethod = false;
							slider1.Value = currValue.X;
							slider2.Value = currValue.Y;
							shouldCallMethod = true;
						};

						this.valueUpdater.Add(valueUpdaterAction);
						childWindow.Closed += (sender, args) => { this.valueUpdater.Remove(valueUpdaterAction); };

						verticalLayout.Add(label);
						verticalLayout.Add(slider1);
						verticalLayout.Add(slider2);
					}
				}
			}

			// ADD WINDOW TO GUI --------------------------------------------------------------------------------------------------
			if (verticalLayout.Widgets.Count > 0) {
				childWindow.Add(verticalLayout);
				Engine.gui.Add(childWindow);
			}
		}

		private double GetInfoNumber(object obj, PropertyInfo info) {
			if (info.PropertyType == typeof(int)) {
				return (int) info.GetValue(obj);
			}

			if (info.PropertyType == typeof(short)) {
				return (short) info.GetValue(obj);
			}

			if (info.PropertyType == typeof(float)) {
				return (float) info.GetValue(obj);
			}

			if (info.PropertyType == typeof(double)) {
				return (double) info.GetValue(obj);
			}

			return 0.0;
		}

		private void SetInfoNumber(object obj, PropertyInfo info, double num) {
			if (info.PropertyType == typeof(int)) {
				info.SetValue(obj, (int) num);
			} else if (info.PropertyType == typeof(short)) {
				info.SetValue(obj, (short) num);
			} else if (info.PropertyType == typeof(float)) {
				info.SetValue(obj, (float) num);
			} else if (info.PropertyType == typeof(double)) {
				info.SetValue(obj, num);
			}
		}

		private (double, double) GetInfoNumberTuple(object obj, PropertyInfo info) {
			if (info.PropertyType == typeof((int, int))) {
				return ((int, int)) info.GetValue(obj);
			}

			if (info.PropertyType == typeof((short, short))) {
				return ((short, short)) info.GetValue(obj);
			}

			if (info.PropertyType == typeof((float, float))) {
				return ((float, float)) info.GetValue(obj);
			}

			if (info.PropertyType == typeof((double, double))) {
				return ((double, double)) info.GetValue(obj);
			}

			return (0.0, 0.0);
		}

		private void SetInfoNumberTuple(object obj, PropertyInfo info, (double, double) num) {
			if (info.PropertyType == typeof((int, int))) {
				info.SetValue(obj, ((int, int)) num);
			} else if (info.PropertyType == typeof((short, short))) {
				info.SetValue(obj, ((short, short)) num);
			} else if (info.PropertyType == typeof((float, float))) {
				info.SetValue(obj, ((float, float)) num);
			} else if (info.PropertyType == typeof((double, double))) {
				info.SetValue(obj, num);
			}
		}

		private double GetInfoNumber(object obj, FieldInfo info) {
			if (info.FieldType == typeof(int)) {
				return (int) info.GetValue(obj);
			}

			if (info.FieldType == typeof(short)) {
				return (short) info.GetValue(obj);
			}

			if (info.FieldType == typeof(float)) {
				return (float) info.GetValue(obj);
			}

			if (info.FieldType == typeof(double)) {
				return (double) info.GetValue(obj);
			}

			return 0.0;
		}

		private void SetInfoNumber(object obj, FieldInfo info, double num) {
			if (info.FieldType == typeof(int)) {
				info.SetValue(obj, (int) num);
			} else if (info.FieldType == typeof(short)) {
				info.SetValue(obj, (short) num);
			} else if (info.FieldType == typeof(float)) {
				info.SetValue(obj, (float) num);
			} else if (info.FieldType == typeof(double)) {
				info.SetValue(obj, num);
			}
		}

		private (double, double) GetInfoNumberTuple(object obj, FieldInfo info) {
			if (info.FieldType == typeof((int, int))) {
				return ((int, int)) info.GetValue(obj);
			}

			if (info.FieldType == typeof((short, short))) {
				return ((short, short)) info.GetValue(obj);
			}

			if (info.FieldType == typeof((float, float))) {
				return ((float, float)) info.GetValue(obj);
			}

			if (info.FieldType == typeof((double, double))) {
				return ((double, double)) info.GetValue(obj);
			}

			return (0.0, 0.0);
		}

		private void SetInfoNumberTuple(object obj, FieldInfo info, (double, double) num) {
			if (info.FieldType == typeof((int, int))) {
				info.SetValue(obj, ((int, int)) num);
			} else if (info.FieldType == typeof((short, short))) {
				info.SetValue(obj, ((short, short)) num);
			} else if (info.FieldType == typeof((float, float))) {
				info.SetValue(obj, ((float, float)) num);
			} else if (info.FieldType == typeof((double, double))) {
				info.SetValue(obj, num);
			}
		}

		private double GetAttributeNumber(Attribute attr, string fieldName) {
			return (double) attr.TryGetValue(fieldName);
		}

		private void SetAttributeNumber(Attribute attr, string fieldName, double num) {
			attr.TrySetValue(fieldName, num);
		}
	}

	public class InspectBoolAttribute : Attribute {
		public string method;

		public InspectBoolAttribute(string method = "") {
			this.method = method;
		}
	}

	public class InspectEnumAttribute : Attribute {
		public string method;

		public InspectEnumAttribute(string method = "") {
			this.method = method;
		}
	}

	public class InspectStringAttribute : Attribute {
		public string method;

		public InspectStringAttribute(string method = "") {
			this.method = method;
		}
	}

	public class InspectColorAttribute : Attribute {
		public string method;

		public InspectColorAttribute(string method = "") {
			this.method = method;
		}
	}

	public class InspectMethodAttribute : Attribute { }

	public class InspectNumericalTupleAttribute : Attribute {
		public double max;
		public string method;
		public double min;
		public double step;

		public InspectNumericalTupleAttribute(double min, double max, double step = 1, string method = "") {
			this.min = min;
			this.max = max;
			this.step = step;
			this.method = method;
		}
	}

	public class InspectVector2fAttribute : Attribute {
		public double max;
		public string method;
		public double min;
		public double step;

		public InspectVector2fAttribute(double min, double max, double step = 1, string method = "") {
			this.min = min;
			this.max = max;
			this.step = step;
			this.method = method;
		}
	}

	public class InspectNumericalAttribute : Attribute {
		public double max;
		public string method;
		public double min;
		public double step;

		public InspectNumericalAttribute(double min, double max, double step = 0.01, string method = "") {
			this.min = min;
			this.max = max;
			this.step = step;
			this.method = method;
		}
	}
}