#version 450

uniform sampler2D heightMap;
uniform sampler2D colorMap;

uniform vec2 cameraPositionUV;
uniform float cameraHeight;
uniform float cameraSteps = 100;

void main(){
	vec2 pixelLocationUV = gl_TexCoord[0].xy;
	
	vec3 pixel = vec3(pixelLocationUV, 0);
	vec3 camera = vec3(cameraPositionUV, cameraHeight);
	
	float sightDistance = distance(camera, pixel);
	float stepSize = 1.0 / cameraSteps;
	
	for(float i = 0; i <= sightDistance; i += stepSize){
		vec3 lerper = mix(camera, pixel, i / sightDistance);
		float lerperHeight = lerper.z;
		float groundHeight = texture(heightMap, lerper.xy).r;
		
		if(lerperHeight <= groundHeight){
			gl_FragColor = texture(colorMap, lerper.xy);
			break;
		}
	}
}
