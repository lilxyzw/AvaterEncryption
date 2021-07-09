#ifndef AVATAR_ENCRYPTION_INCLUDED
#define AVATAR_ENCRYPTION_INCLUDED

// _IgnoreEncryption ("Ignore Encryption", Int) = 0
// _Keys ("Keys", Vector) = (0,0,0,0)

float4 _Keys;
uint _IgnoreEncryption;

float4 vertexDecode(float4 positionOS, float3 normal, float2 uv6, float2 uv7)
{
    if(_IgnoreEncryption) return positionOS;

    float4 keys = floor(_Keys + 0.5);
    keys = keys.x == 0 ? float4(0,0,0,0) : floor(keys / 3) * 3 + 1;

    keys.x *= 1;
    keys.y *= 2;
    keys.z *= 3;
    keys.w *= 4;

    positionOS.xyz -= normal * uv6.x * (sin((keys.z - keys.y) * 2) * cos(keys.w - keys.x));
    positionOS.xyz -= normal * uv6.y * (sin((keys.w - keys.x) * 3) * cos(keys.z - keys.y));
    positionOS.xyz -= normal * uv7.x * (sin((keys.x - keys.w) * 4) * cos(keys.y - keys.z));
    positionOS.xyz -= normal * uv7.y * (sin((keys.y - keys.z) * 5) * cos(keys.x - keys.w));

    return positionOS;
}

#endif