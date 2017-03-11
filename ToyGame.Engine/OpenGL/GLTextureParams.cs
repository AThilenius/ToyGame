﻿using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL
{
  internal class GLTextureParams
  {
    public static readonly GLTextureParams Default = new GLTextureParams();
    public bool GenerateMipMaps = true;
    public All MagFilter = All.Linear;
    public All MinFilter = All.LinearMipmapLinear;
    public TextureTarget Target = TextureTarget.Texture2D;
    public bool UseAnisotropicFiltering = true;
    public All WrapS = All.Repeat;
    public All WrapT = All.Repeat;
  }
}