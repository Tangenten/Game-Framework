#version 460

uniform float time;
uniform float strength = 64.0;

void main() {
	vec2 uv = gl_TexCoord[0].xy;
	
	float x = (uv.x + 4.0 ) * (uv.y + 4.0 ) * (time * 10.0);
	vec4 grain = vec4(mod((mod(x, 13.0) + 1.0) * (mod(x, 123.0) + 1.0), 0.01) - 0.005) * strength;
	
	//grain = 1.0 - grain;

	gl_FragColor += grain;
}
