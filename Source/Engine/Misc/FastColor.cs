using System;
using System.Numerics;
using Helpers;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace TangentEngine {
	public struct FastColor {
		private static readonly Vector4 clampMin = new Vector4(-1f, -1f, -1f, -1f);
		private static readonly Vector4 clampMax = new Vector4(1f, 1f, 1f, 1f);
		private static readonly Vector4 brightness = new Vector4(0.299f, 0.587f, 0.114f, 1f);

		public static readonly FastColor Black = new FastColor(0, 0, 0, 1);
		public static readonly FastColor White = new FastColor(1, 1, 1, 1);
		public static readonly FastColor DarkGrey = new FastColor(0.75f, 0.75f, 0.75f);
		public static readonly FastColor Grey = new FastColor(0.5f, 0.5f, 0.5f);
		public static readonly FastColor LightGrey = new FastColor(0.25f, 0.25f, 0.25f);
		public static readonly FastColor Red = new FastColor(1, 0, 0, 1);
		public static readonly FastColor Green = new FastColor(0, 1, 0, 1);
		public static readonly FastColor Blue = new FastColor(0, 0, 1, 1);
		public static readonly FastColor Yellow = new FastColor(1, 1, 0, 1);
		public static readonly FastColor Magenta = new FastColor(1, 0, 1, 1);
		public static readonly FastColor Cyan = new FastColor(0, 1, 1, 1);
		public static readonly FastColor Transparent = new FastColor(0, 0, 0, 0);

		private Vector4 vector;

		public float R {
			get => this.vector.X;
			set => this.vector.X = value;
		}

		public float G {
			get => this.vector.Y;
			set => this.vector.Y = value;
		}

		public float B {
			get => this.vector.Z;
			set => this.vector.Z = value;
		}

		public float A {
			get => this.vector.W;
			set => this.vector.W = value;
		}

		public FastColor(float r = 1, float g = 1, float b = 1, float a = 1) {
			this.vector = new Vector4(r, g, b, a);
		}

		public FastColor(byte r = 255, byte g = 255, byte b = 255, byte a = 255) {
			this.vector = new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
		}

		public FastColor(FastColor color) {
			this.vector = new Vector4(color.R, color.G, color.B, color.A);
		}

		private FastColor(Vector4 vector) {
			this.vector = vector;
		}

		public void AdjustHue(float amount) {
			FastColor HSL = RGBToHSV(this);
			HSL.R += amount;
			HSL.R = Math.Clamp(HSL.R, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public float GetHue() {
			FastColor HSL = RGBToHSV(this);
			return HSL.R;
		}

		public void SetHue(float hue) {
			FastColor HSL = RGBToHSV(this);
			HSL.R = hue;
			HSL.R = Math.Clamp(HSL.R, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public void AdjustSaturation(float amount) {
			FastColor HSL = RGBToHSV(this);
			HSL.G += amount;
			HSL.G = Math.Clamp(HSL.G, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public float GetSaturation() {
			FastColor HSL = RGBToHSV(this);
			return HSL.G;
		}

		public void SetSaturation(float saturation) {
			FastColor HSL = RGBToHSV(this);
			HSL.G = saturation;
			HSL.G = Math.Clamp(HSL.G, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public void AdjustBrightness(float amount) {
			FastColor HSL = RGBToHSV(this);
			HSL.B += amount;
			HSL.B = Math.Clamp(HSL.B, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public float GetBrightness() {
			FastColor HSL = RGBToHSV(this);
			return HSL.B;
		}

		public void SetBrightness(float brightness) {
			FastColor HSL = RGBToHSV(this);
			HSL.B = brightness;
			HSL.B = Math.Clamp(HSL.B, 0f, 1f);
			this.vector = HSVToRGB(HSL).vector;
		}

		public static FastColor[] GenerateFastColorTable(FastColor startFastColor, int points = 8, int layers = 4, float randLayerScale = 0f, float randHueScale = 0f, float randSatScale = 0f, float randBrightScale = 0f) {
			FastColor[] colors = new FastColor[points * layers];
			for (int i = 0; i < layers; i++) {
				FastColor layerFastColor = startFastColor * (i + 1 + RandomH.GetRandom(0f, randLayerScale));
				for (int j = 0; j < colors.Length / layers; j++) {
					layerFastColor.AdjustHue(1f / points + RandomH.GetRandom(0f, randHueScale));
					layerFastColor.AdjustSaturation(RandomH.GetRandom(0f, randSatScale));
					layerFastColor.AdjustBrightness(RandomH.GetRandom(0f, randBrightScale));
					colors[j * i] = new FastColor(HSVToRGB(layerFastColor));
				}
			}

			return colors;
		}

		public static float Brightness(FastColor color) {
			return Vector4.Dot(color.vector, brightness) / 2f;
		}

		public static FastColor Grayscale(FastColor color) {
			float brightness = Brightness(color);
			return new FastColor(brightness, brightness, brightness);
		}

		public static FastColor GetRandomFastColor() {
			return new FastColor(RandomH.GetRandom(0f, 1f), RandomH.GetRandom(0f, 1f), RandomH.GetRandom(0f, 1f));
		}

		public static FastColor GetRandomFastColorScaled(FastColor color, float scalar) {
			color.vector *= scalar;
			return color;
		}

		public static FastColor GetRandomFixedHue(float hue) {
			return HSVToRGB(new FastColor(hue, RandomH.GetRandom(0f, 1f), RandomH.GetRandom(0f, 1f)));
		}

		public static FastColor GetRandomFixedSaturation(float saturation) {
			return HSVToRGB(new FastColor(RandomH.GetRandom(0f, 1f), saturation, RandomH.GetRandom(0f, 1f)));
		}

		public static FastColor GetRandomFixedBrightness(float brightness) {
			return HSVToRGB(new FastColor(RandomH.GetRandom(0f, 1f), RandomH.GetRandom(0f, 1f), brightness));
		}

		public static FastColor RGBToHSV(FastColor color) {
			Vector4 K = new Vector4(0f, -1f / 3f, 2f / 3f, -1f);

			float px = color.G > color.B ? 1f : 0f;
			Vector4 p = Vector4.Lerp(new Vector4(color.B, color.G, K.W, K.Z), new Vector4(color.G, color.B, K.X, K.Y), px);
			float qx = color.R > p.X ? 1f : 0f;
			Vector4 q = Vector4.Lerp(new Vector4(p.X, p.Y, p.W, color.R), new Vector4(color.R, p.Y, p.Z, p.X), qx);

			float d = q.X - MathF.Min(q.W, q.Y);
			float e = (float) 1.0e-10;
			return new FastColor(new Vector4(MathF.Abs(q.Z + (q.W - q.Y) / (6f * d + e)), d / (q.X + e), q.X, color.A));
		}

		public static FastColor HSVToRGB(FastColor color) {
			Vector4 K = new Vector4(1f, 2f / 3f, 1f / 3f, 3f);
			Vector3 round = new Vector3(color.R, color.R, color.R) + new Vector3(K.X, K.Y, K.Z);
			round.X = round.X % 1f;
			round.Y = round.Y % 1f;
			round.Z = round.Z % 1f;
			Vector3 p = Vector3.Abs(round * 6f - new Vector3(K.W, K.W, K.W));
			return new FastColor(new Vector4(color.B * Vector3.Lerp(new Vector3(K.X, K.X, K.X), Vector3.Clamp(p - new Vector3(K.X, K.X, K.X), Vector3.Zero, Vector3.One), color.G), 1));
		}

		public static FastColor LerpRGB(FastColor color1, FastColor color2, float frac) {
			return new FastColor(Vector4.Lerp(color1.vector, color2.vector, frac));
		}

		public static FastColor LerpHSV(FastColor color1, FastColor color2, float frac) {
			color1 = RGBToHSV(color1);
			color2 = RGBToHSV(color2);
			return HSVToRGB(new FastColor(Vector4.Lerp(color1.vector, color2.vector, frac)));
		}

		public static FastColor operator +(FastColor left, FastColor right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector + right.vector, clampMin, clampMax));
			return color;
		}

		public static FastColor operator +(FastColor left, float right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector + new Vector4(right), clampMin, clampMax));
			return color;
		}

		public static FastColor operator ++(FastColor left) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector + Vector4.One, clampMin, clampMax));
			return color;
		}

		public static FastColor operator -(FastColor left, FastColor right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector - right.vector, clampMin, clampMax));
			return color;
		}

		public static FastColor operator -(FastColor left, float right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector - new Vector4(right), clampMin, clampMax));
			return color;
		}

		public static FastColor operator --(FastColor left) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector - Vector4.One, clampMin, clampMax));
			return color;
		}

		public static FastColor operator *(FastColor left, FastColor right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector * right.vector, clampMin, clampMax));
			return color;
		}

		public static FastColor operator *(FastColor left, float right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector * new Vector4(right), clampMin, clampMax));
			return color;
		}

		public static FastColor operator /(FastColor left, FastColor right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector / right.vector, clampMin, clampMax));
			return color;
		}

		public static FastColor operator /(FastColor left, float right) {
			FastColor color = new FastColor(Vector4.Clamp(left.vector / new Vector4(right), clampMin, clampMax));
			return color;
		}

		public static bool operator ==(FastColor left, FastColor right) {
			return left.Equals(right);
		}

		public static bool operator !=(FastColor left, FastColor right) {
			return !left.Equals(right);
		}

		public override string ToString() {
			return string.Format("[FastColor] R({0}) G({1}) B({2}) A({3})", (object) this.R, (object) this.G, (object) this.B, (object) this.A);
		}

		public override bool Equals(object obj) {
			return obj is FastColor other && this.Equals(other);
		}

		private bool Equals(FastColor other) {
			float tolerance = 0.001f;
			return MathF.Abs(this.R - other.R) < tolerance && MathF.Abs(this.G - other.G) < tolerance && MathF.Abs(this.B - other.B) < tolerance && MathF.Abs(this.A - other.A) < tolerance;
		}

		public static explicit operator Color(FastColor color) {
			color.vector *= 255f;
			return new Color((byte) color.R, (byte) color.G, (byte) color.B, (byte) color.A);
		}

		public static implicit operator FastColor(Color color) {
			return new FastColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
		}

		public static implicit operator Vec4(FastColor color) {
			return new Vec4(color.R, color.G, color.B, color.A);
		}

		public static implicit operator Vector4(FastColor color) {
			return new Vector4(color.R, color.G, color.B, color.A);
		}
	}
}