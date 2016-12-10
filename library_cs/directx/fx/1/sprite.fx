/*-------------------------------------------------------------------------
 スプライト描画用シェーダ
 地図のオフセットとスケールに対応したものが含まれる
 vs_1_1 ps_1_1 用
---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 params
---------------------------------------------------------------------------*/
// ビューポート情報
// 1/ViewportSizeはプリシェーダが求める
float2	ViewportSize;	// {width, height }

// 地図のオフセット
float2	MapOffset;
// 地図のスケール
float	MapScale;
// 切り取ったスプライト矩形に掛けられるスケール
float2	GlobalScale;

// テクスチャサンプラ設定
texture	Texture;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
sampler TextureSampler = sampler_state
{
	Texture = (Texture);
};

/*-------------------------------------------------------------------------
 スクリーン座標からビューポート座標へ変換
---------------------------------------------------------------------------*/
float2 ScreenToViewport(float2 screenPos)
{
	// DirectXのスクリーン座標の0.5ピクセルのずれを直す
	screenPos		-= 0.5f;

	float2 result	= screenPos.xy / ViewportSize.xy * 2 - 1;
	result.y		= -result.y;
	return result;
}

/*-------------------------------------------------------------------------
 回転
---------------------------------------------------------------------------*/
float2 RotationPosition(float2 p, float angle_rad)
{
	float cs = cos(angle_rad);
	float sn = sin(angle_rad);
	return mul(p, float2x2(cs, sn, -sn, cs));
}

/*-------------------------------------------------------------------------
 共通のピクセルシェーダー
---------------------------------------------------------------------------*/
void PS( inout float4 color : COLOR0, float2 texCoord : TEXCOORD0 )
{
    color *= tex2D(TextureSampler, texCoord);
}

/*-------------------------------------------------------------------------
 スクリーン座標からビューポート座標への変換のみのシェーダ
---------------------------------------------------------------------------*/
void TransformedVS(
	float3 position : POSITION0,
	float2 texCoord : TEXCOORD0,
	float4 color    : COLOR0,

	uniform	bool	b_with_offset,

	out float4 outputPosition : POSITION0,
	out float2 outputTexCoord : TEXCOORD0,
	out float4 outputColor    : COLOR0 )
{
	float2	pos;

	if(b_with_offset){
		// グローバルパラメータ対応版
		pos		= (position.xy + MapOffset) * MapScale;		// 地図のオフセットとスケールを考慮
		pos		= floor(pos);	// 小数部切り捨て
	}else{
		// そのまま
		pos		= position.xy;
	}

	// スクリーン座標からビューポート座標に変換
	pos		= ScreenToViewport(pos);
	outputPosition	= float4(pos.x, pos.y, position.z, 1);

	// 他のパラメーターはそのまま
	outputTexCoord	= texCoord;
	outputColor		= color;
}

/*-------------------------------------------------------------------------
 technique
 座標変換後のスプライト描画用
---------------------------------------------------------------------------*/
technique Transformed
{
	pass Pass1
	{
		VertexShader	= compile vs_1_1 TransformedVS(false);
		PixelShader		= compile ps_1_1 PS();
	}
}

/*-------------------------------------------------------------------------
 technique
 座標変換後のスプライト描画用

 MapOffset
 MapScale
 に対応
 GlobalScale
 には未対応なので注意
---------------------------------------------------------------------------*/
technique TransformedWithGlobalParams
{
	pass Pass1
	{
		VertexShader	= compile vs_1_1 TransformedVS(true);
		PixelShader		= compile ps_1_1 PS();
	}
}

/*-------------------------------------------------------------------------
 通常のスプライト
 回転
 拡大
---------------------------------------------------------------------------*/
void SpriteVertexVS(
	float3 position	: POSITION0,
	float2 texCoord	: TEXCOORD0,
	float4 color	: COLOR0,
	float4 param	: TEXCOORD1,	// offset1.x, offset1.y, offset2.x, offset2.y
									// offset1 = 矩形切り出しのオフセット
									// offset2 = その他の自由なオフセット
									// offset2は単純に加算される
	float3 param2	: TEXCOORD2,	// scale.x, scale.y, angle_rad

	uniform	bool	b_with_offset,

	out float4 outputPosition : POSITION0,
	out float2 outputTexCoord : TEXCOORD0,
	out float4 outputColor    : COLOR0 )
{
	float2	pos, offset;

	if(b_with_offset){
		// グローバルパラメータに影響するスプライト
		pos		= (position.xy + MapOffset) * MapScale;		// 地図のオフセットとスケールを考慮
		pos		= floor(pos);	// 小数部切り捨て
		offset	= (RotationPosition(param.xy, param2.z) * param2.xy) * GlobalScale.xy;
	}else{
		// 通常のスプライト
		pos		= floor(position.xy);
		offset	= (RotationPosition(param.xy, param2.z) * param2.xy);
	}
	pos		= pos + param.zw + offset;

	// スクリーン座標からビューポート座標に変換
	pos				= ScreenToViewport(pos);
	outputPosition	= float4(pos.x, pos.y, position.z, 1);

	// 他のパラメーターはそのまま
	outputTexCoord	= texCoord;
	outputColor		= color;
}

/*-------------------------------------------------------------------------
 technique
 通常のスプライト
---------------------------------------------------------------------------*/
technique Sprite
{
	pass Pass1
	{
		VertexShader	= compile vs_1_1 SpriteVertexVS(false);
		PixelShader		= compile ps_1_1 PS();
	}
}

/*-------------------------------------------------------------------------
 technique
 通常のスプライト
 オフセットとスケール
 矩形に対するグローバルスケールに対応版
---------------------------------------------------------------------------*/
technique SpriteWithGlobalParams
{
	pass Pass1
	{
		VertexShader	= compile vs_1_1 SpriteVertexVS(true);
		PixelShader		= compile ps_1_1 PS();
	}
}
