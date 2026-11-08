/*-------------------------------------------------------------------------

 업데이트 확인
 VERSION_URL 의 버전 파일을 받아서 def.VERSION 과 비교한다
 버전 파일 형식: 1행째가 버전(정수), 2행째 이후는 업데이트 내용으로 그대로 표시한다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	static class update_checker
	{
		public enum result {
			error,			// 버전 정보를 얻지 못했다
			latest,			// 사용 중인 버전이 최신
			updated,		// 새 버전이 있다
		};

		/*-------------------------------------------------------------------------
		 버전 파일을 받아서 확인한다
		 updated 일 때만 data 에 업데이트 내용이 들어간다
		---------------------------------------------------------------------------*/
		static public result Check(out string[] data)
		{
			data	= new string[0];

			string	text	= HttpDownload.Download(def.VERSION_URL, Encoding.UTF8);
			if(text == null)	return result.error;

			string[]	lines	= text.Replace("\r\n", "\n").Split('\n');
			int			version;
			try{
				version		= Convert.ToInt32(lines[0].Trim());
			}catch{
				return result.error;
			}
			if(version <= def.VERSION)	return result.latest;

			List<string>	list	= new List<string>();
			for(int i=1; i<lines.Length; i++){
				list.Add(lines[i]);
			}
			data	= list.ToArray();
			return result.updated;
		}
	}
}
