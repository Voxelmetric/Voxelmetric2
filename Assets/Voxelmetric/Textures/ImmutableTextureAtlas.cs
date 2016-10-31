using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class ImmutableTextureAtlas : TextureAtlas
{

    Dictionary<string, Rect> _textureRects = new Dictionary<string, Rect>();

    public ImmutableTextureAtlas(Texture2D[] textures, int maximumSize)
    {
        _texture = new Texture2D(64, 64) { filterMode = FilterMode.Point };
        Rect[] rects = _texture.PackTextures(textures, 0, maximumSize, false);
        for (int i = 0; i < textures.Length; i++)
        {
            _textureRects[textures[i].name] = (rects[i]);
        }
    }

    public override List<String> GetTextureNames()
    {
        return new List<String>(_textureRects.Keys);
    }

    public override Rect GetTextureRect(string name)
    {
        if (!_textureRects.ContainsKey(name))
        {
            Debug.LogError("There is no loaded texture by the name " + name);
            return new Rect();
        }

        return _textureRects[name];
    }
}
