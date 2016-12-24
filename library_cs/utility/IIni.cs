// Type: Utility.Ini.IIni
// Assembly: gvtrademap_cs, Version=1.3.2.3, Culture=neutral, PublicKeyToken=null
// MVID: 3D162A44-1A8B-4B7A-9FC3-6379559CB419
// Assembly location: C:\tmp\A\files\gvtrademap_cs.exe

namespace Utility.Ini
{
  public interface IIni
  {
	string GetProfile(string group_name, string name, string default_value);

	string[] GetProfile(string group_name, string name, string[] default_value);

	void SetProfile(string group_name, string name, string value);

	void SetProfile(string group_name, string name, string[] value);

	bool GetProfile(string group_name, string name, bool default_value);

	int GetProfile(string group_name, string name, int default_value);

	long GetProfile(string group_name, string name, long default_value);

	double GetProfile(string group_name, string name, double default_value);

	float GetProfile(string group_name, string name, float default_value);

	void SetProfile(string group_name, string name, bool value);

	void SetProfile(string group_name, string name, int value);

	void SetProfile(string group_name, string name, long value);

	void SetProfile(string group_name, string name, double value);

	void SetProfile(string group_name, string name, float value);

	bool[] GetProfile(string group_name, string name, bool[] default_value);

	int[] GetProfile(string group_name, string name, int[] default_value);

	long[] GetProfile(string group_name, string name, long[] default_value);

	double[] GetProfile(string group_name, string name, double[] default_value);

	float[] GetProfile(string group_name, string name, float[] default_value);

	void SetProfile(string group_name, string name, bool[] value);

	void SetProfile(string group_name, string name, int[] value);

	void SetProfile(string group_name, string name, long[] value);

	void SetProfile(string group_name, string name, double[] value);

	void SetProfile(string group_name, string name, float[] value);
  }
}
