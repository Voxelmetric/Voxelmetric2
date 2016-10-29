using UnityEngine;
using System.Collections.Generic;

public abstract class TextureAtlas
{
    protected Texture2D _texture;

    public int width
    {
        get
        {
            return _texture.width;
        }
    }

    public int height
    {
        get
        {
            return _texture.height;
        }
    }

    public static explicit operator Texture2D(TextureAtlas d)
    {
        return d._texture;
    }

    public abstract Rect GetTextureRect(string name);

    public abstract List<string> GetTextureNames();
}
