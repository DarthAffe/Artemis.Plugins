using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

public sealed class ShaderRenderer : IDisposable
{
    #region Constants

    private static readonly float[] QUAD_VERTS =
    {
        -1.0f, -1.0f,     0.0f, 0.0f,
        -1.0f, 1.0f,      0.0f, 1.0f,
        1.0f, -1.0f,      1.0f, 0.0f,

        1.0f, -1.0f,      1.0f, 0.0f,
        -1.0f, 1.0f,      0.0f, 1.0f,
        1.0f, 1.0f,       1.0f, 1.0f
    };

    private const string VERTEX_SHADER =
        @"#version 410 core
layout(location = 0) in vec2 position;
layout(location = 1) in vec2 inTexCoord;

                     out vec2 texCoord;

void main()
{
    texCoord = inTexCoord;
    gl_Position = vec4(position.xy, 0.0f, 1.0f);
}";

    #endregion

    #region Properties & Fields

    private bool _disposed = false;

    private readonly OpenGLThread _openGlThread;

    private int _framebuffer;
    private int _texture;
    private int _vertexArray;
    private int _vertexBuffer;
    private ShaderProgram _shader;

    private double _startTime;
    private double _lastFrameTime;
    private int _frame;

    private byte[] _pixelBuffer;

    public int Width { get; }
    public int Height { get; }
    public string Shader { get; }

    #endregion

    #region Constructors

    internal ShaderRenderer(OpenGLThread openGlThread, string fragmentShader, int width, int height)
    {
        this._openGlThread = openGlThread;
        this.Shader = fragmentShader;
        this.Width = width;
        this.Height = height;

        Initialize();
    }

    ~ShaderRenderer()
    {
        Dispose(false);
    }

    #endregion

    #region Methods

    private void Initialize()
    {
        Exception? exception = null;
        _openGlThread.Invoke(() =>
        {
            try
            {
                _shader = new ShaderProgram(VERTEX_SHADER, Shader);

                _vertexArray = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArray);

                _vertexBuffer = GL.GenBuffer();
                GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultWidth, Width);
                GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultHeight, Height);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, QUAD_VERTS.Length * sizeof(float), QUAD_VERTS, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), nint.Zero);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (nint)(2 + sizeof(float)));
                GL.EnableVertexAttribArray(1);
                GL.BindVertexArray(0);

                _framebuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

                _texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, Width, Height, 0, PixelFormat.Rgb,
                              PixelType.UnsignedByte, nint.Zero);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D,
                                        _texture, 0);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                _shader.Use();
                GL.Uniform2(GL.GetUniformLocation(_shader.Handle, ShaderInputs.RESOLUTION), 1, new float[] { Width, Height });
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        if (exception != null)
            throw exception;

        _pixelBuffer = new byte[Width * Height * 4];
    }

    public Span<byte> Update()
    {
        DateTime currentData = DateTime.Now;
        _openGlThread.Invoke(() =>
        {
            double currentFrameTime = GLFW.GetTime();
            double deltaTime = currentFrameTime - _lastFrameTime;
            _lastFrameTime = currentFrameTime;

            GL.Viewport(0, 0, Width, Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, ShaderInputs.TIME), (float)(currentFrameTime - _startTime));
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, ShaderInputs.TIME_DELTA), (float)deltaTime);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, ShaderInputs.FRAME_RATE), (float)(1000 / deltaTime));
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, ShaderInputs.FRAME), ++_frame);
            GL.Uniform4(GL.GetUniformLocation(_shader.Handle, ShaderInputs.DATE), 1, new float[] { currentData.Year, currentData.Month, currentData.Day, (float)new TimeSpan(currentData.Ticks).TotalSeconds });

            GL.BindVertexArray(_vertexArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.GetTextureImage(_texture, 0, PixelFormat.Bgra, PixelType.UnsignedByte, _pixelBuffer.Length, _pixelBuffer);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Finish();
        });

        return _pixelBuffer;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _openGlThread.Invoke(() =>
            {
                try { _shader.Dispose(); } catch { /**/ }
                try { GL.DeleteTexture(_texture); } catch { /**/ }
                try { GL.DeleteFramebuffer(_framebuffer); } catch { /**/ }
                try { GL.DeleteBuffer(_vertexBuffer); } catch { /**/ }
                try { GL.DeleteVertexArray(_vertexArray); } catch { /**/ }
            });

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