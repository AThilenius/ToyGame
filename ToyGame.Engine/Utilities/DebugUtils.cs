using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Utilities
{
  internal static class DebugUtils
  {
    public static void GLErrorCheck()
    {
#if DEBUG
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        throw new Exception(error.ToString());
#endif
    }
  }
}