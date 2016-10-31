using UnityEngine;
using System.Collections.Generic;

public class TextureSetLoader: MonoBehaviour
{
    public string pathToTextureResources = "textures";
    public Material material;
    [HideInInspector]
    public TextureAtlas mainAtlas;

    protected Dictionary<string, TextureSet> textureSets = new Dictionary<string, TextureSet>();

    public virtual void LoadTextures(Voxelmetric vm)
    {
        var resourceTextures = Resources.LoadAll<Texture2D>(pathToTextureResources);
        ImmutableTextureAtlas atlas = new ImmutableTextureAtlas(resourceTextures, 8192);

        for (int i = 0; i < resourceTextures.Length; i++)
        {
            TextureSet tex = new TextureSet(resourceTextures[i].name);
            tex.AddTexture(atlas.GetTextureRect(resourceTextures[i].name));
            AddTexture(tex);
        }

        mainAtlas = atlas;
        material.mainTexture = (Texture2D)atlas;

        Debug.Log(string.Format("Voxelmetric Texture Set Loader loaded {0} textures into {1} texture sets.", resourceTextures.Length, textureSets.Keys.Count));
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
