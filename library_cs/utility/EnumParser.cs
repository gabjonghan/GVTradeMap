// Type: Utility.EnumParser`1
// Assembly: gvtrademap_cs, Version=1.3.2.3, Culture=neutral, PublicKeyToken=null
// MVID: 3D162A44-1A8B-4B7A-9FC3-6379559CB419
// Assembly location: C:\tmp\A\files\gvtrademap_cs.exe

using System;

namespace Utility
{
  public class EnumParser<T> where T : IComparable
  {
    private string[] m_other_to_enum_strings;
    private T m_success_value;
    private string m_to_string;

    public T SuccessValue
    {
      get
      {
        return m_success_value;
      }
    }

    public EnumParser(T success_value, string to_string)
      : this(success_value, to_string, (string[]) null)
    {
    }

    public EnumParser(T success_value, string to_string, string[] to_enum)
    {
      if (!((object) m_success_value is Enum))
        throw new ArgumentException("TはEnumでなければなりません. ");
      m_success_value = success_value;
      m_other_to_enum_strings = to_enum;
      m_to_string = to_string;
    }

    public bool CanParse(string str)
    {
      if (string.Compare(m_to_string, str, true) == 0 || string.Compare(m_success_value.ToString(), str, true) == 0)
        return true;
      object obj1 = Useful.ToEnum(typeof (T), (object) str);
      if (obj1 != null && m_success_value.Equals((object) (T) obj1))
        return true;
      int result;
      if (int.TryParse(str, out result))
      {
        object obj2 = Useful.ToEnum(typeof (T), (object) result);
        if (obj2 != null && m_success_value.Equals((object) (T) obj2))
          return true;
      }
      if (m_other_to_enum_strings != null)
      {
        foreach (string strA in m_other_to_enum_strings)
        {
          if (string.Compare(strA, str, true) == 0)
            return true;
        }
      }
      return false;
    }

    public bool CanParseForOtherCase(string str)
    {
      if (string.Compare(m_to_string, str, true) == 0 || string.Compare(m_success_value.ToString(), str, true) == 0)
        return true;
      if (m_other_to_enum_strings != null)
      {
        foreach (string strA in m_other_to_enum_strings)
        {
          if (string.Compare(strA, str, true) == 0)
            return true;
        }
      }
      return false;
    }

    public override string ToString()
    {
      return m_to_string;
    }
  }
}
