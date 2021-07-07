#version 460

uniform sampler2D inputTexture;
uniform vec2 outputResolution;
uniform float time;

uniform float luminanceThreshold = 0;
uniform float friction = 0.98;

float random(vec2 uv) {
	return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453123);
}

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114)) * color.a;
}

void main() {
	vec2 uv = gl_TexCoord[0].xy;
	vec4 color = texture(inputTexture, gl_TexCoord[0].xy);
	
	float xPos = uv.x;
	float yPos = uv.y;
	float xStep = 1.0 / outputResolution.x;
	float yStep = 1.0 / outputResolution.y;
	
	vec4 pixelLeft = texture(inputTexture, vec2(xPos - xStep, yPos));
	vec4 pixelRight = texture(inputTexture, vec2(xPos + xStep, yPos));
	vec4 pixelUp = texture(inputTexture, vec2(xPos, yPos - yStep));
	vec4 pixelDown = texture(inputTexture, vec2(xPos, yPos + yStep));
	
	vec4 pixelUpLeft = texture(inputTexture, vec2(xPos - xStep, yPos - yStep));
	vec4 pixelUpRight = texture(inputTexture, vec2(xPos + xStep, yPos - yStep));
	vec4 pixelDownLeft = texture(inputTexture, vec2(xPos - xStep, yPos + yStep));
	vec4 pixelDownRight = texture(inputTexture, vec2(xPos + xStep, yPos + yStep));
	
	int randomChoice = int(random(uv + vec2(time, time)) * 8.0);
	
	if(randomChoice == 0 && luminance(pixelLeft) > luminanceThreshold){
		pixelLeft.a = pixelLeft.a * friction;
		gl_FragColor = pixelLeft;
	}
	else if(randomChoice == 1 && luma(pixelRight) > luminanceThreshold){
		pixelRight.a = pixelRight.a * friction;
		gl_FragColor = pixelRight;
	}
	else if(randomChoice == 2 && luma(pixelUp) > luminanceThreshold){
		pixelUp.a = pixelUp.a * friction;
		gl_FragColor = pixelUp;
	}
	else if(randomChoice == 3 && luma(pixelDown) > luminanceThreshold){
		pixelDown.a = pixelDown.a * friction;
		gl_FragColor = pixelDown;
	}
	else if(randomChoice == 4 && luma(pixelUpLeft) > luminanceThreshold){
		pixelUpLeft.a = pixelUpLeft.a * friction;
		gl_FragColor = pixelUpLeft;
	}
	else if(randomChoice == 5 && luma(pixelUpRight) > luminanceThreshold){
		pixelUpRight.a = pixelUpRight.a * friction;
		gl_FragColor = pixelUpRight;
	}
	else if(randomChoice == 6 && luma(pixelDownLeft) > luminanceThreshold){
		pixelDownLeft.a = pixelDownLeft.a * friction;
		gl_FragColor = pixelDownLeft;
	}
	else if(randomChoice == 7 && luma(pixelDownRight) > luminanceThreshold){
		pixelDownRight.a = pixelDownRight.a * friction;
		gl_FragColor = pixelDownRight;
	}else{
		gl_FragColor = color;
	}
}
