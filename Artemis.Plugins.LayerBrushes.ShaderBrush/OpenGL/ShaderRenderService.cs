using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

public unsafe class ShaderRenderService : IDisposable
{
    #region Constants

    public const string SHADER_TEMPLATE =
        @"#version 410 core
     in  vec2 texCoord;
uniform  vec2 iResolution;
uniform float iTime;
uniform float iTimeDelta;
uniform float iFrameRate;
uniform float iFrame;
uniform  vec4 iDate;

    out  vec4 fragColor;
         vec2 fragCoord = gl_FragCoord.xy;

void main()
{
    fragColor = vec4(1.0, 1.0, 1.0, 1.0);
}";

    #endregion

    #region Properties & Fields

    private static int _idCounter = 0;

    private bool _disposed = false;

    private readonly OpenGLThread _openGLThread = new();
    private readonly Dictionary<ShaderEntry, ShaderRenderer> _renderer = new();

    private NativeWindow? _window;

    #endregion

    #region Constructors

    public ShaderRenderService()
    {
        Initialize();
    }

    ~ShaderRenderService()
    {
        Dispose(false);
    }

    #endregion

    #region Methods

    public ShaderEntry RegisterShader(string shader, int width, int height)
    {
        ShaderEntry entry = new(++_idCounter, shader, width, height);

        if (_renderer.Count == 0)
            Initialize();

        _renderer.Add(entry, new ShaderRenderer(_openGLThread, shader, width, height));

        return entry;
    }

    public bool UnregisterShader(ShaderEntry entry)
    {
        bool result = _renderer.Remove(entry, out ShaderRenderer? renderer);
        if (result) renderer!.Dispose();

        if (result && (_renderer.Count == 0))
            Terminate();

        return result;
    }

    public Span<byte> Update(ShaderEntry entry)
    {
        if (!_renderer.TryGetValue(entry, out ShaderRenderer? renderer))
            throw new ArgumentException("The provided entry is not registered on this RenderService");

        return renderer.Update();
    }

    private void Initialize()
    {
        _openGLThread.Invoke(() =>
        {
            GLFW.Init();

            NativeWindowSettings windowSettings = NativeWindowSettings.Default;
            windowSettings.WindowBorder = WindowBorder.Hidden;
            windowSettings.StartVisible = false;
            windowSettings.StartFocused = false;
            windowSettings.Flags = ContextFlags.Offscreen;
            windowSettings.Title = "Artemis - Shaders";
            windowSettings.Size = new Vector2i(1, 1);
            windowSettings.Location = new Vector2i(0, 0);

            _window = new NativeWindow(windowSettings);
            GLFW.WindowHint(WindowHintBool.Visible, false);

            GLFW.MakeContextCurrent(_window.WindowPtr);
        });
    }

    private void Terminate()
    {
        if (_window == null) return;

        _openGLThread.Invoke(() =>
        {
            GLFW.DestroyWindow(_window.WindowPtr);
            GLFW.Terminate();
        });
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            try
            {
                foreach (ShaderEntry renderEntries in _renderer.Keys.ToList())
                    UnregisterShader(renderEntries);
            }
            catch { /**/ }

            try { Terminate(); } catch { /**/ }

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