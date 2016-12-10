/*-------------------------------------------------------------------------

 아이템DB
 전체검색用に継承してる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Drawing;
using System;

using Utility;
using gvo_base;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class ItemDatabaseCustom : ItemDatabase
	{
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public ItemDatabaseCustom(string fname)
			: base(fname)
		{
		}
		
		/*-------------------------------------------------------------------------
		 전체검색
		---------------------------------------------------------------------------*/
		public void FindAll(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler)
		{
			IEnumerator<Data>	e	= base.GetEnumerator();
			while(e.MoveNext()){
				if(handler(e.Current.Name, find_string)){
					list.Add(new GvoDatabase.Find(e.Current));
				}
			}
		}

		/*-------------------------------------------------------------------------
		 전체검색
		 종류での검색用
		---------------------------------------------------------------------------*/
		public void FindAll_FromType(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler)
		{
			IEnumerator<Data>	e	= base.GetEnumerator();
			while(e.MoveNext()){
				if(handler(e.Current.Type, find_string)){
					list.Add(new GvoDatabase.Find(e.Current));
				}
			}
		}
	}
}
