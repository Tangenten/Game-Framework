#version 450

float Bell( float x ){
	float f = ( x / 2.0 ) * 1.5; // Converting -2 to +2 to -1.5 to +1.5
	if( f > -1.5 && f < -0.5 ){
		return( 0.5 * pow(f + 1.5, 2.0));
	}
	else if( f > -0.5 && f < 0.5 ){
		return 3.0 / 4.0 - ( f * f );
	}
	else if( ( f > 0.5 && f < 1.5 ) ){
		return( 0.5 * pow(f - 1.5, 2.0));
	}
	return 0.0;
}

float Triangular( float f ){
	f = f / 2.0;
	if( f < 0.0 ){
		return ( f + 1.0 );
	}
	else {
		return ( 1.0 - f );
	}
	return 0.0;
}

float BSpline( float x ){
	float f = x;
	if( f < 0.0 ){
		f = -f;
	}
	
	if( f >= 0.0 && f <= 1.0 ){
		return ( 2.0 / 3.0 ) + ( 0.5 ) * ( f* f * f ) - (f*f);
	}
	else if( f > 1.0 && f <= 2.0 ){
		return 1.0 / 6.0 * pow( ( 2.0 - f  ), 3.0 );
	}
	return 1.0;
}

float CatMullRom( float x ){
	const float B = 0.0;
	const float C = 0.5;
	float f = x;
	if( f < 0.0 ){
		f = -f;
	}
	if( f < 1.0 ){
		return ( ( 12 - 9 * B - 6 * C ) * ( f * f * f ) +
		( -18 + 12 * B + 6 *C ) * ( f * f ) +
		( 6 - 2 * B ) ) / 6.0;
	}
	else if( f >= 1.0 && f < 2.0 ){
		return ( ( -B - 6 * C ) * ( f * f * f )
		+ ( 6 * B + 30 * C ) * ( f *f ) +
		( - ( 12 * B ) - 48 * C  ) * f +
		8 * B + 24 * C)/ 6.0;
	}
	else {
		return 0.0;
	}
}

vec4 BiCubicBell( sampler2D textureSampler, vec2 TexCoord, vec2 res){
	float fWidth = res.x;
	float fHeight = res.y;
	
	float texelSizeX = 1.0 / fWidth; //size of one texel
	float texelSizeY = 1.0 / fHeight; //size of one texel
	vec4 nSum = vec4( 0.0, 0.0, 0.0, 0.0 );
	vec4 nDenom = vec4( 0.0, 0.0, 0.0, 0.0 );
	float a = fract( TexCoord.x * fWidth ); // get the decimal part
	float b = fract( TexCoord.y * fHeight ); // get the decimal part
	for( int m = -1; m <=2; m++ ){
		for( int n =-1; n<= 2; n++){
			vec4 vecData = texture(textureSampler,
			TexCoord + vec2(texelSizeX * float( m ),
			texelSizeY * float( n )));
			float f  = Bell( float( m ) - a );
			vec4 vecCooef1 = vec4( f,f,f,f );
			float f1 = Bell ( -( float( n ) - b ) );
			vec4 vecCoeef2 = vec4( f1, f1, f1, f1 );
			nSum = nSum + ( vecData * vecCoeef2 * vecCooef1  );
			nDenom = nDenom + (( vecCoeef2 * vecCooef1 ));
		}
	}
	return nSum / nDenom;
}

vec4 BiCubicTriangular( sampler2D textureSampler, vec2 TexCoord, vec2 res){
	float fWidth = res.x;
	float fHeight = res.y;
	
	float texelSizeX = 1.0 / fWidth; //size of one texel
	float texelSizeY = 1.0 / fHeight; //size of one texel
	vec4 nSum = vec4( 0.0, 0.0, 0.0, 0.0 );
	vec4 nDenom = vec4( 0.0, 0.0, 0.0, 0.0 );
	float a = fract( TexCoord.x * fWidth ); // get the decimal part
	float b = fract( TexCoord.y * fHeight ); // get the decimal part
	for( int m = -1; m <=2; m++ ){
		for( int n =-1; n<= 2; n++){
			vec4 vecData = texture(textureSampler,
			TexCoord + vec2(texelSizeX * float( m ),
			texelSizeY * float( n )));
			float f  = Triangular( float( m ) - a );
			vec4 vecCooef1 = vec4( f,f,f,f );
			float f1 = Triangular ( -( float( n ) - b ) );
			vec4 vecCoeef2 = vec4( f1, f1, f1, f1 );
			nSum = nSum + ( vecData * vecCoeef2 * vecCooef1  );
			nDenom = nDenom + (( vecCoeef2 * vecCooef1 ));
		}
	}
	return nSum / nDenom;
}

vec4 BiCubicSpline( sampler2D textureSampler, vec2 TexCoord, vec2 res){
	float fWidth = res.x;
	float fHeight = res.y;
	
	float texelSizeX = 1.0 / fWidth; //size of one texel
	float texelSizeY = 1.0 / fHeight; //size of one texel
	vec4 nSum = vec4( 0.0, 0.0, 0.0, 0.0 );
	vec4 nDenom = vec4( 0.0, 0.0, 0.0, 0.0 );
	float a = fract( TexCoord.x * fWidth ); // get the decimal part
	float b = fract( TexCoord.y * fHeight ); // get the decimal part
	for( int m = -1; m <=2; m++ ){
		for( int n =-1; n<= 2; n++){
			vec4 vecData = texture(textureSampler,
			TexCoord + vec2(texelSizeX * float( m ),
			texelSizeY * float( n )));
			float f  = BSpline( float( m ) - a );
			vec4 vecCooef1 = vec4( f,f,f,f );
			float f1 = BSpline ( -( float( n ) - b ) );
			vec4 vecCoeef2 = vec4( f1, f1, f1, f1 );
			nSum = nSum + ( vecData * vecCoeef2 * vecCooef1  );
			nDenom = nDenom + (( vecCoeef2 * vecCooef1 ));
		}
	}
	return nSum / nDenom;
}

vec4 BiCubicCatMullRom( sampler2D textureSampler, vec2 TexCoord, vec2 res){
	float fWidth = res.x;
	float fHeight = res.y;
	
	float texelSizeX = 1.0 / fWidth; //size of one texel
	float texelSizeY = 1.0 / fHeight; //size of one texel
	vec4 nSum = vec4( 0.0, 0.0, 0.0, 0.0 );
	vec4 nDenom = vec4( 0.0, 0.0, 0.0, 0.0 );
	float a = fract( TexCoord.x * fWidth ); // get the decimal part
	float b = fract( TexCoord.y * fHeight ); // get the decimal part
	for( int m = -1; m <=2; m++ ){
		for( int n =-1; n<= 2; n++){
			vec4 vecData = texture(textureSampler,
			TexCoord + vec2(texelSizeX * float( m ),
			texelSizeY * float( n )));
			float f  = CatMullRom( float( m ) - a );
			vec4 vecCooef1 = vec4( f,f,f,f );
			float f1 = CatMullRom ( -( float( n ) - b ) );
			vec4 vecCoeef2 = vec4( f1, f1, f1, f1 );
			nSum = nSum + ( vecData * vecCoeef2 * vecCooef1  );
			nDenom = nDenom + (( vecCoeef2 * vecCooef1 ));
		}
	}
	return nSum / nDenom;
}

uniform sampler2D inputTexture;
uniform vec2 inputResolution;

void main(){
	gl_FragColor = BiCubicBell(inputTexture, gl_TexCoord[0].xy, inputResolution);
}
