#version 460

float PI = 3.1415926535897932384626433832795;

float map(float value, float min1, float max1, float min2, float max2) {
	return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

float vec2ToRadian(in float y, in float x){
	bool s = (abs(x) > abs(y));
	return mix(PI/2.0 - atan(x,y), atan(y,x), s);
}

// All components are in the range [0…1], including hue.
vec3 hsv2rgb(vec3 c){
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

uniform sampler2D texture;
uniform vec2 resolution;

void main(){
	vec4 texColor = texture2D(texture, gl_Vertex.xy / resolution);
	
	float xPos = texColor.r;
	float yPos = texColor.g;
	float xVel = texColor.b;
	float yVel = texColor.a;
	
	float h = map(vec2ToRadian(xVel, yVel), 0.0, PI, 0.0, 1.0);
	vec3 newColor = hsv2rgb(vec3(h, 0.5, 0.75));
	
	gl_Position = vec4(xPos, yPos, 0, 1);
	//gl_Position = vec4(gl_Vertex.xy / resolution, 1, 1);
	gl_FrontColor = vec4(newColor, 0.5);
	//gl_FrontColor = vec4(vec3(xPos, yPos, xVel), 1);
}
