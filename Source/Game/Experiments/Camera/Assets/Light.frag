#version 450

uniform sampler2D heightMap;

uniform vec2 lightPositionUV;
uniform float lightHeightOffGround;
uniform vec4 lightColor;

uniform float lightSteps;
uniform float lightFalloff;
uniform float lightCollideFalloff;

uniform vec3 cameraRatio = vec3(16.0, 9.0, 2.0);

void main(){
	vec2 pixelPositionUV = gl_TexCoord[0].xy;
	float pixelHeight = texture(heightMap, pixelPositionUV).r;
	
	float lightHeight = texture(heightMap, lightPositionUV).r + lightHeightOffGround;
	
	vec3 light = vec3(vec2(lightPositionUV), lightHeight) * cameraRatio;
	vec3 pixel = vec3(vec2(pixelPositionUV), pixelHeight) * cameraRatio;
	
	float sightDistance = distance(light, pixel);
	
	if((1.0 - (lightFalloff * sightDistance)) >= 0.1){
		float stepSize = 1.0 / lightSteps;
		float scaledLightFalloff = lightFalloff / lightSteps;
		float lightStrength = 1.0;
		
		for(float i = 0; i <= sightDistance; i += stepSize){
			vec3 lerper = mix(light, pixel, i / sightDistance);
			float lerperHeight = lerper.z;
			float groundHeight = texture(heightMap, vec2(lerper.xy) / cameraRatio.xy).r * cameraRatio.z;
			
			if(lerperHeight <= groundHeight){
				lightStrength -= (scaledLightFalloff * lightCollideFalloff);
			}else{
				lightStrength -= scaledLightFalloff;
			}
			
			if (lightStrength <= 0) {
				lightStrength = 0;
				break;
			}
		}
		gl_FragColor =  vec4(vec3(lightColor.rgb), lightStrength);
	}
}

