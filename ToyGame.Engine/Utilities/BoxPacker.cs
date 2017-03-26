using System;
using System.Drawing;

namespace ToyGame.Utilities
{
  /// <summary>
  ///   A super simple implementation of a super simple (and not very well optimized) box packing algorithum.
  /// </summary>
  public class BoxPacker
  {
    #region Fields / Properties

    public readonly int Width;
    public readonly int Height;
    public readonly int Margin;
    private int _shelfHeight;
    private int _shelfOffset;
    private int _currentWidthOffset;

    #endregion

    public BoxPacker(int width, int height, int margin = 2)
    {
      Width = width;
      Height = height;
      Margin = margin;
    }

    public Point? TryPackingBox(Size boxSize)
    {
      boxSize.Width += Margin*2;
      boxSize.Height += Margin*2;
      // Make sure it's not too tall or wide to fix no matter what we do
      if (_shelfOffset + boxSize.Height > Height || boxSize.Width > Width) return null;
      // See if it fits on the current row
      if (_currentWidthOffset + boxSize.Width <= Width)
      {
        // The shelf height is always the tallest item in the shelf
        _shelfHeight = Math.Max(_shelfHeight, boxSize.Height);
        _currentWidthOffset += boxSize.Width;
        return new Point((_currentWidthOffset - boxSize.Width) + Margin, _shelfOffset + Margin);
      }
      // Make sure it can fit on the new shelf
      if (_shelfOffset + _shelfHeight + boxSize.Height > Height) return null;
      // Move the shelf up
      _shelfOffset += _shelfHeight;
      // Put it in spot zero of the shelf, and we are done.
      _shelfHeight = boxSize.Height;
      _currentWidthOffset = boxSize.Width;
      return new Point(Margin, _shelfOffset + Margin);
    }
  }
}