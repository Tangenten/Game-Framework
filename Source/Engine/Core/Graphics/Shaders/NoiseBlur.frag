#version 460

uniform sampler2D inputTexture;

float rand(vec2 co){
	return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

uniform float length = 0.05;
uniform float time;

void main() {
	vec2 uv = gl_TexCoord[0].xy;
	
	uv.x += rand(uv + vec2(time)) * length;
	uv.y += rand(uv + vec2(time)) * length;
	
	vec4 color = texture(inputTexture, gl_TexCoord[0].xy);
	
	gl_FragColor = color;
}
