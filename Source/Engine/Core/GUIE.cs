using System;
using TGUI;

namespace TangentEngine {
	public class GUIE {
		private Gui gui;

		public GUIE(Gui gui) {
			this.gui = gui;

			try {
				// Will default to default ui if this one is not found
				Theme.Default.load("C:\\Users\\Alfred\\RiderProjects\\Engine\\Source\\Engine\\Assets\\Nanogui\\nanogui.style");
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		public void Update() { }

		[ConsoleCommand("CLEAR_GUI")]
		public void Reset() {
			this.gui.RemoveAllWidgets();
		}

		public void Display() {
			this.gui.Draw();
		}

		public void Add(Widget widget) {
			this.gui.Add(widget);
		}

		public void Remove(Widget widget) {
			this.gui.Remove(widget);
		}
	}
}