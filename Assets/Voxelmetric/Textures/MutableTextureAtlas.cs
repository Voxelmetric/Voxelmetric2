using UnityEngine;
using System.Collections.Generic;
using System;

public class MutableTextureAtlas : TextureAtlas
{
    readonly int _texSize;
    int _currentX;
    int _currentY;
    readonly Vector2 normalisedTexSize;
    readonly Rect atlasRect;
    Dictionary<string, Rect> _textureRects = new Dictionary<string, Rect>();

    public MutableTextureAtlas(int textureSize, int atlasSize, TextureFormat format)
    {
        _texSize = textureSize;
        _texture = new Texture2D(atlasSize, atlasSize, format, false);
        _texture.filterMode = FilterMode.Point;

        atlasRect = new Rect(0, 0, atlasSize, atlasSize);
        float normalisedWidth = textureSize / (float)atlasSize;
        normalisedTexSize = new Vector2(normalisedWidth, normalisedWidth);
    }

    public Rect ReserveSpace(out int xPos, out int yPos)
    {
        xPos = _currentX;
        yPos = _currentY;
        Vector2 uvPoint = Rect.PointToNormalized(atlasRect, new Vector2(xPos, yPos));
        MoveNext();
        return new Rect(uvPoint, normalisedTexSize);
    }

    public Rect AddTexture(Texture2D tex, int xPos, int yPos, bool updateTextureImmediate)
    {
        if (tex == null)
            throw new System.ArgumentNullException();

        if (tex.width != _texSize || tex.height != _texSize)
        {
            TextureScale.Point(tex, _texSize, _texSize);
            tex.Apply();
        }
        _texture.SetPixels(xPos, yPos, _texSize, _texSize, tex.GetPixels());
        if (updateTextureImmediate)
        {
            _texture.Apply();
        }

        Vector2 uvPoint = Rect.PointToNormalized(atlasRect, new Vector2(xPos, yPos));
        _textureRects[tex.name] = new Rect(uvPoint, normalisedTexSize);
        return _textureRects[tex.name];
    }

    public void Fill(Color color)
    {
        Color[] colors = _texture.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        _texture.SetPixels(colors);
        _texture.Apply();

    }

    public Rect AddTexture(Texture2D tex, bool updateTextureImmediate = true)
    {
        int targetX = _currentX;
        int targetY = _currentY;
        bool moveNext = true;
        if (_textureRects.ContainsKey(tex.name))
        {
            targetX = (int)_textureRects[tex.name].x;
            targetY = (int)_textureRects[tex.name].y;
            moveNext = false;
        }


        Rect result = AddTexture(tex, targetX, targetY, updateTextureImmediate);
        if (moveNext)
        {
            MoveNext();
        }
        return result;
    }

    private void MoveNext()
    {
        _currentX += _texSize;
        if (_currentX >= atlasRect.width)
        {
            _currentX = 0;
            _currentY += _texSize;
        }
    }

    public Rect[] AddTextures(params Texture2D[] textures)
    {
        List<Rect> result = new List<Rect>();

        foreach (Texture2D tex in textures)
        {
            if (tex == null)
                continue;

            result.Add(AddTexture(tex, false));

        }
        _texture.Apply();
        return result.ToArray();
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

    public override List<String> GetTextureNames()
    {
        return new List<String>(_textureRects.Keys);
    }
}
