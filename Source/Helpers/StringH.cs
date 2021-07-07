using System.Globalization;

namespace Helpers {
	public static class ParseH {
		public static bool? ToBool(this string text) {
			if (text.ToUpper() == "TRUE" || text == "1") return true;
			if (text.ToUpper() == "FALSE" || text == "0") return false;

			return null;
		}

		public static int? ToInt(this string text) {
			if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int i)) return i;

			return null;
		}

		public static float? ToFloat(this string text) {
			if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out float i)) return i;

			return null;
		}

		public static double? ToDouble(this string text) {
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double i)) return i;

			return null;
		}
	}
}