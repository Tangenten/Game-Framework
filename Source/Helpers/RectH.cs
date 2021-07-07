using System;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public static class Rect {
		public static FloatRect Abs(this FloatRect rec) {
			if (rec.Width < 0) {
				rec.Left += rec.Width;
				rec.Width *= -1;
			}

			if (rec.Height < 0) {
				rec.Top += rec.Height;
				rec.Height *= -1;
			}

			return rec;
		}

		public static bool Intersects(this FloatRect rectA, FloatRect rectB, out Vector2f depth) {
			//source: XNA Platformer Example

			// Calculate half sizes.
			float halfWidthA = rectA.Width / 2.0f;
			float halfHeightA = rectA.Height / 2.0f;
			float halfWidthB = rectB.Width / 2.0f;
			float halfHeightB = rectB.Height / 2.0f;

			// Calculate centers.
			Vector2f centerA = new Vector2f(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
			Vector2f centerB = new Vector2f(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

			// Calculate current and minimum-non-intersecting distances between centers.
			float distanceX = centerA.X - centerB.X;
			float distanceY = centerA.Y - centerB.Y;
			float minDistanceX = halfWidthA + halfWidthB;
			float minDistanceY = halfHeightA + halfHeightB;

			// If we are not intersecting at all, return (0, 0).
			if (MathF.Abs(distanceX) >= minDistanceX || MathF.Abs(distanceY) >= minDistanceY) {
				depth = new Vector2f();
				return false;
			}

			// Calculate and return intersection depths.
			float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
			float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

			depth = new Vector2f(depthX, depthY);
			return true;
		}

		public static FloatRect AABB(Vector2f[] vectors) {
			float minX, minY, maxX, maxY;
			minX = minY = float.MaxValue;
			maxX = maxY = float.MinValue;

			for (int i = 0; i < vectors.Length; i++) {
				minX = MathF.Min(minX, vectors[i].X);
				maxX = MathF.Max(maxX, vectors[i].X);

				minY = MathF.Min(minY, vectors[i].Y);
				maxY = MathF.Max(maxY, vectors[i].Y);
			}

			return new FloatRect(minX, minY, maxX - minX, maxY - minY);
		}

		public static float OverlapRatio(this FloatRect recA, FloatRect recB) {
			if (recB.Intersects(recA, out FloatRect overlap)) {
				return MathF.Min(1, overlap.Width * overlap.Height / recA.Width / recA.Height);
			}

			return 0;
		}

		public static bool Contains(this FloatRect rec, Vector2f v) {
			return rec.Contains(v.X, v.Y);
		}

		public static bool Contains(this FloatRect a, FloatRect b) {
			return
				b.Left > a.Left &&
				b.Right() < a.Right() &&
				b.Top > a.Top &&
				b.Bottom() < a.Bottom();
		}

		public static Vector2f Position(this FloatRect rec) {
			return new Vector2f(rec.Left, rec.Top);
		}

		public static Vector2f Size(this FloatRect rec) {
			return new Vector2f(rec.Width, rec.Height);
		}

		public static float Bottom(this FloatRect rec) {
			return rec.Top + rec.Height;
		}

		public static float Right(this FloatRect rec) {
			return rec.Left + rec.Width;
		}

		public static Vector2f Center(this FloatRect rec) {
			return new Vector2f(rec.Left + rec.Width / 2, rec.Top + rec.Height / 2);
		}
	}
}