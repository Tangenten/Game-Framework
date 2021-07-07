#version 460

uniform sampler2D transitionTexture;
uniform float transitionThreshold;

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

void main() {
	float step = step(luma(texture(transitionTexture, gl_TexCoord[0].xy)), transitionThreshold);
	
	gl_FragColor.a = step;
}
