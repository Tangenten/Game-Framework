#version 460

uniform sampler2D inputTexture;

uniform float threshold = 1.0;
uniform float strength = 1.0;

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

void main(){
	vec4 color = texture(inputTexture, gl_TexCoord[0].xy);
	float brightness = luma(color);
	float stepper = step(brightness, threshold);
	
	color = color * stepper;
	gl_FragColor = color;
}
