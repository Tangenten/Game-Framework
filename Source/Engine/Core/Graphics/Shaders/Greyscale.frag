#version 460

uniform sampler2D inputTexture;

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

void main() {
	vec4 color = texture(inputTexture, gl_TexCoord[0].xy);
	
	float luminance = luma(color);
	
	gl_FragColor = vec4(vec3(luminance), color.a);
}
