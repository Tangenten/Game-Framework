#version 460

uniform sampler2D inputTexture;
uniform vec2 outputResolution;
uniform float time;

uniform float decayFactor = 0.5;
uniform float randomFactor = 0.5;
uniform float luminanceThreshold = 0.5;

float random(vec2 st) {
	return fract(sin(dot(st, vec2(12.9898, 78.233))) * 43758.5453123);
}

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
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
	
	int count = 0;
	if(luma(color) > luminanceThreshold){
		count++;
	}
	if(luma(pixelLeft) > luminanceThreshold){
		count++;
	}
	if(luma(pixelRight) > luminanceThreshold){
		count++;
	}
	if(luma(pixelUp) > luminanceThreshold){
		count++;
	}
	if(luma(pixelDown) > luminanceThreshold){
		count++;
	}
	if(luma(pixelUpLeft) > luminanceThreshold){
		count++;
	}
	if(luma(pixelUpRight) > luminanceThreshold){
		count++;
	}
	if(luma(pixelDownLeft) > luminanceThreshold){
		count++;
	}
	if(luma(pixelDownRight) > luminanceThreshold){
		count++;
	}
	
	if(count == 0 || count == 9){
		gl_FragColor = color;
	}else{
		float rand = random(uv + vec2(time, time)) * randomFactor;
		
		gl_FragColor.rgb = color.rgb;
		gl_FragColor.a = (color.a * (decayFactor - rand));
	}
}
