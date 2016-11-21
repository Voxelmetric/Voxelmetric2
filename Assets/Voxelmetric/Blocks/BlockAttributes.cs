using System;
using UnityEngine;
using System.Collections;

public class BlockAttributes
{
    public Voxelmetric vm;

    public string blockName;
    public ushort id;
    public bool isSolid;
    public bool isStatic;
    public TextureSet textureSet;

    public Type blockType;

    public virtual void Initialize(Voxelmetric vm)
    {
        this.vm = vm;
    }

    public virtual Type GetBlockType()
    {
        return blockType;
    }
}
