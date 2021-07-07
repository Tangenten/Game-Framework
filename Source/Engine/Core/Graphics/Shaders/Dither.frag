#version 460

uniform sampler2D inputTexture;
uniform sampler2D ditherTexture;
uniform vec2 ditherTextureResolution;
uniform float ditherTextureScaling = 4.0;

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

void main() {
	vec4 inputTextureColor = texture(inputTexture, gl_TexCoord[0].xy);
	float inputTextureColorBrightness = luma(inputTextureColor);
	
	vec2 ditherTextureUV;
	ditherTextureUV.x = int(mod(gl_FragCoord.x / ditherTextureScaling, ditherTextureResolution.x)) / ditherTextureResolution.x;
	ditherTextureUV.y = int(mod(gl_FragCoord.y / ditherTextureScaling, ditherTextureResolution.y)) / ditherTextureResolution.y;
	vec4 ditherTextureColor = texture(ditherTexture, ditherTextureUV);
	float ditherTextureColorBrightness = ditherTextureColor.r;
	
	float finalDither = inputTextureColorBrightness < ditherTextureColorBrightness ? 0.0 : 1.0;
	gl_FragColor = vec4(inputTextureColor.rgb * finalDither, inputTextureColor.a);
}
