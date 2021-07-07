#version 460

uniform sampler2D inputTexture;
uniform sampler2D displacementTexture;

void main() {
	vec4 displacementPixel = texture(displacementTexture, gl_TexCoord[0].xy);
	
	vec2 uv = gl_TexCoord[0].xy;
	uv.x += (displacementPixel.r * 2.0 - 1.0) * 0.025 * displacementPixel.a;
	uv.y -= (displacementPixel.g * 2.0 - 1.0) * 0.025 * displacementPixel.a;
	
	vec4 color = texture(inputTexture, uv);
	
	gl_FragColor = color;
}
