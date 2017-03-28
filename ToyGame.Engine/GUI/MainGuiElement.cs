using System;
using System.Drawing;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.GUI
{
  /// <summary>
  ///   A specialized subclass of GuiElement for managing the main element (that is spanned across the entire screen)
  /// </summary>
  public class MainGuiElement : GuiElement, IRenderable
  {
    #region Fields / Properties

    public GuiElement FocusedElement { get; private set; }
    private static GLMesh _glMesh;
    private static GLGuiShader _guiShader;
    private readonly Window _window;
    private GLTexture _glTexture;
    private GLVertexArrayObject _vertexArrayObject;
    private GuiElement _lastMouseOver;
    private Bitmap _bitmap;

    #endregion

    public MainGuiElement(Window window)
    {
      _window = window;
      // Setup static mesh if needed, note that it's in screen-space coordinates as no transformation is applied
      _glMesh = _glMesh ??
                new GLMesh(
                  new[] {new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0)},
                  new uint[] {0, 1, 3, 1, 2, 3},
                  new[] {Vector3.Zero, Vector3.UnitX, new Vector3(1, 1, 0), Vector3.UnitY});
      // Setup static shader if needed. Normally this would be managed by a material, but this is a one-off exception
      if (_guiShader == null)
      {
        _guiShader = new GLGuiShader();
        _guiShader.GpuAllocateDeferred();
      }
      // Setup defaults
      Width = new SFixed(window.NativeWindow.Width);
      Height = new SFixed(window.NativeWindow.Height);
      ChildAlignment = ChildAlignments.Row;
      // Create the default bitmap
      _bitmap = new Bitmap(_window.NativeWindow.Width, _window.NativeWindow.Height);
      // Register NativeWindow events
      window.NativeWindow.KeyDown += (sender, args) => FocusedElement?.OnKeyDown(args);
      window.NativeWindow.KeyUp += (sender, args) => FocusedElement?.OnKeyUp(args);
      window.NativeWindow.Resize += (sender, args) =>
      {
        _bitmap.Dispose();
        _bitmap = new Bitmap(_window.NativeWindow.Width, _window.NativeWindow.Height);
      };
      window.NativeWindow.MouseDown += (sender, args) =>
      {
        var clickedElement = GetTopmostElementAtPoint(args.Position);
        if (clickedElement == FocusedElement) return;
        // De-Focus last focused element
        FocusedElement?.OnLostFocus();
        // Focus the new one
        FocusedElement = clickedElement;
        FocusedElement.OnFocus();
      };
      window.NativeWindow.MouseUp += (sender, args) =>
      {
        var clickedElement = GetTopmostElementAtPoint(args.Position);
        // If the mouse is still within the control's bounds fire click event
        if (clickedElement == FocusedElement) FocusedElement?.OnClicked(args);
        // TODO: Fire the drag release event here
        clickedElement?.OnMouseUp(args);
      };
      window.NativeWindow.MouseMove += (sender, args) =>
      {
        var mouseOverElement = GetTopmostElementAtPoint(args.Position);
        if (mouseOverElement != _lastMouseOver) _lastMouseOver?.OnMouseLeave();
        _lastMouseOver = mouseOverElement;
        mouseOverElement?.OnMouseEnter();
      };
      window.NativeWindow.MouseWheel += (sender, args) => FocusedElement?.OnMouseWheel(args);
      window.NativeWindow.FocusedChanged += (sender, args) =>
      {
        if (window.NativeWindow.Focused)
        {
          // Reforcused
          FocusedElement?.OnFocus();
        }
        else
        {
          // Unfocused
          FocusedElement?.OnLostFocus();
          _lastMouseOver?.OnMouseLeave();
        }
      };
    }

    public void EnqueueDrawCalls(GLDrawCallBatch drawCallbatch)
    {
      // TODO: GLTexture updating needs to be made double-buffered rather than creating a copy of the entire image every time.
      using (var graphics = Graphics.FromImage(_bitmap))
      {
        graphics.Clear(Color.FromArgb(255, 45, 45, 48));
        Draw(graphics);
      }
      // Re-upload texutre to GPU
      if (_glTexture == null)
      {
        _glTexture = _glTexture ?? new GLTexture(GLTextureParams.Default, _bitmap);
      }
      else
      {
        _glTexture.Update(_bitmap);
      }
      _glTexture.GpuAllocateDeferred();
      if (_vertexArrayObject == null)
      {
        _vertexArrayObject = new GLVertexArrayObject(_glMesh, _guiShader);
        _vertexArrayObject.GpuAllocateDeferred();
      }
      drawCallbatch.AddDrawCall(new GLDrawCall(_guiShader,
        new[] {new GLDrawCall.GLTextureBind(_glTexture, TextureTarget.Texture2D, TextureUnit.Texture0)},
        _vertexArrayObject, new GLDrawCall.UniformBind[0],
        () =>
          GL.DrawElements(PrimitiveType.Triangles, _glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
            IntPtr.Zero)));
    }
  }
}