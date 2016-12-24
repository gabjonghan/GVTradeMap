/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *	 RandomBase.cs
 * Class:
 *	 Rei.Random.RandomBase
 * Purpose:
 *	 A base class for random number generator.
 * Remark:
 * History:
 *	 2007/10/6 initial release.
 * 
 */

using System;
using System.Diagnostics;

//namespace Rei.Random {
namespace Utility {

	/// <summary>
	/// 各種?似乱数ジェネレ???用基底クラス。
	/// 派生クラスはNextUInt32を実装する必要があります。
	/// </summary>
	public abstract class RandomBase {

		/// <summary>
		/// 派生クラスで符号なし32bitの?似乱数を生成する必要があります。
		/// </summary>
		public abstract UInt32 NextUInt32();

		/// <summary>
		/// 符号あり32bitの?似乱数を取得します。
		/// </summary>
		public virtual Int32 NextInt32() {
			return (Int32)NextUInt32();
		}

		/// <summary>
		/// 符号なし64bitの?似乱数を取得します。
		/// </summary>
		public virtual UInt64 NextUInt64() {
			return ((UInt64)NextUInt32() << 32) | NextUInt32();
		}

		/// <summary>
		/// 符号あり64bitの?似乱数を取得します。
		/// </summary>
		public virtual Int64 NextInt64() {
			return ((Int64)NextUInt32() << 32) | NextUInt32();
		}

		/// <summary>
		/// ?似乱数列を生成し、バイト配列に順に格?します。
		/// </summary>
		public virtual void NextBytes( byte[] buffer ) {
			int i = 0;
			UInt32 r;
			while (i + 4 <= buffer.Length) {
				r = NextUInt32();
				buffer[i++] = (byte)r;
				buffer[i++] = (byte)(r >> 8);
				buffer[i++] = (byte)(r >> 16);
				buffer[i++] = (byte)(r >> 24);
			}
			if (i >= buffer.Length) return;
			r = NextUInt32();
			buffer[i++] = (byte)r;
			if (i >= buffer.Length) return;
			buffer[i++] = (byte)(r >> 8);
			if (i >= buffer.Length) return;
			buffer[i++] = (byte)(r >> 16);
		}
		
		/// <summary>
		/// [0,1)の?似乱数を取得します。
		/// 返される値は0を含みますが1を含みません。
		/// </summary>
		public virtual double NextDouble()
		{
			return (1.0/4294967296.0) * NextUInt32();
		}

		/// <summary>
		/// [0,1]の?似乱数を取得します。
		/// 返される値は0及び1を含みます。
		/// </summary>
		public virtual double NextDouble2()
		{
			return (1.0/4294967295.0) * NextUInt32();
		}
	
		/// <summary>
		/// 0以上の符号付き整数を取得します。
		/// </summary>
		public virtual int Next()
		{
			return (Int32)NextUInt32();
		}

		/// <summary>
		/// 指定した最大値より小さい 0 以上の乱数を返します。
		/// 返される値には0を含みますがmax_valueを含みません。
		/// </summary>
		public virtual int Next(int max_value)
		{
			Trace.Assert(max_value >= 0, "RandomBase.Next()", "max_value は 0 以上にする必要があります。 ");
			return (int)(NextDouble() * max_value);
		}

		/// <summary>
		/// 指定した範囲内の乱数を返します。
		/// 返される値にはmin_valueを含みますがmax_valueを含みません。
		/// </summary>
		public virtual int Next(int min_value, int max_value)
		{
			Trace.Assert(max_value >= min_value, "RandomBase.Next()", "max_value は min_value 以上にする必要があります。");
			max_value	-= min_value;	// 必ず正の値
			return min_value + (int)(NextDouble() * max_value);
		}
	}
}
