using System;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public class CollisionH {
		public struct CollisionData {
			public float collisionDepth;
			public Vector2f collisionPosition;
			public Vector2f collisionNormal;
		}

		public bool PointCircleCollision(Vector2f pPos, Vector2f cPos, float cRad) {
			float distanceBetweenPoints = VectorH.DistanceBetweenPoints(cPos, pPos);
			return distanceBetweenPoints <= cRad;
		}

		public bool PointCircleCollision(Vector2f pPos, Vector2f cPos, float cRad, out CollisionData data) {
			float distanceBetweenPoints = VectorH.DistanceBetweenPoints(cPos, pPos);

			Vector2f angleBetweenCircles = cPos - pPos;
			angleBetweenCircles.X = MathF.Abs(angleBetweenCircles.X);
			angleBetweenCircles.Y = MathF.Abs(angleBetweenCircles.Y);
			angleBetweenCircles.Normalize();

			data.collisionNormal = angleBetweenCircles;
			data.collisionPosition = cPos + angleBetweenCircles * cRad;
			data.collisionDepth = MathF.Abs(distanceBetweenPoints - cRad);

			return distanceBetweenPoints <= cRad;
		}

		public bool PointAABBCollision(Vector2f pPos, FloatRect rect) {
			return pPos.X > rect.Left && pPos.X < rect.Left + rect.Width && pPos.Y > rect.Top && pPos.Y < rect.Top + rect.Height;
		}

		public bool PointAABBCollision(Vector2f pPos, FloatRect rect, out CollisionData data) {
			Vector2f rectCenter = rect.Center();
			float dx = MathF.Max(MathF.Abs(pPos.X - rectCenter.X) - rect.Width / 2, 0);
			float dy = MathF.Max(MathF.Abs(pPos.Y - rectCenter.Y) - rect.Height / 2, 0);
			Vector2f collidePoint = new Vector2f(dx, dy);
			data.collisionPosition = collidePoint;

			float distanceBetweenRectAndCollide = VectorH.DistanceBetweenPoints(rectCenter, collidePoint);
			float distanceBetweenRectAndPoint = VectorH.DistanceBetweenPoints(rectCenter, pPos);
			data.collisionDepth = MathF.Abs(distanceBetweenRectAndPoint - distanceBetweenRectAndCollide);

			if (collidePoint.X == rect.Left) {
				data.collisionNormal = new Vector2f(-1f, 0);
			} else if (collidePoint.X == rect.Left + rect.Width) {
				data.collisionNormal = new Vector2f(1f, 0);
			} else if (collidePoint.Y == rect.Top) {
				data.collisionNormal = new Vector2f(0f, -1f);
			} else {
				data.collisionNormal = new Vector2f(0f, 1f);
			}

			return pPos.X > rect.Left && pPos.X < rect.Left + rect.Width && pPos.Y > rect.Top && pPos.Y < rect.Top + rect.Height;
		}

		public bool CircleCircleCollision(Vector2f c1Pos, float c1Rad, Vector2f c2Pos, float c2Rad) {
			float distanceBetweenCircles = VectorH.DistanceBetweenPoints(c1Pos, c2Pos);
			float radiansPutTogether = c1Rad + c2Rad;

			return distanceBetweenCircles <= radiansPutTogether;
		}

		public bool CircleCircleCollision(Vector2f c1Pos, float c1Rad, Vector2f c2Pos, float c2Rad, out CollisionData data) {
			float distanceBetweenCircles = VectorH.DistanceBetweenPoints(c1Pos, c2Pos);
			float radiansPutTogether = c1Rad + c2Rad;

			Vector2f angleBetweenCircles = c1Pos - c2Pos;
			angleBetweenCircles.X = MathF.Abs(angleBetweenCircles.X);
			angleBetweenCircles.Y = MathF.Abs(angleBetweenCircles.Y);
			angleBetweenCircles.Normalize();

			data.collisionNormal = angleBetweenCircles;
			data.collisionPosition = c1Pos + angleBetweenCircles * c1Rad;
			data.collisionDepth = MathF.Abs(distanceBetweenCircles - radiansPutTogether);

			return distanceBetweenCircles <= radiansPutTogether;
		}

		public bool AABBAABBCollision(FloatRect rect1, FloatRect rect2) {
			return rect1.Left < rect2.Left + rect2.Width && rect1.Left + rect1.Width > rect2.Left && rect1.Top < rect2.Top + rect2.Height && rect1.Top + rect1.Height > rect2.Top;
		}
	}
}