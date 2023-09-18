namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

public class ShaderInputs
{
    /// <summary>
    /// vec3, viewport resolution (in pixels)
    /// </summary>
    internal const string RESOLUTION = "iResolution";

    /// <summary>
    /// float, shader playback time (in seconds)
    /// </summary>
    internal const string TIME = "iTime";

    /// <summary>
    /// float, render time (in seconds)
    /// </summary>
    internal const string TIME_DELTA = "iTimeDelta";

    /// <summary>
    /// float, shader frame rate
    /// </summary>
    internal const string FRAME_RATE = "iFrameRate";

    /// <summary>
    /// int, shader playback frame
    /// </summary>
    internal const string FRAME = "iFrame";

    /// <summary>
    /// vec4, (year, month, day, time in seconds)
    /// </summary>
    internal const string DATE = "iDate";
}