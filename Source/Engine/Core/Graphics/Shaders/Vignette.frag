#version 460

uniform float strength = 16.0;
uniform float length = 0.25;

void main() {
	vec2 uv = gl_TexCoord[0].xy;
	
	uv *=  1.0 - uv.yx;   //vec2(1.0)- uv.yx; -> 1.-u.yx; Thanks FabriceNeyret !
	
	float vig = uv.x * uv.y * strength; // multiply with sth for intensity
	vig = pow(vig, length); // change pow for modifying the extend of the vignette
	
	gl_FragColor.a = 1.0 - vig;
}
