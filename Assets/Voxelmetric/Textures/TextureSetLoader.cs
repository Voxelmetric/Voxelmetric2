using UnityEngine;
using System.Collections.Generic;

public class TextureSetLoader: MonoBehaviour
{
    public string pathToTextureResources = "textures";
    public Material material;
    [HideInInspector]
    public Texture2D tilesheet;

    protected Dictionary<string, TextureSet> textureSets = new Dictionary<string, TextureSet>();

    public virtual void LoadTextures(Voxelmetric vm)
    {
        var resourceTextures = Resources.LoadAll<Texture2D>(pathToTextureResources);
        Texture2D packedTextures = new Texture2D(64, 64) { filterMode = FilterMode.Point };
        Rect[] rects = packedTextures.PackTextures(resourceTextures, 0, 8192, false);

        for (int i = 0; i < resourceTextures.Length; i++)
        {
            TextureSet tex = new TextureSet(resourceTextures[i].name);
            tex.AddTexture(rects[i]);
            AddTexture(tex);
        }

        tilesheet = packedTextures;
        material.mainTexture = packedTextures;

        Debug.Log(string.Format("Voxelmetric Texture Set Loader loaded {0} textures into {1} texture sets.", rects.Length, textureSets.Keys.Count));
    }

    public virtual void AddTexture(TextureSet textureSet)
    {
        textureSets.Add(textureSet.name, textureSet);
    }

    public virtual TextureSet GetByName(string name)
    {
        if (!textureSets.ContainsKey(name))
        {
            Debug.LogError("There is no loaded texture by the name " + name);
            return null;
        }

        return textureSets[name];
    }
}
