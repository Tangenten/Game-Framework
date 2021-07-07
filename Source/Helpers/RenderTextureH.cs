using System;
using OpenTK.Graphics.OpenGL4;
using SFML.Graphics;
using SFML.System;

namespace Helpers {
	public static class RenderTextureH {
		public static void Clear(this RenderTexture renderTexture, Color color, RenderStates renderStates) {
			RectangleShape rectangleShape = new RectangleShape(renderTexture.GetView().Size);
			rectangleShape.Origin = rectangleShape.Size / 2f;
			rectangleShape.Position = renderTexture.GetView().Center;
			rectangleShape.Rotation = renderTexture.GetView().Rotation;
			rectangleShape.OutlineThickness = 0f;
			rectangleShape.FillColor = color;

			renderTexture.Draw(rectangleShape, renderStates);
		}

		public static void Draw(this RenderTexture renderTextureDestination, RenderTexture renderTextureSource, BlendMode? blendMode = null, Shader shader = null, FloatRect? drawRect = null) {
			Sprite s = new Sprite(renderTextureSource.Texture);

			if (drawRect == null) {
				s.Position = new Vector2f(0f, 0f);
				s.Scale = renderTextureDestination.Size.FloatDivide(renderTextureSource.Size);
			} else {
				s.Position = new Vector2f(drawRect.Value.Left, drawRect.Value.Top);
				s.Scale = renderTextureDestination.Size.FloatDivide(new Vector2u((uint) drawRect.Value.Width, (uint) drawRect.Value.Height));
			}

			blendMode ??= BlendMode.Alpha;

			RenderStates renderStates = new RenderStates(blendMode.Value, Transform.Identity, null, shader);

			renderTextureDestination.Draw(s, renderStates);
		}

		public static void Draw(this RenderTexture renderTextureDestination, Texture textureSource, BlendMode? blendMode = null, Shader shader = null, FloatRect? drawRect = null) {
			Sprite s = new Sprite(textureSource);

			if (drawRect == null) {
				s.Position = new Vector2f(0f, 0f);
				s.Scale = renderTextureDestination.Size.FloatDivide(textureSource.Size);
			} else {
				s.Position = new Vector2f(drawRect.Value.Left, drawRect.Value.Top);
				s.Scale = renderTextureDestination.Size.FloatDivide(new Vector2u((uint) drawRect.Value.Width, (uint) drawRect.Value.Height));
			}

			blendMode ??= BlendMode.Alpha;

			RenderStates renderStates = new RenderStates(blendMode.Value, Transform.Identity, null, shader);

			renderTextureDestination.Draw(s, renderStates);
		}

		public static Vector2f GetRelativePosition(this RenderTexture renderTextureA, RenderTexture renderTextureB, Vector2f positionB) {
			Vector2f uv = positionB.Divide((Vector2f) renderTextureB.Size);

			return uv.Multiply((Vector2f) renderTextureA.Size);
		}

		public static Vector2f GetRelativePosition(this RenderTexture renderTextureA, Vector2f resolutionB, Vector2f positionB) {
			Vector2f uv = positionB.Divide(resolutionB);

			return uv.Multiply((Vector2f) renderTextureA.Size);
		}

		public static Vector2f UvPosition(this RenderTexture renderTextureA, Vector2f position) {
			position = position.Divide((Vector2f) renderTextureA.Size);
			return position;
		}

		public static Vector2f FlipXAxis(this RenderTexture renderTextureA, Vector2f position) {
			float x = position.X - renderTextureA.Size.X;
			position.X = MathF.Abs(x);
			return position;
		}

		public static Vector2f FlipYAxis(this RenderTexture renderTextureA, Vector2f position) {
			float y = position.Y - renderTextureA.Size.Y;
			position.Y = MathF.Abs(y);
			return position;
		}

		public static void TextureRgba32F(this RenderTexture renderTexture, in float[] initialValues) {
			Texture.Bind(renderTexture.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba32f,
				(int) renderTexture.Size.X,
				(int) renderTexture.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				initialValues);
			Texture.Bind(null);
		}

		public static void TextureRgba16F(this RenderTexture renderTexture, in float[] initialValues) {
			Texture.Bind(renderTexture.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba16f,
				(int) renderTexture.Size.X,
				(int) renderTexture.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				initialValues);
			Texture.Bind(null);
		}

		public static void TextureRgba8(this RenderTexture renderTexture, in float[] initialValues) {
			Texture.Bind(renderTexture.Texture);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba8,
				(int) renderTexture.Size.X,
				(int) renderTexture.Size.Y,
				0,
				PixelFormat.Rgba,
				PixelType.Float,
				initialValues);
			Texture.Bind(null);
		}
	}
}