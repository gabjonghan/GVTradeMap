// Type: Utility.EnumParserUtility`1
// Assembly: gvtrademap_cs, Version=1.3.2.3, Culture=neutral, PublicKeyToken=null
// MVID: 3D162A44-1A8B-4B7A-9FC3-6379559CB419
// Assembly location: C:\tmp\A\files\gvtrademap_cs.exe

using System;

namespace Utility
{
  public static class EnumParserUtility<T> where T : IComparable
  {
    public static string ToString(EnumParser<T>[] list, T value)
    {
      foreach (EnumParser<T> enumParser in list)
      {
        if (enumParser.SuccessValue.Equals((object) value))
          return enumParser.ToString();
      }
      return (string) null;
    }

    public static string ToString(EnumParser<T>[] list, T value, string default_value)
    {
      return EnumParserUtility<T>.ToString(list, value) ?? default_value;
    }

    public static T ToEnum(string str, T default_value)
    {
      object obj1 = Useful.ToEnum(typeof (T), (object) str);
      if (obj1 != null)
        return (T) obj1;
      int result;
      if (int.TryParse(str, out result))
      {
        object obj2 = Useful.ToEnum(typeof (T), (object) result);
        if (obj2 != null)
          return (T) obj2;
      }
      return default_value;
    }

    public static T ToEnum(EnumParser<T>[] list, string str, T default_value)
    {
      object obj1 = Useful.ToEnum(typeof (T), (object) str);
      if (obj1 != null)
        return (T) obj1;
      int result;
      if (int.TryParse(str, out result))
      {
        object obj2 = Useful.ToEnum(typeof (T), (object) result);
        if (obj2 != null)
          return (T) obj2;
      }
      foreach (EnumParser<T> enumParser in list)
      {
        if (enumParser.CanParseForOtherCase(str))
          return enumParser.SuccessValue;
      }
      return default_value;
    }
  }
}
