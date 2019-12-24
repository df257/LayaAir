#ifdef GL_FRAGMENT_PRECISION_HIGH
	precision highp float;
#else
	precision mediump float;
#endif

#define PI 3.14159265359
#include "Lighting.glsl";

uniform sampler2D u_Texture;
uniform vec4 u_TintColor;

varying vec3 v_Texcoord;
varying vec2 v_Image180ScaleAndCutoff;
varying vec4 v_Layout3DScaleAndOffset;

vec2 ToRadialCoords(vec3 coords)
{
	vec3 normalizedCoords = normalize(coords);
	float latitude = acos(normalizedCoords.y);
	float longitude = atan(normalizedCoords.z,normalizedCoords.x);
	vec2 sphereCoords = vec2(longitude, latitude) * vec2(0.5/PI, 1.0/PI);
	return vec2(0.5,1.0) - sphereCoords;
}


void main()
{	
	vec2 tc = ToRadialCoords(v_Texcoord);
	if (tc.x > v_Image180ScaleAndCutoff.y)
		gl_FragColor=vec4(0,0,0,1);
	tc.x = mod(tc.x*v_Image180ScaleAndCutoff.x, 1.0);
	tc = (tc + v_Layout3DScaleAndOffset.xy) * v_Layout3DScaleAndOffset.zw;

	mediump vec4 tex = texture2D (u_Texture, tc);
	mediump vec3 c = decodeHDR (tex, 1.0);
	c = c * u_TintColor.rgb * 2.0;//Gamma Space is 2.0,linear space is 4.59479380
	gl_FragColor=vec4(c, 1.0);
}

