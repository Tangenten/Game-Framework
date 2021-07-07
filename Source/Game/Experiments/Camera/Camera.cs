using System;
using System.Threading.Tasks;
using DotnetNoise;
using Helpers;
using SFML.Graphics;
using SFML.System;
using TangentEngine;

namespace Camera {
	public class HeightM : RenTexSprite {
		public static event Action? heightMapBuilt;

		private FastNoise noise;

		[InspectNumerical(1, 2048, 1, "SetNoise")]
		private int noiseSeed = 1;

		[InspectNumerical(0.001, 16, 0.001, "CreateHeights")]
		private float noiseExponentation = 1.3f;

		[InspectNumerical(0.000001, 0.1, 0.0001, "CreateHeights")]
		private float noiseFrequency = 0.01f;

		[InspectNumerical(0.001, 4, 0.001, "CreateHeights")]
		private float noiseGain = 0.27f;

		[InspectNumerical(0.001, 16, 0.001, "CreateHeights")]
		private float noiseHeight = 1f;

		[InspectNumerical(0.0001, 16, 0.001, "CreateHeights")]
		private float noiseLacunarity = 3.1f;

		[InspectNumerical(1, 16, 1, "CreateHeights")]
		private float noiseOctaves = 4f;

		[InspectNumerical(0.001, 2, 0.001, "CreateHeights")]
		private float noiseRigid = 1f;

		[InspectNumerical(1, 512, 0.01, "CreateHeights")]
		private float noiseTerrace = 1f;

		[InspectEnum("SetNoise")] private NoiseType noiseTyper = NoiseType.SIMPLEX_FBM;

		private enum NoiseType {
			SIMPLEX_FBM,
			SIMPLEX_BILLOW
		}

		public int heightMapWidth;
		public int heightMapHeight;

		public float[,] heights;
		public float[] pixels;

		public HeightM(int width, int height) : base(width, height) {
			this.heightMapWidth = width;
			this.heightMapHeight = height;
			this.SetSmooth(true);

			this.heights = new float[this.heightMapWidth, this.heightMapHeight];
			this.pixels = new float[this.textureWidth * this.textureHeight * 4];
			this.SetNoise();

			Engine.inspector.BuildInspectionGui(this);
		}

		public void SetNoise() {
			this.noise = new FastNoise(this.noiseSeed);
			this.noise.InterpMethod = FastNoise.Interp.Hermite;

			switch (this.noiseTyper) {
				case NoiseType.SIMPLEX_FBM:
					this.noise.UsedNoiseType = FastNoise.NoiseType.SimplexFractal;
					this.noise.FractalTypeMethod = FastNoise.FractalType.Fbm;
					break;
				case NoiseType.SIMPLEX_BILLOW:
					this.noise.UsedNoiseType = FastNoise.NoiseType.SimplexFractal;
					this.noise.FractalTypeMethod = FastNoise.FractalType.Billow;
					break;
			}
		}

		public float GetSimplexFbm(float x, float y) {
			return TweenH.Linear((this.noise.GetSimplexFractal(x, y) + 1f) / 2f, 0.200f, 0.760f, 0f, 1f); // 0-1 range
		}

		public float GetSimplexBillow(float x, float y) {
			return TweenH.Linear((this.noise.GetSimplexFractal(x, y) + 1f) / 2f, 0f, 0.625f, 0f, 1f); // 0-1 range
		}

		public void CreateHeights() {
			if (this.noise.Frequency != this.noiseFrequency || this.noise.Gain != this.noiseGain || this.noise.Lacunarity != this.noiseLacunarity || this.noise.Octaves != (int) this.noiseOctaves) {
				this.noise.Frequency = this.noiseFrequency;
				this.noise.Gain = this.noiseGain;
				this.noise.Lacunarity = this.noiseLacunarity;
				this.noise.Octaves = (int) this.noiseOctaves;
			}

			Func<float, float, float> noiseAlgo = this.GetSimplexFbm;
			switch (this.noiseTyper) {
				case NoiseType.SIMPLEX_FBM:
					noiseAlgo = this.GetSimplexFbm;
					break;
				case NoiseType.SIMPLEX_BILLOW:
					noiseAlgo = this.GetSimplexBillow;
					break;
			}

			float sin = OscH.SinOsc(1, 400f, 0.05f, 0f, Engine.time.elapsedGameTime);
			float cos = OscH.SinOsc(1, 400f, 0.05f, 0.25f, Engine.time.elapsedGameTime);
			Parallel.For(0, this.heightMapWidth, i => {
				for (int j = 0; j < this.heightMapHeight; j++) {
					float noise = noiseAlgo(i + sin, j + cos);

					noise = noise * this.noiseHeight; // height multiply
					noise = (float) Math.Pow(noise, this.noiseExponentation); // exponent

					if (Math.Abs(this.noiseRigid - 1f) > 0.01f) {
						noise = 2 * (this.noiseRigid - Math.Abs(this.noiseRigid - noise)); // rigidness
					}

					if (Math.Abs(this.noiseTerrace - 1f) > 0.01f) {
						noise = (float) (Math.Round(noise * this.noiseTerrace) / this.noiseTerrace); // terrace
					}

					this.heights[i, j] = noise;
				}
			});

			heightMapBuilt?.Invoke();
			this.CreatePixels();
		}

		public float GetPixel(int x, int y) {
			return this.pixels[(y * this.textureWidth + x) * 4 + 0];
		}

		public void SetPixel(int x, int y, float val) {
			this.pixels[(y * this.textureWidth + x) * 4 + 0] = val;
			this.pixels[(y * this.textureWidth + x) * 4 + 1] = val;
			this.pixels[(y * this.textureWidth + x) * 4 + 2] = val;
			this.pixels[(y * this.textureWidth + x) * 4 + 3] = 1f;
		}

		public void CreatePixels() {
			Parallel.For(0, this.textureWidth, i => {
				for (int j = 0; j < this.textureHeight; j++) {
					this.SetPixel(i, j, this.heights[i, j]);
				}
			});

			this.UploadPixels();
		}

		public void UploadPixels() {
			this.renderTexture.TextureRgba16F(this.pixels);
			this.renderTexture.Display();
		}
	}

	public class AlbedoH : RenTexSprite {
		[InspectColor("CreateAlbedos")] public Color color0 = new Color(1, 34, 72);
		[InspectColor("CreateAlbedos")] public Color color1 = new Color(37, 97, 130);

		[InspectColor("CreateAlbedos")] public Color color2 = new Color(66, 110, 131);
		[InspectColor("CreateAlbedos")] public Color color3 = new Color(57, 188, 249);

		[InspectColor("CreateAlbedos")] public Color color4 = new Color(200, 207, 67);
		[InspectColor("CreateAlbedos")] public Color color5 = new Color(162, 189, 103);

		[InspectColor("CreateAlbedos")] public Color color6 = new Color(85, 177, 39);
		[InspectColor("CreateAlbedos")] public Color color7 = new Color(34, 81, 37);

		public float[] pixels;
		public float[,] heights;

		public AlbedoH(int width, int height, float[,] heights) : base(width, height) {
			this.pixels = new float[this.textureWidth * this.textureHeight * 4];
			this.heights = heights;

			HeightM.heightMapBuilt += this.CreateAlbedos;
		}

		public void CreateAlbedos() {
			Parallel.For(0, this.textureWidth, i => {
				Color textureColor;
				for (int j = 0; j < this.textureHeight; j++) {
					float height = this.heights[i, j];

					if (height < 0.15f) {
						textureColor = TweenH.ColorLerpRgb(height, 0f, 0.15f, this.color0, this.color1);
						this.heights[i, j] = 0.30f;
					} else if (height < 0.30f) {
						textureColor = TweenH.ColorLerpRgb(height, 0.15f, 0.30f, this.color2, this.color3);
						this.heights[i, j] = 0.30f;
					} else if (height < 0.35f) {
						textureColor = TweenH.ColorLerpRgb(height, 0.30f, 0.35f, this.color4, this.color5);
					} else if (height < 0.90f) {
						textureColor = TweenH.ColorLerpRgb(height, 0.35f, 0.85f, this.color6, this.color7);
					} else {
						textureColor = TweenH.ColorLerpRgb(height, 0.85f, 1f, this.color7, new Color(156, 156, 156, 255));
					}

					this.SetPixel(i, j, textureColor);
				}
			});

			this.UploadMap();
		}

		public void UploadMap() {
			this.renderTexture.TextureRgba8(this.pixels);
			this.renderTexture.Display();
		}

		public Color GetPixel(int x, int y) {
			Color color = new Color();
			color.R = (byte) (this.pixels[(y * this.textureWidth + x) * 4 + 0] * 255f);
			color.G = (byte) (this.pixels[(y * this.textureWidth + x) * 4 + 1] * 255f);
			color.B = (byte) (this.pixels[(y * this.textureWidth + x) * 4 + 2] * 255f);
			color.A = (byte) (this.pixels[(y * this.textureWidth + x) * 4 + 3] * 255f);
			return color;
		}

		public void SetPixel(int x, int y, Color val) {
			this.pixels[(y * this.textureWidth + x) * 4 + 0] = val.R / 255f;
			this.pixels[(y * this.textureWidth + x) * 4 + 1] = val.G / 255f;
			this.pixels[(y * this.textureWidth + x) * 4 + 2] = val.B / 255f;
			this.pixels[(y * this.textureWidth + x) * 4 + 3] = val.A / 255f;
		}
	}

	public class NormalH : RenTexSprite {
		public float[] pixels;
		public float[,] heights;

		public NormalH(int width, int height, float[,] heights) : base(width, height) {
			this.pixels = new float[this.textureWidth * this.textureHeight * 4];
			this.heights = heights;

			this.SetSmooth(true);

			//HeightM.heightMapBuilt += this.CreateNormals;
		}

		public void CreateNormals() {
			Parallel.For(1, this.textureWidth - 1, i => {
				Vector2f normalVector;
				for (int j = 1; j < this.textureHeight - 1; j++) {
					float xDelta = (this.heights[i - 1, j] - this.heights[i + 1, j] + 1) * 0.5f;
					float yDelta = (this.heights[i, j - 1] - this.heights[i, j + 1] + 1) * 0.5f;

					normalVector.X = xDelta;
					normalVector.Y = yDelta;
					normalVector.Normalize();

					this.SetPixel(i, j, normalVector);
				}
			});

			this.UploadMap();
		}

		public Vector2f GetPixel(int x, int y) {
			Vector2f vec = new Vector2f();
			vec.X = this.pixels[(y * this.textureWidth + x) * 4 + 0];
			vec.Y = this.pixels[(y * this.textureWidth + x) * 4 + 1];
			return vec;
		}

		public void SetPixel(int x, int y, Vector2f val) {
			this.pixels[(y * this.textureWidth + x) * 4 + 0] = val.X;
			this.pixels[(y * this.textureWidth + x) * 4 + 1] = val.Y;
			this.pixels[(y * this.textureWidth + x) * 4 + 2] = 1f;
			this.pixels[(y * this.textureWidth + x) * 4 + 3] = 1f;
		}

		public void UploadMap() {
			this.renderTexture.TextureRgba16F(this.pixels);
			this.renderTexture.Display();
		}
	}

	public class LightM : RenTexSprite {
		[InspectNumerical(0.01f, 1f, 0.001)] private float ambientOcculusion = 0.2f;
		[InspectNumerical(0.1f, 8f, 0.001)] private float lightCollideFalloff = 2f;
		[InspectNumerical(0.01, 1)] private float lightFalloff = 0.15f;
		[InspectNumerical(1, 1000, 1)] private float lightSteps = 24;

		[InspectColor] private Color lightColor = Color.Yellow.Divide(4f);

		private Shader lightShader;

		public LightM(int width, int height) : base(width, height) {
			this.lightShader = new Shader(null, null, Engine.assets.GetStream("Engine.Source.Game.Experiments.Camera.Assets.Light.frag"));
			this.SetSmooth(true);

			Engine.inspector.BuildInspectionGui(this);
		}

		public void Update(in RenderTexture heightMap, Vector2f mousePosUV) {
			this.lightShader.SetUniform("heightMap", heightMap.Texture);

			this.lightShader.SetUniform("lightFalloff", this.lightFalloff);
			this.lightShader.SetUniform("lightCollideFalloff", this.lightCollideFalloff);
			this.lightShader.SetUniform("lightColor", this.lightColor.ToVec4());
			this.lightShader.SetUniform("lightHeightOffGround", 0.10f);
			this.lightShader.SetUniform("lightSteps", this.lightSteps);
			this.lightShader.SetUniform("lightPositionUV", mousePosUV.FlipYUV());

			this.renderTexture.Clear(new Color(0, 0, 0, (byte) (this.ambientOcculusion * 255f)));
			this.renderTexture.Draw(heightMap, BlendMode.Alpha, this.lightShader);

			CircleShape c = new CircleShape(2f);
			c.Origin = new Vector2f(c.Radius, c.Radius);
			c.Position = mousePosUV.Multiply(this.textureSize);
			c.FillColor = Color.White;
			this.renderTexture.Draw(c, new RenderStates(BlendMode.Alpha));

			this.renderTexture.Display();
		}
	}

	public class TransformM : RenTexSprite {
		[InspectNumerical(0.000001, 32f, 0.0001)]
		public float transformHeight = 3f;

		[InspectVector2f(0f, 1f, 0.001f)] public Vector2f transformPosition = new Vector2f(0.5f, 0.5f);
		[InspectNumerical(1, 1000, 1)] private float cameraSteps = 64;

		private Shader transformShader;

		private float[,] heights;

		public TransformM(int width, int height, float[,] heights) : base(width, height) {
			this.transformShader = new Shader(null, null, Engine.assets.GetStream("Engine.Source.Game.Experiments.Camera.Assets.3DTransform.frag"));
			this.heights = heights;

			Engine.inspector.BuildInspectionGui(this);
		}

		public void Update(RenderTexture heightMap, RenderTexture colorMap) {
			this.transformShader.SetUniform("heightMap", heightMap.Texture);
			this.transformShader.SetUniform("colorMap", colorMap.Texture);
			this.transformShader.SetUniform("cameraHeight", this.transformHeight);
			this.transformShader.SetUniform("cameraSteps", this.cameraSteps);

			//float sin = OscH.SinOsc(0, 1f, 1f, 0f, (float) Engine.time.elapsedGameTime);
			//float cos = OscH.SinOsc(0, 1f, 1f, 0.5f, (float) Engine.time.elapsedGameTime);
			//this.transformPosition.X = sin;
			//this.transformPosition.Y = cos;
			this.transformShader.SetUniform("cameraPositionUV", transformPosition.FlipYUV());

			this.renderTexture.Clear();
			this.renderTexture.Draw(colorMap, BlendMode.Alpha, this.transformShader);
			this.renderTexture.Display();
		}

		public Vector2f CameraToMouseRayCast() {
			Vector2f mouseUvPosition = Engine.input.GetMouseGamePosition();
			mouseUvPosition = this.sprite.InverseTransform.TransformPoint(mouseUvPosition).Divide(this.GetTextureSize());

			Vector3f cameraUvPosition = new Vector3f(this.transformPosition.X, this.transformPosition.Y, this.transformHeight).FlipYUV();
			Vector3f groundPlaneUvPosition = new Vector3f(mouseUvPosition.X, mouseUvPosition.Y, 0f).FlipYUV();

			int heightsArrayWidth = this.heights.GetLength(0) - 1;
			int heightsArrayHeight = this.heights.GetLength(1) - 1;

			float sightDistance = VectorH.DistanceBetweenPoints(groundPlaneUvPosition, cameraUvPosition);
			float stepSize = 1f / this.cameraSteps;
			for (float i = 0; i < sightDistance + stepSize; i += stepSize) {
				Vector3f lerper = TweenH.VectorLerp(cameraUvPosition, groundPlaneUvPosition, i / sightDistance);
				float lerperHeight = lerper.Z;
				float groundHeight = this.heights[
					(int) Math.Clamp(lerper.X * heightsArrayWidth, 0, heightsArrayWidth),
					(int) Math.Clamp(lerper.Y * heightsArrayHeight, 0, heightsArrayHeight)];

				if (lerperHeight <= groundHeight) {
					return lerper.ToVector2F().FlipYUV();
				}
			}

			return new Vector2f(0, 0);
		}
	}

	public class CameraGame : GameE {
		private HeightM heightMap;
		private AlbedoH albedoMap;
		private NormalH normalMap;
		private LightM lightMap;
		private RenTexSprite combinedMap;
		private TransformM transformMap;

		public override void Start() {
			this.heightMap = new HeightM(1920 / 4, 1080 / 4);
			this.heightMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));

			this.albedoMap = new AlbedoH(1920 / 4, 1080 / 4, this.heightMap.heights);
			this.albedoMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));

			this.normalMap = new NormalH(1920 / 4, 1080 / 4, this.heightMap.heights);
			this.normalMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));

			this.lightMap = new LightM(1920 / 8, 1080 / 8);
			this.lightMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));

			this.combinedMap = new RenTexSprite(1920 / 2, 1080 / 2);
			this.combinedMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));
			this.combinedMap.SetSmooth(true);

			this.transformMap = new TransformM(1920 / 2, 1080 / 2, this.heightMap.heights);
			this.transformMap.ScaleTo(new FloatRect(0, 0, Engine.camera.gameWidth, Engine.camera.gameHeight));

			this.heightMap.CreateHeights();
		}

		public override void Update() {
			this.heightMap.CreateHeights();

			Vector2f rayPos = this.transformMap.CameraToMouseRayCast();
			//Vector2f rayPos = this.lightMap.sprite.InverseTransform.TransformPoint(Engine.input.GetMouseGamePosition()).Divide(this.lightMap.textureSize);
			this.lightMap.Update(this.heightMap.renderTexture, rayPos);

			this.combinedMap.Clear();
			this.combinedMap.DrawOntoTextureUV(new Vector2f(0f, 0f), new Vector2f(1f, 1f), this.albedoMap, new RenderStates(BlendMode.Alpha));
			this.combinedMap.DrawOntoTextureUV(new Vector2f(0f, 0f), new Vector2f(1f, 1f), this.lightMap, new RenderStates(new BlendMode(BlendMode.Factor.One, BlendMode.Factor.One, BlendMode.Equation.Add, BlendMode.Factor.One, BlendMode.Factor.OneMinusSrcAlpha, BlendMode.Equation.Subtract)));

			this.transformMap.Update(this.heightMap.renderTexture, this.combinedMap.renderTexture);

			Engine.graphics.Draw(this.transformMap);
		}

		public override void Stop() { }
	}
}