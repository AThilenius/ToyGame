using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ToyGame.GUI
{
  /// <summary>
  ///   The base-most class for a GUI control, represents little more than the layout of the control and it's children and
  ///   input Chain Of Command handling.
  /// </summary>
  public partial class GuiElement
  {
    #region Fields / Properties

    public SSizing Width = new SAuto();
    public SSizing Height = new SAuto();
    public int MinWidth = 0;
    public int MinHeight = 0;
    public ChildAlignments ChildAlignment = ChildAlignments.Column;
    public BorderSize Margin = new BorderSize(0);
    public BorderSize Padding = new BorderSize(2);
    public BorderStyle Border = new BorderStyle(0, Color.Transparent);
    public List<GuiElement> Children = new List<GuiElement>();
    public Rectangle WorkingArea { get; private set; }

    #endregion

    public void LayoutChildren(Rectangle workingArea)
    {
      WorkingArea = workingArea;
      // Add Margin
      workingArea.X += Margin.Left;
      workingArea.Width -= Margin.Left + Margin.Right;
      workingArea.Y += Margin.Top;
      workingArea.Height -= Margin.Top + Margin.Bottom;
      // Agrograte a lot of things needed for child layout
      var heightTop = ChildAlignment == ChildAlignments.Column;
      var childrenWorkingAreas = Children.Select(c => new {c, d = new Rectangle()})
        .ToDictionary(kvp => kvp.c, kvp => kvp.d);
      var totalFixedMajor = Children.Select(c => c.ComputeMinLength(heightTop)).Sum();
      var avalableWeightedMajor = (heightTop ? workingArea.Height : workingArea.Width) - totalFixedMajor;
      var weightedChildren = Children.Where(c => (heightTop ? c.Height : c.Width) is SWeighted).ToArray();
      var childrenWeightsTotal =
        weightedChildren.Select(c => ((SWeighted) (heightTop ? c.Height : c.Width)).Weight).Sum();
      var majorOffset = (heightTop ? workingArea.Y : workingArea.X);
      foreach (var child in Children)
      {
        var childWorkingArea = childrenWorkingAreas[child];
        // Add in major axis offset and padding
        if (heightTop) childWorkingArea.Y = majorOffset + Padding.Top;
        else childWorkingArea.X = majorOffset + Padding.Left;
        // Add in minor axis offset and width with padding
        if (heightTop) childWorkingArea.X = workingArea.X + Padding.Left;
        else childWorkingArea.Y = workingArea.Y + Padding.Top;
        if (heightTop) childWorkingArea.Width = workingArea.Width - (Padding.Left + Padding.Right);
        else childWorkingArea.Height = workingArea.Height - (Padding.Top + Padding.Bottom);
        int majorLength;
        if ((heightTop ? child.Height : child.Width) is SFixed)
        {
          // If it's fixed, just let it be what ever length it wants
          majorLength = ((SFixed) (heightTop ? child.Height : child.Width)).Size;
        }
        else if ((heightTop ? child.Height : child.Width) is SWeighted)
        {
          // If it's weighted, compute it's width based on avalable weighted width and how much weight it has
          var weight = ((SWeighted) (heightTop ? child.Height : child.Width)).Weight;
          majorLength = (int) Math.Round((avalableWeightedMajor*weight)/childrenWeightsTotal);
        }
        else if ((heightTop ? child.Height : child.Width) is SAuto)
        {
          // The width becomes the accumulation of all child elements along that axes
          majorLength = child.ComputeMinLength(heightTop);
        }
        else throw new NotSupportedException();
        if (heightTop) childWorkingArea.Height = majorLength - (Padding.Top + Padding.Bottom);
        else childWorkingArea.Width = majorLength - (Padding.Left + Padding.Right);
        majorOffset += majorLength;
        child.LayoutChildren(childWorkingArea);
      }
    }

    public GuiElement GetTopmostElementAtPoint(Point point)
    {
      // Out of bounds check.
      if (point.X < WorkingArea.Left || point.Y < WorkingArea.Top || point.X > WorkingArea.Left + WorkingArea.Width ||
          point.Y > WorkingArea.Top + WorkingArea.Height) return null;
      // Return child that is top-most, or this if they are all out of bounds
      return Children.Select(c => c.GetTopmostElementAtPoint(point)).FirstOrDefault(c => c != null) ?? this;
    }

    /// <summary>
    ///   Computes the min length of the given axes for this control (taking into account all it's children)
    /// </summary>
    /// <param name="heightTop">If true, computes relative to height/top.</param>
    /// <returns>The minimum length the control 'wants' to be.</returns>
    private int ComputeMinLength(bool heightTop)
    {
      var axisLayout = heightTop ? Height : Width;
      if (axisLayout is SFixed)
      {
        // Fixed axes are always want the height / width they request
        return ((SFixed) axisLayout).Size;
      }
      if (axisLayout is SWeighted)
      {
        // Weighted axes can scale down to MinWidth, ignoring children
        return heightTop ? MinHeight : MinWidth;
      }
      if (axisLayout is SAuto)
      {
        // Auto axes are either their MinWidth or the accumulation of it's children
        return Math.Max(heightTop ? MinHeight : MinWidth, Children.Select(c => c.ComputeMinLength(heightTop)).Sum());
      }
      throw new NotSupportedException();
    }
  }
}