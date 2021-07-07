using System;
using System.Threading;
using ColorMine.ColorSpaces;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public static class RandomH {
		private static ThreadLocal<Random> r = new ThreadLocal<Random>(() => new Random());

		public static float GetRandom(float min, float max) {
			return (float) (r.Value.NextDouble() * (max - min)) + min;
		}

		public static double GetRandom(double min, double max) {
			return r.Value.NextDouble() * (max - min) + min;
		}

		public static int GetRandom(int min, int max) {
			return r.Value.Next(min, max + 1);
		}

		public static uint GetRandom(uint min, uint max) {
			return (uint) r.Value.Next((int) min, (int) max);
		}

		public static short GetRandom(short min, short max) {
			return (short) r.Value.Next(min, max);
		}

		public static byte GetRandom(byte min, byte max) {
			return (byte) r.Value.Next(min, max);
		}

		public static float GetRandomBell(float min, float max, float mu = 0, float sigma = 1f) {
			float x1 = (float) (1 - r.Value.NextDouble());
			float x2 = (float) (1 - r.Value.NextDouble());

			float y1 = MathF.Sqrt(-2f * MathF.Log(x1)) * MathF.Sin(2f * MathF.PI * x2);
			float y2 = y1 * sigma + mu;
			y2 /= 3;
			y2 += 1;
			y2 /= 2;
			return y2 * (max - min) + min;
		}

		public static float GetRandomTriangular(float min, float max, float mid) {
			float u = (float) r.Value.NextDouble();

			return u < (mid - min) / (max - min)
				? min + MathF.Sqrt(u * (max - min) * (mid - min))
				: max - MathF.Sqrt((1 - u) * (max - min) * (max - mid));
		}

		public static Color GetRandomColor() {
			return new Color((byte) GetRandom(0, 255), (byte) GetRandom(0, 255), (byte) GetRandom(0, 255));
		}

		public static Color GetRandomColor(Color color, int scalar) {
			color.R += (byte) GetRandom(-scalar, scalar);
			color.G += (byte) GetRandom(-scalar, scalar);
			color.B += (byte) GetRandom(-scalar, scalar);
			return color;
		}

		public static Color GetRandomColorLuminance(float l) {
			Lch? myLch = new Lch(l, GetRandom(0, 100), GetRandom(0, 360));
			return ColorH.LchToRgb((float) myLch.L, (float) myLch.C, (float) myLch.H, 255);
		}

		public static Vector2f GetRandomVector(float xMin, float xMax, float yMin, float yMax) {
			return new Vector2f(GetRandom(xMin, xMax), GetRandom(yMin, yMax));
		}
	}
}