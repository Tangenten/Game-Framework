using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using JM.LinqFaster;
using SFML.Graphics.Glsl;
using SFML.System;

namespace Helpers {
	public static class VectorH {
		public static Vector2f GetTopLeftOfVertices(in Vector2f[] vertices) {
			float minX = float.MaxValue;
			float minY = float.MaxValue;

			for (int i = 0; i < vertices.Length; i++) {
				if (vertices[i].X <= minX && vertices[i].Y <= minY) {
					minX = vertices[i].X;
					minY = vertices[i].Y;
				}
			}

			return new Vector2f(minX, minY);
		}

		public static void TranslateToOrigin(ref Vector2f[] vertices, Vector2f origin) {
			Vector2f topLeft = GetTopLeftOfVertices(vertices) + origin;

			for (int i = 0; i < vertices.Length; i++) {
				vertices[i] -= topLeft;
			}
		}

		public static float DotProduct(this Vector2f v1, Vector2f v2) {
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		public static Vector2f RotatePointAroundPoint(Vector2f point, Vector2f origin, float radians) {
			float rotX = MathF.Cos(radians) * (point.X - origin.X) - MathF.Sin(radians) * (point.Y - origin.Y) + origin.X;
			float rotY = MathF.Sin(radians) * (point.X - origin.X) + MathF.Cos(radians) * (point.Y - origin.Y) + origin.Y;
			return new Vector2f(rotX, rotY);
		}

		public static float AreaOfVertices(in Vector2f[] vertices) {
			List<Vector2f> vList = vertices.ToList();
			return MathF.Abs(vList.TakeF(vList.Count - 1).SelectF((p, i) => p.X * vList[i + 1].Y - p.Y * vList[i + 1].X).SumF() / 2);
		}

		public static Vector2f RadianToVector(float radians) {
			return new Vector2f(MathF.Cos(radians), MathF.Sin(radians));
		}

		public static float VectorToRadian(Vector2f v) {
			return MathF.Atan2(v.Y, v.X);
		}

		public static float VectorToDegree(Vector2f v) {
			return MathF.Atan2(v.Y, v.X) * (180f / MathF.PI);
		}

		public static Vector2f DegreeToVector(float degree) {
			float radian = degree * (MathF.PI / 180f);
			return new Vector2f(MathF.Cos(radian), MathF.Sin(radian));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(this Vector2f v) {
			return MathF.Sqrt(v.X * v.X + v.Y * v.Y);
		}

		public static float RadianBetweenPoints(Vector2f v1, Vector2f v2) {
			return MathF.Atan2(v2.Y - v1.Y, v2.X - v1.X);
		}

		public static Vector2f VectorBetweenTwoPoints(Vector2f v1, Vector2f v2) {
			return v2 - v1;
		}

		public static Vector2f NormalVectorBetweenTwoPoints(Vector2f v1, Vector2f v2) {
			return NormalizeVector(v2 - v1);
		}

		public static Vector2f CentroidFromVertices(in Vector2f[] vertices) {
			float sumX = 0;
			float sumY = 0;

			for (int i = 0; i < vertices.Length; i++) {
				sumX += vertices[i].X;
				sumY += vertices[i].Y;
			}

			return new Vector2f(sumX / vertices.Length, sumY / vertices.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceBetweenPoints(Vector2f v1, Vector2f v2) {
			return MathF.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceBetweenPoints(Vector3f v1, Vector3f v2) {
			return MathF.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y) + (v2.Z - v1.Z) * (v2.Z - v1.Z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2f NormalizeVector(Vector2f v) {
			float length = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
			return new Vector2f(v.X / length, v.Y / length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2f FastNormalizeVector(Vector2f v) {
			float inversedMagnitude = MathH.FastInvSqrt(Magnitude(v));
			return v * inversedMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Normalize(ref this Vector2f v) {
			if (v.X == 0f && v.Y == 0f) {
				return;
			}

			float length = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
			v.X /= length;
			v.Y /= length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FastNormalize(ref this Vector2f v) {
			float inversedMagnitude = MathH.FastInvSqrt(Magnitude(v));
			v *= inversedMagnitude;
		}

		public static Vector2f LineNormal(Vector2f lineStart, Vector2f lineEnd) {
			float dx = lineEnd.X - lineStart.X;
			float dy = lineEnd.Y - lineStart.Y;

			return new Vector2f(-dy, dx);
		}

		public static bool PointInsideVertices(Vector2f point, in Vector2f[] poly) {
			float x = point.X;
			float y = point.Y;
			int n = poly.Length;
			bool inside = false;
			bool includeEdges = true;

			float p1X = poly[0].X;
			float p1Y = poly[0].Y;
			for (int i = 1; i < n + 1; i++) {
				float p2X = poly[i % n].X;
				float p2Y = poly[i % n].Y;

				if (p1Y == p2Y) {
					if (y == p1Y) {
						if (MathF.Min(p1X, p2X) <= x && x <= MathF.Max(p1X, p2X)) {
							inside = includeEdges;
							break;
						}

						if (x < MathF.Min(p1X, p2X)) inside = !inside;
					}
				} else {
					if (MathF.Min(p1Y, p2Y) <= y && y <= MathF.Max(p1Y, p2Y)) {
						float xinters = (y - p1Y) * (p2X - p1X) / (p2Y - p1Y) + p1X;
						if (x == xinters) inside = includeEdges;

						if (x < xinters) inside = !inside;
					}
				}

				p1X = p2X;
				p1Y = p2Y;
			}

			return inside;
		}

		public static Vector2f LineIntersection(in Vector2f[] line1, in Vector2f[] line2) {
			float Slope(Vector2f p1, Vector2f p2) {
				if (p2.X == p1.X)
					return p2.Y - p1.Y;
				return (p2.Y - p1.Y) / (p2.X - p1.X);
			}

			float YIntercept(float slope, Vector2f p1) {
				return p1.Y - 1f * slope * p1.X;
			}

			float m1 = Slope(line1[0], line1[1]);
			float b1 = YIntercept(m1, line1[0]);
			float m2 = Slope(line2[0], line2[1]);
			float b2 = YIntercept(m2, line2[0]);

			float x = 0;
			if (m1 == m2)
				x = b2 - b1;
			else
				x = (b2 - b1) / (m1 - m2);

			float y = m1 * x + b1;

			return new Vector2f(x, y);
		}

		public static Vector2f? LineSegmentIntersection(in Vector2f[] line1, in Vector2f[] line2) {
			Vector2f intersectionPoint = LineIntersection(line1, line2);

			if (MathF.Min(line1[0].X, line1[1].X) - 0.001 <= intersectionPoint.X && intersectionPoint.X <= MathF.Max(line1[0].X, line1[1].X) + 0.001)
				if (MathF.Min(line1[0].Y, line1[1].Y) - 0.001 <= intersectionPoint.Y && intersectionPoint.Y <= MathF.Max(line1[0].Y, line1[1].Y) + 0.001)
					if (MathF.Min(line2[0].X, line2[1].X) - 0.001 <= intersectionPoint.X && intersectionPoint.X <= MathF.Max(line2[0].X, line2[1].X) + 0.001)
						if (MathF.Min(line2[0].Y, line2[1].Y) - 0.001 <= intersectionPoint.Y && intersectionPoint.Y <= MathF.Max(line2[0].Y, line2[1].Y) + 0.001)
							return intersectionPoint;

			return null;
		}

		public static Vector2f FloatDivide(this Vector2u left, Vector2u right) {
			Vector2f v1 = (Vector2f) left;
			Vector2f v2 = (Vector2f) right;
			return new Vector2f(v1.X / v2.X, v1.Y / v2.Y);
		}

		public static Vector2f Divide(this Vector2f v1, Vector2f v2) {
			return new Vector2f(v1.X / v2.X, v1.Y / v2.Y);
		}

		public static Vector2f Multiply(this Vector2f v1, Vector2f v2) {
			return new Vector2f(v1.X * v2.X, v1.Y * v2.Y);
		}

		public static Vec2 FlipYUV(this Vec2 vec2) {
			Vec2 v = new Vec2(vec2.X, vec2.Y);
			v.Y = MathF.Abs(v.Y - 1f);
			return v;
		}

		public static Vec2 FlipY(this Vec2 vec2, float yResolution) {
			Vec2 v = new Vec2(vec2.X, vec2.Y);
			v.Y = MathF.Abs(v.Y - yResolution);
			return v;
		}

		public static Vector2f FlipYUV(this Vector2f vec2) {
			Vector2f v = new Vector2f(vec2.X, vec2.Y);
			v.Y = MathF.Abs(v.Y - 1f);
			return v;
		}

		public static Vector2f FlipY(this Vector2f vec2, float yResolution) {
			Vector2f v = new Vector2f(vec2.X, vec2.Y);
			v.Y = MathF.Abs(v.Y - yResolution);
			return v;
		}

		public static Vector3f FlipYUV(this Vector3f vec3) {
			Vector3f v = new Vector3f(vec3.X, vec3.Y, vec3.Z);
			v.Y = MathF.Abs(v.Y - 1f);
			return v;
		}

		public static Vector2 ToVector2(this Vector2f vector) {
			return new Vector2(vector.X, vector.Y);
		}

		public static Vector2 ToVector2(this Vector2i vector) {
			return new Vector2(vector.X, vector.Y);
		}

		public static Vector2 ToVector2(this Vector2u vector) {
			return new Vector2(vector.X, vector.Y);
		}

		public static Vector2f ToVector2F(this Vector3f v) {
			return new Vector2f(v.X, v.Y);
		}

		public static Vector3f ToVector3F(this Vector2f v) {
			return new Vector3f(v.X, v.Y, 0f);
		}

		public static Vector2f GetNormal(this Vector2f v) {
			return new Vector2f(-v.Y, v.X);
		}

		public static Vec4 ToVec4(this Vector4 vector4) {
			return new Vec4(vector4.X, vector4.Y, vector4.Z, vector4.W);
		}

		public static Vec3 ToVec3(this Vector3 vector3) {
			return new Vec3(vector3.X, vector3.Y, vector3.Z);
		}

		public static Vec2 ToVec2(this Vector2 vector2) {
			return new Vec2(vector2.X, vector2.Y);
		}

		public static Vector3f ToVector3f(this Vector3 vector3) {
			return new Vector3f(vector3.X, vector3.Y, vector3.Z);
		}

		public static Vector2f ToVector2f(this Vector2 vector2) {
			return new Vector2f(vector2.X, vector2.Y);
		}

		public static Vector2i ToVector2i(this Vector2 vector2) {
			return new Vector2i((int) vector2.X, (int) vector2.Y);
		}

		public static Vector2u ToVector2u(this Vector2 vector2) {
			return new Vector2u((uint) vector2.X, (uint) vector2.Y);
		}

		public static void Normalize(this Vector2 v) {
			v /= v.Length();
		}

		public static Vector2 GetNormalized(this Vector2 v) {
			return new Vector2(v.X, v.Y) / v.Length();
		}
	}
}