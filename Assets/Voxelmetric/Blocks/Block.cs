using System;
using UnityEngine;
using System.Collections.Generic;

public struct Block {
   
    public Block(int id)
    {
        this.id = (ushort)id;
        data = new byte[8];
        textureSet = null;
    }

    public ushort id;
    public byte[] data;
    public TextureSet textureSet;

    /// <summary>
    /// Used to access the block's attributes.
    /// </summary>
    public BlockType GetBlockType(Voxelmetric vm)
    {
        return vm.components.blockLoader.BlockAttr(id);
    }

    public override string ToString() { return "Block " + id; }

    // These block types are reserved and always loaded with these ids.
    // The Block type loader MUST implement these two with these ids
    public static readonly ushort VoidId = 1;
    public static readonly ushort AirId = 0;
}
