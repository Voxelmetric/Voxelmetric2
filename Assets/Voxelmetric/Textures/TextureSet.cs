using UnityEngine;
using System.Collections.Generic;

public class TextureSet
{
    public string name;
    List<Rect> textures = new List<Rect>();

    public TextureSet(string name)
    {
        this.name = name;
    }

    public virtual void AddTexture(Rect texture)
    {
        textures.Add(texture);
    }

    public virtual Rect GetTexture(BaseChunk chunk, Pos blockPos, Direction direction)
    {
        if (textures.Count == 0)
        {
            Debug.LogError("Block texture object is empty but something is requesting a texture!");
            return new Rect();
        }

        if (textures.Count == 1)
            return textures[0];

        System.Random random = new System.Random(blockPos.GetHashCode());
        int randomNumber = random.Next(0, textures.Count);
        return textures[randomNumber];
    }
}
