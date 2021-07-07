using System;
using System.Collections.Generic;
using System.Linq;
using JM.LinqFaster;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public static class VertexH {
		public static Vertex[] GenerateSquareVertices(float sideLength, Color color) {
			Vertex[] vertices = new Vertex[4];
			vertices[0] = new Vertex(new Vector2f(0f, 0f), color);
			vertices[1] = new Vertex(new Vector2f(sideLength, 0f), color);
			vertices[2] = new Vertex(new Vector2f(sideLength, sideLength), color);
			vertices[3] = new Vertex(new Vector2f(0f, sideLength), color);

			return vertices;
		}

		public static float AreaOfVertices(in Vertex[] vertices) {
			List<Vertex> vList = vertices.ToList();
			return MathF.Abs(vList.TakeF(vList.Count - 1).SelectF((p, i) => p.Position.X * vList[i + 1].Position.Y - p.Position.Y * vList[i + 1].Position.X).SumF() / 2);
		}

		public static void TranslateVerticesToPoint(ref Vertex[] vertices, Vector2f pos) {
			Vector2f centroid = GetCenterFromVertices(vertices);
			Vector2f translate = pos - centroid;

			for (int i = 0; i < vertices.Length; i++) {
				vertices[i].Position += translate;
			}
		}

		public static Vector2f GetCenterFromVertices(in Vertex[] vertices) {
			float sumX = 0;
			float sumY = 0;

			for (int i = 0; i < vertices.Length; i++) {
				sumX += vertices[i].Position.X;
				sumY += vertices[i].Position.Y;
			}

			return new Vector2f(sumX / vertices.Length, sumY / vertices.Length);
		}
	}
}