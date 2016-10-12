using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockType
{
    public Voxelmetric vm;

    public string blockName;
    public ushort id;

    public bool solid;
    public bool canBeWalkedOn;
    public bool canBeWalkedThrough;

    public virtual void Initialize(Voxelmetric vm)
    {
        this.vm = vm;
    }

    public virtual string GetName(BaseChunk chunk, Pos pos, Block block)
    {
        return blockName;
    }

    public void Render(BaseChunk chunk, Pos pos, Block block, ref MeshData meshData)
    {
        PreRender(chunk, pos, block);
        AddMeshData(chunk, pos, block, ref meshData);
        PostRender(chunk, pos, block);
    }

    public virtual Block OnCreate(BaseChunk chunk, Pos pos, Block block)
    {
        return block;
    }

    public virtual void OnDestroy(BaseChunk chunk, Pos pos, Block block, int destroyer) { }
    public virtual void RandomUpdate(BaseChunk chunk, Pos pos, Block block) { }

    protected virtual void PreRender(BaseChunk chunk, Pos pos, Block block) { }
    protected virtual void AddMeshData(BaseChunk chunk, Pos pos, Block block, ref MeshData meshData) { }
    protected virtual void PostRender(BaseChunk chunk, Pos pos, Block block) { }

    public virtual bool IsSolid(BaseChunk chunk, Pos pos, Block block, Direction direction)
    {
        return solid;
    }

    public virtual byte[] SerializeBlock(Block block)
    {
        return new byte[0];
    }

    public virtual Block DeserializeData(byte[] data)
    {
        return new Block();
    }

}
