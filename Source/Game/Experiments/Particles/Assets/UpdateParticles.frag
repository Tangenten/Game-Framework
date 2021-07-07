#version 460

float map(float value, float min1, float max1, float min2, float max2) {
	return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

uniform sampler2D texture;
uniform vec2 resolution;
uniform vec2 mouse;

uniform float posStrength;
uniform float velStrength;

void main(){
	vec4 texColor = texture2D(texture, gl_TexCoord[0].xy);
	
	float xPos = texColor.r;
	float yPos = texColor.g;
	float xVel = texColor.b;
	float yVel = texColor.a;
	
	vec2 pos = vec2(xPos, yPos);
	vec2 vel = vec2(xVel, yVel);
	
	vec2 middle = mouse;
	vec2 dirToMiddle = middle - pos;
	float lengthToMiddle = length(dirToMiddle);
	dirToMiddle = normalize(dirToMiddle);
	
	float lengthScalar = clamp(map(lengthToMiddle, 4000.0, 0.0, 0.0, 2.0), 0.0, 2.0);
	vec2 newVel = vel + ((dirToMiddle * velStrength));
	newVel *= 0.995;
	vec2 newPos = pos + (newVel * posStrength);
	
	vec4 newColor = vec4(newPos, newVel);
	
	gl_FragColor = newColor;
}
