using System;
using System.Collections.Generic;
using ColorMine.ColorSpaces;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public static class TweenH {
		public static float Linear(float fromMin, float fromMax, float fraction) {
			return fromMin * (1f - fraction) + fromMax * fraction;
		}

		public static float Cosine(float fromMin, float fromMax, float fraction) {
			fraction = (1f - MathF.Cos(fraction * MathF.PI)) / 2f;
			return fromMin * (1f - fraction) + fromMax * fraction;
		}

		public static float Linear(float fromValue, float fromMin, float fromMax, float toMin, float toMax) {
			return Math.Clamp(Linear(toMin, toMax, (fromValue - fromMin) / (fromMax - fromMin)), MathF.Min(toMin, toMax), MathF.Max(toMin, toMax));
		}

		public static float Cosine(float fromValue, float fromMin, float fromMax, float toMin, float toMax) {
			return Math.Clamp(Cosine(toMin, toMax, (fromValue - fromMin) / (fromMax - fromMin)), MathF.Min(toMin, toMax), MathF.Max(toMin, toMax));
		}

		public static float Linear(float fromValue, float fromMin, float fromMax, in float[] array) {
			float x = Linear(fromValue, fromMin, fromMax, 0f, array.Length - 1f);
			int xRoundDown = (int) x;
			int xRoundup = (int) (x + 1f);
			float xFraction = x - xRoundDown;
			return Linear(array[xRoundDown], array[xRoundup], xFraction);
		}

		public static float Linear(float fromValue, float fromMin, float fromMax, in List<float> list) {
			float x = Linear(fromValue, fromMin, fromMax, 0f, list.Count - 1f);
			int xRoundDown = (int) x;
			int xRoundup = (int) (x + 1f);
			float xFraction = x - xRoundDown;
			return Linear(list[xRoundDown], list[xRoundup], xFraction);
		}

		public static float Cosine(float fromValue, float fromMin, float fromMax, in float[] array) {
			float x = Cosine(fromValue, fromMin, fromMax, 0f, array.Length - 1f);
			int xRoundDown = (int) x;
			int xRoundup = (int) (x + 1f);
			float xFraction = x - xRoundDown;
			return Cosine(array[xRoundDown], array[xRoundup], xFraction);
		}

		public static float Cosine(float fromValue, float fromMin, float fromMax, in List<float> list) {
			float x = Cosine(fromValue, fromMin, fromMax, 0f, list.Count - 1f);
			int xRoundDown = (int) x;
			int xRoundup = (int) (x + 1f);
			float xFraction = x - xRoundDown;
			return Cosine(list[xRoundDown], list[xRoundup], xFraction);
		}

		public static float Accelerate(float fromMin, float fromMax, float fraction, float factor) {
			return MathF.Pow(fraction, 2f * factor) * (fromMax - fromMin) + fromMin;
		}

		public static float Accelerate(float fromValue, float fromMin, float fromMax, float toMin, float toMax, float factor) {
			return Accelerate(toMin, toMax, (fromValue - fromMin) / (fromMax - fromMin), factor);
		}

		public static float Decelerate(float fromMin, float fromMax, float fraction, float factor) {
			return MathF.Pow(fraction, 1f / (factor * 2f)) * (fromMax - fromMin) + fromMin;
		}

		public static float Decelerate(float fromValue, float fromMin, float fromMax, float toMin, float toMax, float factor) {
			return Decelerate(toMin, toMax, (fromValue - fromMin) / (fromMax - fromMin), factor);
		}

		public static float SmoothStep(float x, float x0, float x1) {
			x = x * x * (3f - 2f * x);
			return x * (x1 - x0) + x0;
		}

		public static float SmootherStep(float x, float x0, float x1) {
			x = x * x * x * (x * (x * 6 - 15) + 10);
			return x * (x1 - x0) + x0;
		}

		public static float SmoothToTarget(float current, float target, float scalar) {
			return current += (target - current) * scalar;
		}

		public static void SmoothToTarget(ref float current, float target, float scalar) {
			current += (target - current) * scalar;
		}

		public static float ExponentialSmoothing(float input, float scalar) {
			return input *= scalar;
		}

		public static float LinearSmoothing(float input, float scalar) {
			return input += scalar;
		}

		public static Vector2f VectorLerp(float x, float x0, float x1, Vector2f v1, Vector2f v2) {
			Vector2f vector = new Vector2f();
			vector.X = Linear(x, x0, x1, v1.X, v2.X);
			vector.Y = Linear(x, x0, x1, v1.Y, v2.Y);
			return vector;
		}

		public static Vector2f VectorLerp(Vector2f x, Vector2f x0, Vector2f x1, Vector2f v1, Vector2f v2) {
			Vector2f vector = new Vector2f();
			vector.X = Linear(x.X, x0.X, x1.X, v1.X, v2.X);
			vector.Y = Linear(x.Y, x0.Y, x1.Y, v1.Y, v2.Y);
			return vector;
		}

		public static Vector3f VectorLerp(Vector3f fromMin, Vector3f fromMax, float fraction) {
			Vector3f v = new Vector3f();
			v.X = Linear(fromMin.X, fromMax.X, fraction);
			v.Y = Linear(fromMin.Y, fromMax.Y, fraction);
			v.Z = Linear(fromMin.Z, fromMax.Z, fraction);
			return v;
		}

		public static Vector2f VectorLerpNormalized(float x, float x0, float x1, Vector2f v1, Vector2f v2) {
			Vector2f vector = new Vector2f();
			vector.X = (byte) Linear(x, x0, x1, v1.X, v2.X);
			vector.Y = (byte) Linear(x, x0, x1, v1.Y, v2.Y);
			vector.Normalize();
			return vector;
		}

		public static Color ColorLerpRgb(float x, float x0, float x1, Color c1, Color c2) {
			Color color = new Color();
			color.R = (byte) Linear(x, x0, x1, c1.R, c2.R);
			color.G = (byte) Linear(x, x0, x1, c1.G, c2.G);
			color.B = (byte) Linear(x, x0, x1, c1.B, c2.B);
			color.A = (byte) Linear(x, x0, x1, c1.A, c2.A);
			return color;
		}

		public static Color ColorLerpLch(float x, float x0, float x1, Color c1, Color c2) {
			Lch lch1 = ColorH.RgbToLch(c1);
			Lch lch2 = ColorH.RgbToLch(c2);

			Lch lch3 = new Lch {
				L = Linear(x, x0, x1, (float) lch1.L, (float) lch2.L),
				C = Linear(x, x0, x1, (float) lch1.C, (float) lch2.C),
				H = Linear(x, x0, x1, (float) lch1.H, (float) lch2.H)
			};

			IRgb myRgb = lch3.ToRgb();

			byte a = (byte) Linear(x, x0, x1, c1.A, c2.A);
			return new Color((byte) myRgb.R, (byte) myRgb.G, (byte) myRgb.B, a);
		}
	}
}