using System;
using ColorMine.ColorSpaces;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace Helpers {
	public static class ColorH {
		public static Color HslToRgb(float h, float s, float l, byte a) {
			Hsl? myHsl = new Hsl(h, s, l);
			IRgb? myRgb = myHsl.ToRgb();
			return new Color((byte) myRgb.R, (byte) myRgb.G, (byte) myRgb.B, a);
		}

		public static void HslToRgb(float h, float s, float l, ref Color color) {
			Hsl? myHsl = new Hsl(h, s, l);
			IRgb? myRgb = myHsl.ToRgb();
			color.R = (byte) myRgb.R;
			color.G = (byte) myRgb.G;
			color.B = (byte) myRgb.B;
		}

		public static Color HsvToRgb(float hue, float saturation, float value) {
			Hsv? myLch = new Hsv(hue, saturation, value);
			IRgb? myRgb = myLch.ToRgb();
			return new Color((byte) myRgb.R, (byte) myRgb.G, (byte) myRgb.B, 255);
		}

		public static Color HsvToRgb(Hsv hsv) {
			return HsvToRgb((float) hsv.H, (float) hsv.S, (float) hsv.V);
		}

		public static Color LchToRgb(float l, float c, float h, byte a) {
			Lch? myLch = new Lch(l, c, h);
			IRgb? myRgb = myLch.ToRgb();
			return new Color((byte) (myRgb.R * 255f), (byte) (myRgb.G * 255f), (byte) (myRgb.B * 255f), a);
		}

		public static void LchToRgb(float l, float c, float h, ref Color color) {
			Lch? myLch = new Lch(l, c, h);
			IRgb? myRgb = myLch.ToRgb();
			color.R = (byte) myRgb.R;
			color.G = (byte) myRgb.G;
			color.B = (byte) myRgb.B;
		}

		public static Lch RgbToLch(Color c) {
			Rgb? myRgb = new Rgb(c.R, c.G, c.B);
			return myRgb.To<Lch>();
		}

		public static Hsl RgbToHsl(Color c) {
			Rgb? myRgb = new Rgb(c.R, c.G, c.B);
			return myRgb.To<Hsl>();
		}

		public static Hsv RgbToHsv(Color startValueRgb) {
			Rgb? myRgb = new Rgb(startValueRgb.R, startValueRgb.G, startValueRgb.B);
			return myRgb.To<Hsv>();
		}

		public static float RgbToLuma(Color color) {
			return color.R / 255f * 0.3f + color.G / 255f * 0.59f + color.B / 255f * 0.11f;
		}

		public static Color RgbToGrayscale(Color color) {
			float luma = RgbToLuma(color);
			return new Color((byte) (luma * 255f), (byte) (luma * 255f), (byte) (luma * 255f));
		}

		public static Vec4 ToVec4(this Color color) {
			return new Vec4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
		}

		public static Color Divide(this Color color, float div) {
			color.R = (byte) (color.R / div);
			color.G = (byte) (color.G / div);
			color.B = (byte) (color.B / div);
			color.A = (byte) (color.A / div);
			return color;
		}

		public static Color Multiply(this Color color, float div) {
			color.R = (byte) Math.Clamp(color.R * div, byte.MinValue, byte.MaxValue);
			color.G = (byte) Math.Clamp(color.G * div, byte.MinValue, byte.MaxValue);
			color.B = (byte) Math.Clamp(color.B * div, byte.MinValue, byte.MaxValue);
			color.A = (byte) Math.Clamp(color.A * div, byte.MinValue, byte.MaxValue);
			return color;
		}
	}
}