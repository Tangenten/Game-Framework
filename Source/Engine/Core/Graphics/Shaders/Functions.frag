#version 460

vec2 getUV() {
	return gl_TexCoord[0].xy;
}

vec2 getOutputPixel(){
	return gl_FragCoord.xy;
}

vec2 getTextureResolution(sampler2D texture){
	return textureSize(texture, 0);
}

vec2 UVToPixelInt(vec2 uv, vec2 resolution){
	vec2 pixel = uv * resolution;
	pixel.x = int(pixel.x);
	pixel.y = int(pixel.y);
	pixel.y = int(abs(pixel.y - (resolution.y - 1)));
	
	return pixel;
}

vec2 UVToPixelFloat(vec2 uv, vec2 resolution){
	vec2 pixel = uv * resolution;
	pixel.y = int(abs(pixel.y - (resolution.y - 1)));
	
	return pixel;
}

vec2 FlipYPixel(vec2 pixel, vec2 resolution){
	vec2 pix = pixel;
	pix.y = int(abs(pix.y - (resolution.y - 1)));
	return pix;
}

vec2 pixelToUV(vec2 pixel, vec2 resolution){
	pixel.y = int(abs(pixel.y - (resolution.y - 1)));
	return pixel.xy / resolution;
}

vec3 rgb2hsv(vec3 c){
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c){
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float luma(vec3 color) {
	return dot(color, vec3(0.299, 0.587, 0.114));
}

float luma(vec4 color) {
	return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

float map(float value, float min1, float max1, float min2, float max2) {
	return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

float range(float vmin, float vmax, float value) {
	return (value - vmin) / (vmax - vmin);
}

vec2 range(vec2 vmin, vec2 vmax, vec2 value) {
	return (value - vmin) / (vmax - vmin);
}

vec3 range(vec3 vmin, vec3 vmax, vec3 value) {
	return (value - vmin) / (vmax - vmin);
}

vec4 range(vec4 vmin, vec4 vmax, vec4 value) {
	return (value - vmin) / (vmax - vmin);
}

float rand(float n){return fract(sin(n) * 43758.5453123);}

float noise(float p){
	float fl = floor(p);
	float fc = fract(p);
	return mix(rand(fl), rand(fl + 1.0), fc);
}

float noise(vec2 n) {
	const vec2 d = vec2(0.0, 1.0);
	vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
	return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

// Permutation polynomial: (34x^2 + x) mod 289
vec3 worleyPermute(vec3 x) {
	return mod((34.0 * x + 1.0) * x, 289.0);
}

vec3 worleyDistance(vec3 x, vec3 y,  bool manhattanDistance) {
	return manhattanDistance ?  abs(x) + abs(y) :  (x * x + y * y);
}

vec2 worley(vec2 P, float jitter, bool manhattanDistance) {
	float K= 0.142857142857; // 1/7
	float Ko= 0.428571428571 ;// 3/7
	vec2 Pi = mod(floor(P), 289.0);
	vec2 Pf = fract(P);
	vec3 oi = vec3(-1.0, 0.0, 1.0);
	vec3 of = vec3(-0.5, 0.5, 1.5);
	vec3 px = worleyPermute(Pi.x + oi);
	vec3 p = worleyPermute(px.x + Pi.y + oi); // p11, p12, p13
	vec3 ox = fract(p*K) - Ko;
	vec3 oy = mod(floor(p*K),7.0)*K - Ko;
	vec3 dx = Pf.x + 0.5 + jitter*ox;
	vec3 dy = Pf.y - of + jitter*oy;
	vec3 d1 = worleyDistance(dx,dy, manhattanDistance); // d11, d12 and d13, squared
	p = worleyPermute(px.y + Pi.y + oi); // p21, p22, p23
	ox = fract(p*K) - Ko;
	oy = mod(floor(p*K),7.0)*K - Ko;
	dx = Pf.x - 0.5 + jitter*ox;
	dy = Pf.y - of + jitter*oy;
	vec3 d2 = worleyDistance(dx,dy, manhattanDistance); // d21, d22 and d23, squared
	p = worleyPermute(px.z + Pi.y + oi); // p31, p32, p33
	ox = fract(p*K) - Ko;
	oy = mod(floor(p*K),7.0)*K - Ko;
	dx = Pf.x - 1.5 + jitter*ox;
	dy = Pf.y - of + jitter*oy;
	vec3 d3 = worleyDistance(dx,dy, manhattanDistance); // d31, d32 and d33, squared
	// Sort out the two smallest worleyDistanceances (F1, F2)
	vec3 d1a = min(d1, d2);
	d2 = max(d1, d2); // Swap to keep candidates for F2
	d2 = min(d2, d3); // neither F1 nor F2 are now in d3
	d1 = min(d1a, d2); // F1 is now in d1
	d2 = max(d1a, d2); // Swap to keep candidates for F2
	d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
	d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
	d1.yz = min(d1.yz, d2.yz); // F2 is now not in d2.yz
	d1.y = min(d1.y, d1.z); // nor in  d1.z
	d1.y = min(d1.y, d2.x); // F2 is in d1.y, we're done.
	return sqrt(d1.xy);
}

vec3 mod289(vec3 x) {
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec2 mod289(vec2 x) {
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec3 simplexPermute(vec3 x) {
	return mod289(((x*34.0)+1.0)*x);
}

float simplex(vec2 v){
	const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
	0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
	-0.577350269189626,  // -1.0 + 2.0 * C.x
	0.024390243902439); // 1.0 / 41.0
	// First corner
	vec2 i  = floor(v + dot(v, C.yy) );
	vec2 x0 = v -   i + dot(i, C.xx);
	
	// Other corners
	vec2 i1;
	//i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
	//i1.y = 1.0 - i1.x;
	i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
	// x0 = x0 - 0.0 + 0.0 * C.xx ;
	// x1 = x0 - i1 + 1.0 * C.xx ;
	// x2 = x0 - 1.0 + 2.0 * C.xx ;
	vec4 x12 = x0.xyxy + C.xxzz;
	x12.xy -= i1;
	
	// Permutations
	i = mod289(i); // Avoid truncation effects in permutation
	vec3 p = simplexPermute( simplexPermute( i.y + vec3(0.0, i1.y, 1.0 ))
	+ i.x + vec3(0.0, i1.x, 1.0 ));
	
	vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
	m = m*m ;
	m = m*m ;
	
	// Gradients: 41 points uniformly over a line, mapped onto a diamond.
	// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
	
	vec3 x = 2.0 * fract(p * C.www) - 1.0;
	vec3 h = abs(x) - 0.5;
	vec3 ox = floor(x + 0.5);
	vec3 a0 = x - ox;
	
	// Normalise gradients implicitly by scaling m
	// Approximation of: m *= inversesqrt( a0*a0 + h*h );
	m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
	
	// Compute final noise value at P
	vec3 g;
	g.x  = a0.x  * x0.x  + h.x  * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	return 130.0 * dot(m, g);
}

