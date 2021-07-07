#version 460

uniform sampler2D inputTexture;
uniform float time;

uniform float decayFactor = 0.98;
uniform float randomFactor = 0.001;

float random(vec2 st) {
	return fract(sin(dot(st, vec2(12.9898, 78.233))) * 43758.5453123);
}

void main() {
	vec4 color = texture(inputTexture, gl_TexCoord[0].xy);
	
	vec2 uv = gl_TexCoord[0].xy;
	float rand = random(uv + vec2(time, time)) * randomFactor;
	
	gl_FragColor.rgb = color.rgb;
	gl_FragColor.a = (color.a * (decayFactor - rand));
}
