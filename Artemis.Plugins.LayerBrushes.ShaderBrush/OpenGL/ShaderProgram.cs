using System;
using OpenTK.Graphics.OpenGL;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

public sealed class ShaderProgram : IDisposable
{
    #region Properties & Fields

    private bool _disposed = false;
    private readonly int _handle;

    public int Handle
    {
        get
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return _handle;
        }
    }

    #endregion

    #region Constructors

    public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        GL.CompileShader(vertexShader);

        string infoLogVert = GL.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrEmpty(infoLogVert))
            throw new ApplicationException(infoLogVert);

        GL.CompileShader(fragmentShader);

        string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrEmpty(infoLogFrag))
            throw new ApplicationException(infoLogFrag);

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        GL.LinkProgram(_handle);

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    ~ShaderProgram()
    {
        Dispose(false);
    }

    #endregion

    #region Methods

    public void Use()
    {
        if (_disposed) throw new ObjectDisposedException(GetType().FullName);

        GL.UseProgram(_handle);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_handle != nint.Zero)
                GL.DeleteProgram(_handle);
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}