using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame.GUI
{
  public struct BorderSize
  {
    #region Fields / Properties

    public int Top;
    public int Right;
    public int Bottom;
    public int Left;

    #endregion

    public BorderSize(int top, int right, int bottom, int left)
    {
      Top = top;
      Right = right;
      Bottom = bottom;
      Left = left;
    }

    public BorderSize(int size)
    {
      Top = size;
      Right = size;
      Bottom = size;
      Left = size;
    }
  }

  public struct BorderStyle
  {
    #region Fields / Properties

    private BorderSize Size;
    private Color Color;
    private BorderSize BorderRadius;

    #endregion

    public BorderStyle(int borderSize, Color color)
    {
      Size = new BorderSize(borderSize);
      Color = color;
      BorderRadius = new BorderSize(0);
    }

    public BorderStyle(BorderSize size, Color color)
    {
      Size = size;
      Color = color;
      BorderRadius = new BorderSize(0);
    }
  }

  public abstract class SSizing
  {
    #region Fields / Properties

    public SizingTypes Type;

    #endregion

    protected SSizing(SizingTypes type)
    {
      Type = type;
    }
  }

  public class SFixed : SSizing
  {
    #region Fields / Properties

    public int Size;

    #endregion

    public SFixed(int size) : base(SizingTypes.Fixed)
    {
      Size = size;
    }

    public static explicit operator SFixed(int size)
    {
      return new SFixed(size);
    }
  }

  public class SWeighted : SSizing
  {
    #region Fields / Properties

    public float Weight;

    #endregion

    public SWeighted(float weight = 1.0f) : base(SizingTypes.Weighted)
    {
      Weight = weight;
    }
  }

  public class SFill : SWeighted
  {
    public SFill() : base(1.0f)
    {
    }
  }

  public class SAuto : SSizing
  {
    public SAuto() : base(SizingTypes.Auto)
    {
    }
  }

  public enum ChildAlignments
  {
    Row,
    Column,
    Explicit
  }

  public enum SizingTypes
  {
    Auto,
    Fill,
    Weighted,
    Fixed
  }
}
