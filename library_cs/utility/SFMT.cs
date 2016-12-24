/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *	 SFMT.cs
 * Class:
 *	 Rei.Random.SFMT
 *	 Rei.Random.MTPeriodType
 * Purpose:
 *	 A random number generator using SIMD-oriented Fast Mersenne Twister(SFMT).
 * Remark:
 *	 This code is C# implementation of SFMT.
 *	 SFMT was introduced by Mutsuo Saito and Makoto Matsumoto.
 *	 See http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/SFMT/index.html for detail of SFMT.
 * History:
 *	 2007/10/6 initial release.
 * 
 */

using System;

//namespace Rei.Random {
namespace Utility {

	/// <summary>
	/// SFMTの?似乱数ジェネレ???クラス。
	/// </summary>
	public class SFMT : RandomBase {

		#region Fields

		/// <summary>
		/// 周期を?す指数。
		/// </summary>
		protected int MEXP;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected int POS1;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected int SL1;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected int SL2;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected int SR1;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected int SR2;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected UInt32 MSK1;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected UInt32 MSK2;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected UInt32 MSK3;
		/// <summary>
		/// MTを決定するパラメ???の一つ。
		/// </summary>
		protected UInt32 MSK4;
		/// <summary>
		/// MTの周期を保証するための確認に用いるパラメ???の一つ。
		/// </summary>
		protected UInt32 PARITY1;
		/// <summary>
		/// MTの周期を保証するための確認に用いるパラメ???の一つ。
		/// </summary>
		protected UInt32 PARITY2;
		/// <summary>
		/// MTの周期を保証するための確認に用いるパラメ???の一つ。
		/// </summary>
		protected UInt32 PARITY3;
		/// <summary>
		/// MTの周期を保証するための確認に用いるパラメ???の一つ。
		/// </summary>
		protected UInt32 PARITY4;

		/// <summary>
		/// 各要素を128bitとしたときの内部状態ベクトルの個数。
		/// </summary>
		protected int N;
		/// <summary>
		/// 各要素を32bitとしたときの内部状態ベクトルの個数。
		/// </summary>
		protected int N32;
		/// <summary>
		/// 計算の高速化用。
		/// </summary>
		protected int SL2_x8;
		/// <summary>
		/// 計算の高速化用。
		/// </summary>
		protected int SR2_x8;
		/// <summary>
		/// 計算の高速化用。
		/// </summary>
		protected int SL2_ix8;
		/// <summary>
		/// 計算の高速化用。
		/// </summary>
		protected int SR2_ix8;

		/// <summary>
		/// 内部状態ベクトル。
		/// </summary>
		protected UInt32[] sfmt;
		/// <summary>
		/// 内部状態ベクトルのうち、次に乱数として使用するインデックス。
		/// </summary>
		protected int idx;

		#endregion

		/// <summary>
		/// 現在時刻を種とした、(2^19937-1)周期のSFMT?似乱数ジェネレ???を初期化します。
		/// </summary>
		public SFMT() : this(Environment.TickCount, 19937) { }

		/// <summary>
		/// seedを種とした、(2^19937-1)周期の?似乱数ジェネレ???を初期化します。
		/// </summary>
		public SFMT( int seed ) : this(seed, 19937) { }

		/// <summary>
		/// seedを種とした、periodで?される周期の?似乱数ジェネレ???を初期化します。
		/// </summary>
		public SFMT( int seed, MTPeriodType period ) : this(seed, (int)period) { }

		/// <summary>
		/// seedを種とした、(2^mexp-1)周期の?似乱数ジェネレ???を初期化します。
		/// mexpは607,1279,2281,4253,11213,19937,44497,86243,132049,216091のいずれかである必要があります。
		/// </summary>
		public SFMT( int seed, int mexp ) {
			this.MEXP = mexp;
			if (mexp == 607) {
				POS1 = 2;
				SL1 = 15;
				SL2 = 3;
				SR1 = 13;
				SR2 = 3;
				MSK1 = 0xfdff37ffU;
				MSK2 = 0xef7f3f7dU;
				MSK3 = 0xff777b7dU;
				MSK4 = 0x7ff7fb2fU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0x00000000U;
				PARITY4 = 0x5986f054U;
			} else if (mexp == 1279) {
				POS1 = 7;
				SL1 = 14;
				SL2 = 3;
				SR1 = 5;
				SR2 = 1;
				MSK1 = 0xf7fefffdU;
				MSK2 = 0x7fefcfffU;
				MSK3 = 0xaff3ef3fU;
				MSK4 = 0xb5ffff7fU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0x00000000U;
				PARITY4 = 0x20000000U;
			} else if (mexp == 2281) {
				POS1 = 12;
				SL1 = 19;
				SL2 = 1;
				SR1 = 5;
				SR2 = 1;
				MSK1 = 0xbff7ffbfU;
				MSK2 = 0xfdfffffeU;
				MSK3 = 0xf7ffef7fU;
				MSK4 = 0xf2f7cbbfU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0x00000000U;
				PARITY4 = 0x41dfa600U;
			} else if (mexp == 4253) {
				POS1 = 17;
				SL1 = 20;
				SL2 = 1;
				SR1 = 7;
				SR2 = 1;
				MSK1 = 0x9f7bffffU;
				MSK2 = 0x9fffff5fU;
				MSK3 = 0x3efffffbU;
				MSK4 = 0xfffff7bbU;
				PARITY1 = 0xa8000001U;
				PARITY2 = 0xaf5390a3U;
				PARITY3 = 0xb740b3f8U;
				PARITY4 = 0x6c11486dU;
			} else if (mexp == 11213) {
				POS1 = 68;
				SL1 = 14;
				SL2 = 3;
				SR1 = 7;
				SR2 = 3;
				MSK1 = 0xeffff7fbU;
				MSK2 = 0xffffffefU;
				MSK3 = 0xdfdfbfffU;
				MSK4 = 0x7fffdbfdU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0xe8148000U;
				PARITY4 = 0xd0c7afa3U;
			} else if (mexp == 19937) {
				POS1 = 122;
				SL1 = 18;
				SL2 = 1;
				SR1 = 11;
				SR2 = 1;
				MSK1 = 0xdfffffefU;
				MSK2 = 0xddfecb7fU;
				MSK3 = 0xbffaffffU;
				MSK4 = 0xbffffff6U;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0x00000000U;
				PARITY4 = 0x13c9e684U;
				PARITY4 = 0x20000000U;
			} else if (mexp == 44497) {
				POS1 = 330;
				SL1 = 5;
				SL2 = 3;
				SR1 = 9;
				SR2 = 3;
				MSK1 = 0xeffffffbU;
				MSK2 = 0xdfbebfffU;
				MSK3 = 0xbfbf7befU;
				MSK4 = 0x9ffd7bffU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0xa3ac4000U;
				PARITY4 = 0xecc1327aU;
			} else if (mexp == 86243) {
				POS1 = 366;
				SL1 = 6;
				SL2 = 7;
				SR1 = 19;
				SR2 = 1;
				MSK1 = 0xfdbffbffU;
				MSK2 = 0xbff7ff3fU;
				MSK3 = 0xfd77efffU;
				MSK4 = 0xbf9ff3ffU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0x00000000U;
				PARITY4 = 0xe9528d85U;
			} else if (mexp == 132049) {
				POS1 = 110;
				SL1 = 19;
				SL2 = 1;
				SR1 = 21;
				SR2 = 1;
				MSK1 = 0xffffbb5fU;
				MSK2 = 0xfb6ebf95U;
				MSK3 = 0xfffefffaU;
				MSK4 = 0xcff77fffU;
				PARITY1 = 0x00000001U;
				PARITY2 = 0x00000000U;
				PARITY3 = 0xcb520000U;
				PARITY4 = 0xc7e91c7dU;
			} else if (mexp == 216091) {
				POS1 = 627;
				SL1 = 11;
				SL2 = 3;
				SR1 = 10;
				SR2 = 1;
				MSK1 = 0xbff7bff7U;
				MSK2 = 0xbfffffffU;
				MSK3 = 0xbffffa7fU;
				MSK4 = 0xffddfbfbU;
				PARITY1 = 0xf8000001U;
				PARITY2 = 0x89e80709U;
				PARITY3 = 0x3bd2b64bU;
				PARITY4 = 0x0c64b1e4U;
			} else {
				throw new ArgumentException();
			}
			init_gen_rand(seed);
		}

		/// <summary>
		/// 符号なし32bitの?似乱数を取得します。
		/// </summary>
		public override UInt32 NextUInt32() {
			if (idx >= N32) {
				gen_rand_all();
				idx = 0;
			}
			return sfmt[idx++];
		}

		/// <summary>
		/// ジェネレ???を初期化します。
		/// 途中で初期化する必要がある場合向け
		/// 主にリプレイでステ?ジ開始時に乱数の種を覚えておきたい時等
		/// </summary>
		/// <param name="seed"></param>
		public void Seed(int seed)
		{
			init_gen_rand(seed);
		}
		
		/// <summary>
		/// ジェネレ???を初期化します。
		/// </summary>
		/// <param name="seed"></param>
		protected void init_gen_rand( int seed ) {
			int i;
			//変数初期化
			N = MEXP / 128 + 1;
			N32 = N * 4;
			SL2_x8 = SL2 * 8;
			SR2_x8 = SR2 * 8;
			SL2_ix8 = 64 - SL2 * 8;
			SR2_ix8 = 64 - SR2 * 8;
			//内部状態配列確保
			sfmt = new UInt32[N32];
			//内部状態配列初期化
			sfmt[0] = (UInt32)seed;
			for (i = 1; i < N32; i++)
				sfmt[i] = (UInt32)(1812433253 * (sfmt[i - 1] ^ (sfmt[i - 1] >> 30)) + i);
			//確認
			period_certification();
			//初期位置設定
			idx = N32;
		}

		/// <summary>
		/// 内部状態ベクトルが適切か確認し、必要であれば調節します。
		/// </summary>
		protected void period_certification() {
			UInt32[] PARITY = new UInt32[] { PARITY1, PARITY2, PARITY3, PARITY4 };
			UInt32 inner = 0;
			int i, j;
			UInt32 work;

			for (i = 0; i < 4; i++) inner ^= sfmt[i] & PARITY[i];
			for (i = 16; i > 0; i >>= 1) inner ^= inner >> i;
			inner &= 1;
			// check OK
			if (inner == 1) return;
			// check NG, and modification
			for (i = 0; i < 4; i++) {
				work = 1;
				for (j = 0; j < 32; j++) {
					if ((work & PARITY[i]) != 0) {
						sfmt[i] ^= work;
						return;
					}
					work = work << 1;
				}
			}
		}

		/// <summary>
		/// 内部状態ベクトルを更新します。
		/// </summary>
		protected virtual void gen_rand_all() {
			if (MEXP == 19937) { gen_rand_all_19937(); return; }
			int a, b, c, d;
			UInt64 xh, xl, yh, yl;

			a = 0;
			b = POS1 * 4;
			c = (N - 2) * 4;
			d = (N - 1) * 4;
			do {
				xh = ((UInt64)sfmt[a + 3] << 32) | sfmt[a + 2];
				xl = ((UInt64)sfmt[a + 1] << 32) | sfmt[a + 0];
				yh = xh << (SL2_x8) | xl >> (SL2_ix8);
				yl = xl << (SL2_x8);
				xh = ((UInt64)sfmt[c + 3] << 32) | sfmt[c + 2];
				xl = ((UInt64)sfmt[c + 1] << 32) | sfmt[c + 0];
				yh ^= xh >> (SR2_x8);
				yl ^= xl >> (SR2_x8) | xh << (SR2_ix8);

				sfmt[a + 3] = sfmt[a + 3] ^ ((sfmt[b + 3] >> SR1) & MSK4) ^ (sfmt[d + 3] << SL1) ^ ((UInt32)(yh >> 32));
				sfmt[a + 2] = sfmt[a + 2] ^ ((sfmt[b + 2] >> SR1) & MSK3) ^ (sfmt[d + 2] << SL1) ^ ((UInt32)yh);
				sfmt[a + 1] = sfmt[a + 1] ^ ((sfmt[b + 1] >> SR1) & MSK2) ^ (sfmt[d + 1] << SL1) ^ ((UInt32)(yl >> 32));
				sfmt[a + 0] = sfmt[a + 0] ^ ((sfmt[b + 0] >> SR1) & MSK1) ^ (sfmt[d + 0] << SL1) ^ ((UInt32)yl);

				c = d; d = a; a += 4; b += 4;
				if (b >= N32) b = 0;
			} while (a < N32);
		}

		/// <summary>
		/// gen_rand_allの(2^19937-1)周期用。
		/// </summary>
		private void gen_rand_all_19937() {
			int a, b, c, d;
			UInt32[] p = this.sfmt;

			const int cMEXP = 19937;
			const int cPOS1 = 122;
			const uint cMSK1 = 0xdfffffefU;
			const uint cMSK2 = 0xddfecb7fU;
			const uint cMSK3 = 0xbffaffffU;
			const uint cMSK4 = 0xbffffff6U;
			const int cSL1 = 18;
			const int cSR1 = 11;
			const int cN = cMEXP / 128 + 1;
			const int cN32 = cN * 4;

			a = 0;
			b = cPOS1 * 4;
			c = (cN - 2) * 4;
			d = (cN - 1) * 4;
			do {
				p[a + 3] = p[a + 3] ^ (p[a + 3] << 8) ^ (p[a + 2] >> 24) ^ (p[c + 3] >> 8) ^ ((p[b + 3] >> cSR1) & cMSK4) ^ (p[d + 3] << cSL1);
				p[a + 2] = p[a + 2] ^ (p[a + 2] << 8) ^ (p[a + 1] >> 24) ^ (p[c + 3] << 24) ^ (p[c + 2] >> 8) ^ ((p[b + 2] >> cSR1) & cMSK3) ^ (p[d + 2] << cSL1);
				p[a + 1] = p[a + 1] ^ (p[a + 1] << 8) ^ (p[a + 0] >> 24) ^ (p[c + 2] << 24) ^ (p[c + 1] >> 8) ^ ((p[b + 1] >> cSR1) & cMSK2) ^ (p[d + 1] << cSL1);
				p[a + 0] = p[a + 0] ^ (p[a + 0] << 8) ^ (p[c + 1] << 24) ^ (p[c + 0] >> 8) ^ ((p[b + 0] >> cSR1) & cMSK1) ^ (p[d + 0] << cSL1);
				c = d; d = a; a += 4; b += 4;
				if (b >= cN32) b = 0;
			} while (a < cN32);
		}

	}

	/// <summary>
	/// メルセンヌツイス??で用いる周期をあらわす列挙?。
	/// </summary>
	public enum MTPeriodType {
		/// <summary>
		/// 2^607-1周期のMT。
		/// </summary>
		MT607 = 607,
		/// <summary>
		/// 2^1279-1周期のMT。
		/// </summary>
		MT1279 = 1279,
		/// <summary>
		/// 2^2281-1周期のMT。
		/// </summary>
		MT2281 = 2281,
		/// <summary>
		/// 2^4253-1周期のMT。
		/// </summary>
		MT4253 = 4253,
		/// <summary>
		/// 2^11213-1周期のMT。
		/// </summary>
		MT11213 = 11213,
		/// <summary>
		/// 2^19937-1周期のMT。
		/// </summary>
		MT19937 = 19937,
		/// <summary>
		/// 2^44497-1周期のMT。
		/// </summary>
		MT44497 = 44497,
		/// <summary>
		/// 2^86243-1周期のMT。
		/// </summary>
		MT86243 = 86243,
		/// <summary>
		/// 2^132049-1周期のMT。
		/// </summary>
		MT132049 = 132049,
		/// <summary>
		/// 2^216091-1周期のMT。
		/// </summary>
		MT216091 = 216091
	}
}