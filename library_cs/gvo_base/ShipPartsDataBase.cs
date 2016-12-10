// Type: gvo_base.ShipPartsDataBase
// Assembly: gvtrademap_cs, Version=1.3.2.3, Culture=neutral, PublicKeyToken=null
// MVID: 3D162A44-1A8B-4B7A-9FC3-6379559CB419
// Assembly location: C:\tmp\A\files\gvtrademap_cs.exe

using System;
using System.Collections.Generic;
using System.Xml;
using Utility;

namespace gvo_base
{
  public class ShipPartsDataBase
  {
    private static EnumParser<ShipPartsDataBase.BuildShipType>[] _builshiptype_enum_param = new EnumParser<ShipPartsDataBase.BuildShipType>[5]
    {
      new EnumParser<ShipPartsDataBase.BuildShipType>(ShipPartsDataBase.BuildShipType.Small1, "소형1"),
      new EnumParser<ShipPartsDataBase.BuildShipType>(ShipPartsDataBase.BuildShipType.Small2, "소형2"),
      new EnumParser<ShipPartsDataBase.BuildShipType>(ShipPartsDataBase.BuildShipType.Middle, "중형"),
      new EnumParser<ShipPartsDataBase.BuildShipType>(ShipPartsDataBase.BuildShipType.Big1, "대형1"),
      new EnumParser<ShipPartsDataBase.BuildShipType>(ShipPartsDataBase.BuildShipType.Big2, "대형2")
    };
    private static EnumParser<ShipPartsDataBase.ShipSizeType>[] _shipsizetype_enum_param = new EnumParser<ShipPartsDataBase.ShipSizeType>[5]
    {
      new EnumParser<ShipPartsDataBase.ShipSizeType>(ShipPartsDataBase.ShipSizeType.Small, "소형"),
      new EnumParser<ShipPartsDataBase.ShipSizeType>(ShipPartsDataBase.ShipSizeType.Middle, "중형"),
      new EnumParser<ShipPartsDataBase.ShipSizeType>(ShipPartsDataBase.ShipSizeType.Big, "대형"),
      new EnumParser<ShipPartsDataBase.ShipSizeType>(ShipPartsDataBase.ShipSizeType.All, "전부"),
      new EnumParser<ShipPartsDataBase.ShipSizeType>(ShipPartsDataBase.ShipSizeType.MiddleAndBig, "중형, 대형")
    };
    private static EnumParser<ShipPartsDataBase.PartsType>[] _partstype_enum_param = new EnumParser<ShipPartsDataBase.PartsType>[4]
    {
      new EnumParser<ShipPartsDataBase.PartsType>(ShipPartsDataBase.PartsType.Hull, "선체"),
      new EnumParser<ShipPartsDataBase.PartsType>(ShipPartsDataBase.PartsType.Sail, "돛"),
      new EnumParser<ShipPartsDataBase.PartsType>(ShipPartsDataBase.PartsType.Cannon, "대포"),
      new EnumParser<ShipPartsDataBase.PartsType>(ShipPartsDataBase.PartsType.Other, "병기")
    };
    private static EnumParser<ShipPartsDataBase.BuildTargetType>[] _buildtargettype_enum_param = new EnumParser<ShipPartsDataBase.BuildTargetType>[10]
    {
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Hp, "내구"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.VSail, "세로돛"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.HSail, "가로돛"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Row, "조력"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Turn, "선회"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Wave, "내파"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Armor, "장갑"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Cabin, "선실"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.CannonRoom, "포실"),
      new EnumParser<ShipPartsDataBase.BuildTargetType>(ShipPartsDataBase.BuildTargetType.Storage, "창고")
    };
    private List<ShipPartsDataBase.ShipPart> m_list;

    public List<ShipPartsDataBase.ShipPart> PartsList
    {
      get
      {
        return this.m_list;
      }
    }

    static ShipPartsDataBase()
    {
    }

    public ShipPartsDataBase(string file_name)
    {
      Console.WriteLine("public ShipPartsDataBase(string file_name)");
      this.m_list = new List<ShipPartsDataBase.ShipPart>();
      this.load_xml(file_name);
    }

    public static ShipPartsDataBase.BuildShipType ToBuildShipType(int index)
    {
      return ShipPartsDataBase.ToBuildShipType(index.ToString());
    }

    public static ShipPartsDataBase.BuildShipType ToBuildShipType(string name)
    {
      return EnumParserUtility<ShipPartsDataBase.BuildShipType>.ToEnum(ShipPartsDataBase._builshiptype_enum_param, name, ShipPartsDataBase.BuildShipType.Big2);
    }

    public static string ToString(ShipPartsDataBase.BuildShipType type)
    {
      return EnumParserUtility<ShipPartsDataBase.BuildShipType>.ToString(ShipPartsDataBase._builshiptype_enum_param, type, "불명");
    }

    public static ShipPartsDataBase.ShipSizeType ToShipSizeType(int index)
    {
      return ShipPartsDataBase.ToShipSizeType(index.ToString());
    }

    public static ShipPartsDataBase.ShipSizeType ToShipSizeType(string name)
    {
      return EnumParserUtility<ShipPartsDataBase.ShipSizeType>.ToEnum(ShipPartsDataBase._shipsizetype_enum_param, name, ShipPartsDataBase.ShipSizeType.All);
    }

    public static string ToString(ShipPartsDataBase.ShipSizeType type)
    {
      return EnumParserUtility<ShipPartsDataBase.ShipSizeType>.ToString(ShipPartsDataBase._shipsizetype_enum_param, type, "불명");
    }

    public static ShipPartsDataBase.PartsType ToPartsType(int index)
    {
      return ShipPartsDataBase.ToPartsType(index.ToString());
    }

    public static ShipPartsDataBase.PartsType ToPartsType(string name)
    {
      return EnumParserUtility<ShipPartsDataBase.PartsType>.ToEnum(ShipPartsDataBase._partstype_enum_param, name, ShipPartsDataBase.PartsType.Hull);
    }

    public static string ToString(ShipPartsDataBase.PartsType type)
    {
      return EnumParserUtility<ShipPartsDataBase.PartsType>.ToString(ShipPartsDataBase._partstype_enum_param, type, "불명");
    }

    public static ShipPartsDataBase.BuildTargetType ToBuildTargetType(int index)
    {
      return ShipPartsDataBase.ToBuildTargetType(index.ToString());
    }

    public static ShipPartsDataBase.BuildTargetType ToBuildTargetType(string name)
    {
      return EnumParserUtility<ShipPartsDataBase.BuildTargetType>.ToEnum(ShipPartsDataBase._buildtargettype_enum_param, name, ShipPartsDataBase.BuildTargetType.Hp);
    }

    public static string ToString(ShipPartsDataBase.BuildTargetType type)
    {
      return EnumParserUtility<ShipPartsDataBase.BuildTargetType>.ToString(ShipPartsDataBase._buildtargettype_enum_param, type, "불명");
    }

    public ShipPartsDataBase.ShipPart Find(string name)
    {
      foreach (ShipPartsDataBase.ShipPart shipPart in this.m_list)
      {
        if (shipPart.Name == name)
          return shipPart;
      }
      return (ShipPartsDataBase.ShipPart) null;
    }

    public ShipPartsDataBase.ShipPart Find(string name, ShipPartsDataBase.PartsType type)
    {
      ShipPartsDataBase.ShipPart shipPart = this.Find(name);
      if (shipPart == null)
        return (ShipPartsDataBase.ShipPart) null;
      if (shipPart.PartsType != type)
        return (ShipPartsDataBase.ShipPart) null;
      else
        return shipPart;
    }

    public bool IsParts(string name)
    {
      return this.Find(name) != null;
    }

    public List<ShipPartsDataBase.ShipPart> GetPartsList(ShipPartsDataBase.PartsType type)
    {
      List<ShipPartsDataBase.ShipPart> list = new List<ShipPartsDataBase.ShipPart>();
      foreach (ShipPartsDataBase.ShipPart shipPart in this.m_list)
      {
        if (shipPart.PartsType == type)
          list.Add(shipPart);
      }
      return list;
    }

    public List<ShipPartsDataBase.ShipPart> GetPartsList(ShipPartsDataBase.PartsType type, ShipPartsDataBase.BuildShipType type2)
    {
      return this.GetPartsList(type, ShipPartsDataBase.ToShipSizeType(type2));
    }

    public List<ShipPartsDataBase.ShipPart> GetPartsList(ShipPartsDataBase.PartsType type, ShipPartsDataBase.ShipSizeType type2)
    {
      List<ShipPartsDataBase.ShipPart> list = new List<ShipPartsDataBase.ShipPart>();
      foreach (ShipPartsDataBase.ShipPart shipPart in this.m_list)
      {
        if (shipPart.PartsType == type)
        {
          if (shipPart.ShipSizeType != ShipPartsDataBase.ShipSizeType.All)
          {
            if (shipPart.ShipSizeType == ShipPartsDataBase.ShipSizeType.MiddleAndBig)
            {
              if (type2 == ShipPartsDataBase.ShipSizeType.Small)
                continue;
            }
            else if (shipPart.ShipSizeType != type2)
              continue;
          }
          list.Add(shipPart);
        }
      }
      return list;
    }

    private void add(ShipPartsDataBase.PartsType type, ShipPartsDataBase.ShipSizeType ship_type1, string name, int min0, int max0, int min1, int max1, int min2, int max2, int min3, int max3, int min4, int max4, int min5, int max5, int min6, int max6, int min7, int max7, int min8, int max8, int min9, int max9)
    {
      this.m_list.Add(new ShipPartsDataBase.ShipPart(type, ship_type1, name, min0, max0, min1, max1, min2, max2, min3, max3, min4, max4, min5, max5, min6, max6, min7, max7, min8, max8, min9, max9));
    }

    private void add(ShipPartsDataBase.PartsType type, ShipPartsDataBase.ShipSizeType ship_type1, string name, int min0, int max0, int min1, int max1, int min2, int max2, int min3, int max3, int min4, int max4, int min5, int max5, int min6, int max6, int min7, int max7, int min8, int max8, int min9, int max9, bool is_general_purpose)
    {
      this.m_list.Add(new ShipPartsDataBase.ShipPart(type, ship_type1, name, min0, max0, min1, max1, min2, max2, min3, max3, min4, max4, min5, max5, min6, max6, min7, max7, min8, max8, min9, max9, is_general_purpose));
    }

    public static int GetBuildStep(ShipPartsDataBase.BuildTargetType type)
    {
      switch (type)
      {
        case ShipPartsDataBase.BuildTargetType.Hp:
          return 10;
        case ShipPartsDataBase.BuildTargetType.VSail:
        case ShipPartsDataBase.BuildTargetType.HSail:
          return 5;
        case ShipPartsDataBase.BuildTargetType.Row:
        case ShipPartsDataBase.BuildTargetType.Turn:
        case ShipPartsDataBase.BuildTargetType.Wave:
        case ShipPartsDataBase.BuildTargetType.Armor:
          return 1;
        default:
          return 2;
      }
    }

    public static int[] GetBuildMax(ShipPartsDataBase.BuildShipType type)
    {
      switch (type)
      {
        case ShipPartsDataBase.BuildShipType.Small1:
          return new int[10]
          {
            90,
            30,
            30,
            0,
            6,
            5,
            5,
            8,
            8,
            9
          };
        case ShipPartsDataBase.BuildShipType.Small2:
          return new int[10]
          {
            130,
            50,
            50,
            9,
            10,
            9,
            9,
            16,
            16,
            17
          };
        case ShipPartsDataBase.BuildShipType.Middle:
          return new int[10]
          {
            170,
            70,
            70,
            13,
            14,
            13,
            13,
            24,
            24,
            25
          };
        case ShipPartsDataBase.BuildShipType.Big1:
          return new int[10]
          {
            210,
            90,
            90,
            17,
            18,
            17,
            17,
            32,
            32,
            33
          };
        default:
          return new int[10]
          {
            250,
            110,
            110,
            21,
            22,
            21,
            21,
            40,
            40,
            41
          };
      }
    }

    public static ShipPartsDataBase.ShipSizeType ToShipSizeType(ShipPartsDataBase.BuildShipType type)
    {
      switch (type)
      {
        case ShipPartsDataBase.BuildShipType.Small1:
        case ShipPartsDataBase.BuildShipType.Small2:
          return ShipPartsDataBase.ShipSizeType.Small;
        case ShipPartsDataBase.BuildShipType.Middle:
          return ShipPartsDataBase.ShipSizeType.Middle;
        default:
          return ShipPartsDataBase.ShipSizeType.Big;
      }
    }

    private void load_xml(string file_name)
    {
      this.m_list.Clear();
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(file_name);
        if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.ChildNodes.Count <= 0)
          return;
        foreach (XmlNode n in xmlDocument.DocumentElement.ChildNodes)
        {
          if (n.Attributes["name"] != null)
          {
            ShipPartsDataBase.ShipPart shipPart = ShipPartsDataBase.ShipPart.FromXml(n);
            if (shipPart != null)
              this.m_list.Add(shipPart);
          }
        }
      }
      catch
      {
        this.m_list.Clear();
      }
    }

    private void write_xml(string file_name)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.AppendChild((XmlNode) xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string) null));
      xmlDocument.AppendChild((XmlNode) xmlDocument.CreateElement("ship_parts_root"));
      foreach (ShipPartsDataBase.ShipPart shipPart in this.m_list)
        shipPart.WriteXml((XmlNode) xmlDocument.DocumentElement);
      xmlDocument.Save(file_name);
    }

    public enum BuildShipType
    {
      Small1,
      Small2,
      Middle,
      Big1,
      Big2,
    }

    public enum ShipSizeType
    {
      Small,
      Middle,
      Big,
      All,
      MiddleAndBig,
    }

    public enum PartsType
    {
      Hull,
      Sail,
      Cannon,
      Other,
    }

    public enum BuildTargetType
    {
      Hp,
      VSail,
      HSail,
      Row,
      Turn,
      Wave,
      Armor,
      Cabin,
      CannonRoom,
      Storage,
    }

    public class DataValue
    {
      private int m_index;
      private int m_min;
      private int m_max;
      private int m_average;

      public int Index
      {
        get
        {
          return this.m_index;
        }
      }

      public int Min
      {
        get
        {
          return this.m_min;
        }
      }

      public int Max
      {
        get
        {
          return this.m_max;
        }
      }

      public int Average
      {
        get
        {
          return this.m_average;
        }
      }

      public bool HasValue
      {
        get
        {
          return this.m_min != 0 || this.m_max != 0 || this.m_average != 0;
        }
      }

      public DataValue(int index)
        : this(index, 0, 0, 0)
      {
      }

      public DataValue(int index, int min, int max, int average)
      {
        this.m_index = index;
        this.SetData(min, max, average);
      }

      public void SetData(int min, int max, int average)
      {
        this.m_min = min;
        this.m_max = max;
        this.m_average = average;
      }

      private string num_string(int num)
      {
        string str = "";
        if (num > 0)
          str = str + "+";
        return str + num.ToString();
      }

      public string ToStringSimple()
      {
        if (!this.HasValue)
          return "";
        else
          return this.num_string(this.Min) + "～" + this.num_string(this.Max);
      }

      public override string ToString()
      {
        if (!this.HasValue)
          return "";
        else
          return this.num_string(this.Min) + "～" + this.num_string(this.Average) + "～" + this.num_string(this.Max);
      }
    }

    public class ShipPart
    {
      private ShipPartsDataBase.PartsType m_type;
      private ShipPartsDataBase.ShipSizeType m_ship_size_type;
      private string m_name;
      private ShipPartsDataBase.DataValue[] m_value_tbl;
      private List<ShipPartsDataBase.DataValue> m_opt_value_tbl;
      private bool m_is_general_purpose;

      public string Skill { get; private set; }

      public string Citys { get; private set; }

      public string NPC { get; private set; }

      public string Materials { get; private set; }

      public string ShipType { get; private set; }

      public string Price { get; private set; }

      public string Name
      {
        get
        {
          return this.m_name;
        }
      }

      public ShipPartsDataBase.DataValue[] ValueTbl
      {
        get
        {
          return this.m_value_tbl;
        }
      }

      public List<ShipPartsDataBase.DataValue> OptValueTbl
      {
        get
        {
          return this.m_opt_value_tbl;
        }
      }

      public ShipPartsDataBase.PartsType PartsType
      {
        get
        {
          return this.m_type;
        }
      }

      public ShipPartsDataBase.ShipSizeType ShipSizeType
      {
        get
        {
          return this.m_ship_size_type;
        }
      }

      public bool IsGeneralPurpose
      {
        get
        {
          return this.m_is_general_purpose;
        }
      }

      private ShipPart()
      {
        this.m_type = ShipPartsDataBase.PartsType.Hull;
        this.m_ship_size_type = ShipPartsDataBase.ShipSizeType.All;
        this.m_name = "";
        this.m_value_tbl = new ShipPartsDataBase.DataValue[Enum.GetValues(typeof (ShipPartsDataBase.BuildTargetType)).Length];
        for (int index = 0; index < this.m_value_tbl.Length; ++index)
          this.m_value_tbl[index] = new ShipPartsDataBase.DataValue(index);
        this.m_opt_value_tbl = new List<ShipPartsDataBase.DataValue>();
        this.m_is_general_purpose = false;
        this.Skill = "";
        this.Citys = "";
        this.NPC = "";
        this.Materials = "";
        this.ShipType = "";
        this.Price = "";
      }

      public ShipPart(ShipPartsDataBase.PartsType type, ShipPartsDataBase.ShipSizeType ship_type1, string name, int min0, int max0, int min1, int max1, int min2, int max2, int min3, int max3, int min4, int max4, int min5, int max5, int min6, int max6, int min7, int max7, int min8, int max8, int min9, int max9)
        : this()
      {
        this.m_type = type;
        this.m_ship_size_type = ship_type1;
        this.m_name = name;
        this.m_value_tbl[0].SetData(min0, max0, max0 - (max0 - min0) / 2);
        this.m_value_tbl[1].SetData(min1, max1, max1 - (max1 - min1) / 2);
        this.m_value_tbl[2].SetData(min2, max2, max2 - (max2 - min2) / 2);
        this.m_value_tbl[3].SetData(min3, max3, max3 - (max3 - min3) / 2);
        this.m_value_tbl[4].SetData(min4, max4, max4 - (max4 - min4) / 2);
        this.m_value_tbl[5].SetData(min5, max5, max5 - (max5 - min5) / 2);
        this.m_value_tbl[6].SetData(min6, max6, max6 - (max6 - min6) / 2);
        this.m_value_tbl[7].SetData(min7, max7, max7 - (max7 - min7) / 2);
        this.m_value_tbl[8].SetData(min8, max8, max8 - (max8 - min8) / 2);
        this.m_value_tbl[9].SetData(min9, max9, max9 - (max9 - min9) / 2);
        this.optimize();
      }

      public ShipPart(ShipPartsDataBase.PartsType type, ShipPartsDataBase.ShipSizeType ship_type1, string name, int min0, int max0, int min1, int max1, int min2, int max2, int min3, int max3, int min4, int max4, int min5, int max5, int min6, int max6, int min7, int max7, int min8, int max8, int min9, int max9, bool is_general_purpose)
        : this(type, ship_type1, name, min0, max0, min1, max1, min2, max2, min3, max3, min4, max4, min5, max5, min6, max6, min7, max7, min8, max8, min9, max9)
      {
        this.m_is_general_purpose = is_general_purpose;
      }

      private void optimize()
      {
        this.m_opt_value_tbl.Clear();
        foreach (ShipPartsDataBase.DataValue dataValue in this.m_value_tbl)
        {
          if (dataValue.HasValue)
            this.m_opt_value_tbl.Add(dataValue);
        }
      }

      public override string ToString()
      {
        return this.Name + "\n" + this.ToStringParamsOnly();
      }

      public string ToStringParamsOnly()
      {
        string str1 = "강화도：\n";
        for (int index = 0; index < this.m_value_tbl.Length; ++index)
        {
          string str2 = this.m_value_tbl[index].ToStringSimple();
          if (!string.IsNullOrEmpty(str2))
            str1 = str1 + ShipPartsDataBase.ToString(ShipPartsDataBase.ToBuildTargetType(index)) + "：" + str2 + "\n";
        }
        string str3 = str1 + "배타입：" + this.ShipType + "\n" + "배크기：" + ShipPartsDataBase.ToString(this.ShipSizeType) + "\n";
        if (this.IsGeneralPurpose)
        {
          str3 = str3 + "판매항구：" + this.Citys + "\n" + "가격：" + this.Price;
        }
        else
        {
          if (!string.IsNullOrEmpty(this.Citys))
            str3 = str3 + "작성항구：" + this.Citys + "\n";
          if (!string.IsNullOrEmpty(this.NPC))
            str3 = str3 + "NPC：" + this.NPC + "\n";
          if (!string.IsNullOrEmpty(this.Skill))
            str3 = str3 + "스킬：" + this.Skill + "\n";
          if (!string.IsNullOrEmpty(this.Materials))
            str3 = str3 + "소재：" + this.Materials + "\n";
        }
        return str3;
      }

      public string AddExtendData(string[] split, string last_citys)
      {
        if (split.Length <= 1 || this.Name != split[1])
          return last_citys;
        if (this.IsGeneralPurpose)
        {
          if (split.Length <= 4)
            return last_citys;
          if (split[2] != "~")
            last_citys = split[2];
          this.Citys = last_citys.Replace("&br;", ", ");
          this.Citys = this.Citys.Trim();
          this.Price = split[3].Trim();
          this.ShipType = split[4].Trim();
        }
        else
        {
          if (split.Length <= 9)
            return last_citys;
          this.Skill = this.get_data(split[2], split[3]);
          if (split[4] != "~")
            last_citys = split[4];
          this.Citys = last_citys.Replace("&br;", ", ");
          this.Citys = this.Citys.Trim();
          this.NPC = split[5].Trim();
          this.Materials = this.get_data(split[6], split[7], split[8]);
          this.ShipType = split[9].Trim();
        }
        return last_citys;
      }

      private string get_data(params string[] data)
      {
        string str = "";
        foreach (string data1 in data)
        {
          string data2 = this.get_data(data1);
          if (data2 != null)
            str = str + data2 + ", ";
        }
        if (str != "")
          str = str.Remove(str.Length - 1);
        return str;
      }

      private string get_data(string data)
      {
        data = data.Trim();
        if (string.IsNullOrEmpty(data))
          return (string) null;
        if (data == "-")
          return (string) null;
        else
          return data;
      }

      internal static ShipPartsDataBase.ShipPart FromXml(XmlNode n)
      {
        if (n == null)
          return (ShipPartsDataBase.ShipPart) null;
        if (n.ChildNodes == null)
          return (ShipPartsDataBase.ShipPart) null;
        if (n.Name != "ship_part")
          return (ShipPartsDataBase.ShipPart) null;
        if (n.Attributes["name"] == null)
          return (ShipPartsDataBase.ShipPart) null;
        ShipPartsDataBase.ShipPart shipPart = new ShipPartsDataBase.ShipPart();
        shipPart.m_name = Useful.XmlGetAttribute(n, "name", shipPart.m_name);
        shipPart.m_type = ShipPartsDataBase.ToPartsType(Useful.XmlGetAttribute(n, "PartsType", ShipPartsDataBase.ToString(shipPart.m_type)));
        shipPart.m_ship_size_type = ShipPartsDataBase.ToShipSizeType(Useful.XmlGetAttribute(n, "ShipSizeType", ShipPartsDataBase.ToString(shipPart.m_ship_size_type)));
        shipPart.m_is_general_purpose = Useful.ToBool(Useful.XmlGetAttribute(n, "IsGeneralPurpose", shipPart.m_is_general_purpose.ToString()), shipPart.m_is_general_purpose);
        for (int index = 0; index < Enum.GetValues(typeof (ShipPartsDataBase.BuildTargetType)).Length; ++index)
        {
          XmlNode element = Useful.XmlGetElement(n, "data_values", ShipPartsDataBase.ToString(ShipPartsDataBase.ToBuildTargetType(index)));
          if (element != null)
          {
            int min = Useful.ToInt32(Useful.XmlGetAttribute(element, "Min", shipPart.m_value_tbl[index].Min.ToString()), shipPart.m_value_tbl[index].Min);
            int max = Useful.ToInt32(Useful.XmlGetAttribute(element, "Max", shipPart.m_value_tbl[index].Max.ToString()), shipPart.m_value_tbl[index].Max);
            shipPart.m_value_tbl[index].SetData(min, max, max - (max - min) / 2);
          }
        }
        XmlNode node = (XmlNode) n["detail"];
        if (node != null)
        {
          shipPart.Skill = Useful.XmlGetAttribute(node, "Skill", shipPart.Skill);
          shipPart.Citys = Useful.XmlGetAttribute(node, "Citys", shipPart.Citys);
          shipPart.NPC = Useful.XmlGetAttribute(node, "NPC", shipPart.NPC);
          shipPart.Materials = Useful.XmlGetAttribute(node, "Materials", shipPart.Materials);
          shipPart.ShipType = Useful.XmlGetAttribute(node, "ShipType", shipPart.ShipType);
          shipPart.Price = Useful.XmlGetAttribute(node, "Price", shipPart.Price);
        }
        shipPart.optimize();
        return shipPart;
      }

      internal void WriteXml(XmlNode p_node)
      {
        XmlNode xmlNode = Useful.XmlAddNode(p_node, "ship_part", this.Name);
        Useful.XmlAddAttribute(xmlNode, "PartsType", ShipPartsDataBase.ToString(this.m_type));
        Useful.XmlAddAttribute(xmlNode, "ShipSizeType", ShipPartsDataBase.ToString(this.m_ship_size_type));
        Useful.XmlAddAttribute(xmlNode, "IsGeneralPurpose", this.m_is_general_purpose.ToString());
        for (int index = 0; index < Enum.GetValues(typeof (ShipPartsDataBase.BuildTargetType)).Length; ++index)
        {
          XmlNode node = Useful.XmlAddNode(xmlNode, "data_values", ShipPartsDataBase.ToString(ShipPartsDataBase.ToBuildTargetType(index)));
          Useful.XmlAddAttribute(node, "Min", this.m_value_tbl[index].Min.ToString());
          Useful.XmlAddAttribute(node, "Max", this.m_value_tbl[index].Max.ToString());
        }
        XmlNode node1 = Useful.XmlAddNode(xmlNode, "detail");
        Useful.XmlAddAttribute(node1, "Skill", this.Skill);
        Useful.XmlAddAttribute(node1, "Citys", this.Citys);
        Useful.XmlAddAttribute(node1, "NPC", this.NPC);
        Useful.XmlAddAttribute(node1, "Materials", this.Materials);
        Useful.XmlAddAttribute(node1, "ShipType", this.ShipType);
        Useful.XmlAddAttribute(node1, "Price", this.Price);
      }
    }
  }
}
