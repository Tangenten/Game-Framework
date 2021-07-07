#version 460

uniform sampler2D inputTexture;
uniform vec2 outputResolution;

uniform float radius;
uniform vec2 direction;

void main() {
	vec2 uv = gl_TexCoord[0].xy;
	
	//this will be our RGBA sum
	vec4 sum = vec4(0.0);
	
	//the amount to blur, i.e. how far off center to sample from
	//1.0 -> blur by one pixel
	//2.0 -> blur by two pixels, etc.
	float blur = radius / outputResolution.x;
	
	//the direction of our blur
	//(1.0, 0.0) -> x-axis blur
	//(0.0, 1.0) -> y-axis blur
	float hstep = direction.x;
	float vstep = direction.y;
	
	//apply blurring, using a 9-tap filter with predefined gaussian weights
	
	sum += texture(inputTexture, vec2(uv.x - 4.0 * blur * hstep, uv.y - 4.0 * blur * vstep)) * 0.0162162162;
	sum += texture(inputTexture, vec2(uv.x - 3.0 * blur * hstep, uv.y - 3.0 * blur * vstep)) * 0.0540540541;
	sum += texture(inputTexture, vec2(uv.x - 2.0 * blur * hstep, uv.y - 2.0 * blur * vstep)) * 0.1216216216;
	sum += texture(inputTexture, vec2(uv.x - 1.0 * blur * hstep, uv.y - 1.0 * blur * vstep)) * 0.1945945946;
	
	sum += texture(inputTexture, vec2(uv.x, uv.y)) * 0.2270270270;
	
	sum += texture(inputTexture, vec2(uv.x + 1.0 * blur * hstep, uv.y + 1.0 * blur * vstep)) * 0.1945945946;
	sum += texture(inputTexture, vec2(uv.x + 2.0 * blur * hstep, uv.y + 2.0 * blur * vstep)) * 0.1216216216;
	sum += texture(inputTexture, vec2(uv.x + 3.0 * blur * hstep, uv.y + 3.0 * blur * vstep)) * 0.0540540541;
	sum += texture(inputTexture, vec2(uv.x + 4.0 * blur * hstep, uv.y + 4.0 * blur * vstep)) * 0.0162162162;
	
	gl_FragColor = vec4(sum.rgb, 1.0);
}
