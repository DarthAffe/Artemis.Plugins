namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

public readonly struct ShaderEntry
{
    #region Properties & Fields

    public readonly int Id;
    public readonly string Shader;
    public readonly int Width;
    public readonly int Height;

    #endregion

    #region Constructors

    internal ShaderEntry(int id, string shader, int width, int height)
    {
        this.Id = id;
        this.Shader = shader;
        this.Width = width;
        this.Height = height;
    }

    #endregion

    #region Operators

    public static bool operator ==(ShaderEntry left, ShaderEntry right) => left.Equals(right);
    public static bool operator !=(ShaderEntry left, ShaderEntry right) => !left.Equals(right);

    #endregion

    #region Methods

    public bool Equals(ShaderEntry other) => Id == other.Id;
    public override bool Equals(object? obj) => obj is ShaderEntry other && Equals(other);

    public override int GetHashCode() => Id;

    #endregion
}