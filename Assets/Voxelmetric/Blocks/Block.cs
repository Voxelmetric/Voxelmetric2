using System;
using UnityEngine;
using System.Collections.Generic;

public class Block {

    public ushort id { get { return attributes.id; } }

    public BlockAttributes attributes;

    public override string ToString() { return GetName(); }

    public virtual bool CanBeWalkedOn()
    {
        return IsSolid(Direction.up);
    }

    public virtual bool CanBeWalkedThrough() {
        return !IsSolid(Direction.up);
    }

    public virtual string GetName()
    {
        return attributes.blockName;
    }

    public void Render(Chunk chunk, Pos pos, Block block, MeshData meshData)
    {
        PreRender(chunk, pos);
        AddMeshData(chunk, pos, ref meshData);
        PostRender(chunk, pos);
    }

    public virtual void OnCreate(Chunk chunk, Pos pos) { }

    public virtual void OnDestroy(Chunk chunk, Pos pos, int destroyer) { }
    public virtual void RandomUpdate(Chunk chunk, Pos pos) { }

    protected virtual void PreRender(Chunk chunk, Pos pos) { }
    protected virtual void AddMeshData(Chunk chunk, Pos pos, ref MeshData meshData) { }
    protected virtual void PostRender(Chunk chunk, Pos pos) { }

    public virtual bool IsSolid(Direction direction)
    {
        return attributes.isSolid;
    }

    public virtual byte[] SerializeBlock(Block block)
    {
        throw new NotImplementedException();
    }

    public virtual Block DeserializeData(byte[] data)
    {
        throw new NotImplementedException();
    }

    // These block types are reserved and always loaded with these ids.
    // The Block type loader MUST implement these two with these ids
    public static readonly ushort VoidId = 1;
    public static readonly ushort AirId = 0;
}
