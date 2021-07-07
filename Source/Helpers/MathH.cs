using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Helpers {
	public static class MathH {
		public static int Mod(int x, int m) {
			if (m == 0) {
				return m;
			}

			int r = x % m;
			return r < 0 ? r + m : r;
		}

		public static float Mod(float x, float m) {
			if (m == 0) {
				return m;
			}

			float r = x % m;
			return r < 0 ? r + m : r;
		}

		public static int ModRange(int x, int min, int max) {
			return ((x - min) % (max - min) + (max - min)) % (max - min) + min;
		}

		public static float ModRange(float x, float min, float max) {
			return ((x - min) % (max - min) + (max - min)) % (max - min) + min;
		}

		public static float DegreeToRadian(float degree) {
			return MathF.PI * degree / 180f;
		}

		public static float RadianToDegree(float radian) {
			return 180f / MathF.PI * radian;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float FastInvSqrt(float x) {
			FloatIntUnion union = new FloatIntUnion {x = x};
			union.i = 0x5f3759df - (union.i >> 1);
			x = union.x;
			x = x * (1.5f - 0.5f * x * x * x);
			return x;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntUnion {
			[FieldOffset(0)] public float x;

			[FieldOffset(0)] public int i;
		}
	}
}