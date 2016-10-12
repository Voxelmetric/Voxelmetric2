using UnityEngine;
using System.Collections.Generic;

public class ChunkController : MonoBehaviour {

    internal Voxelmetric vm;

    [SerializeField]
    private int _chunkPower = 4;

    [HideInInspector]
    public int chunkSize;
    [HideInInspector]
    public int chunkPower;

    [SerializeField]
    private float _blockSize = 1;
    [HideInInspector]
    public float blockSize;

    public virtual void Initialize(Voxelmetric vm)
    {
        this.vm = vm;
        chunkPower = _chunkPower;
        chunkSize = 1 << chunkPower;
        blockSize = _blockSize;
    }

    public void Destroy(Pos chunkPos)
    {
        BaseChunk chunk = GetChunk(chunkPos);
        if (chunk != null) Destroy(chunk);
    }

    public virtual void Destroy(BaseChunk chunk) { }

    public virtual BaseChunk GetChunk(Pos chunkPos)
    {
        return null;
    }

    public virtual Block GetBlock(Pos blockPos)
    {
        return new Block();
    }

    public virtual void SetBlock(Pos blockPos, Block block)
    {
        BaseChunk chunk = GetChunk(blockPos);

        if (chunk != null)
        {
            chunk.SetBlock(block, blockPos);
        }
    }

    public virtual BaseChunk CreateChunk(Pos pos)
    {
        return null;
    }

    public virtual void SafePlaceBlock(Pos blockPos, Block block)
    {
        BaseChunk chunk = GetChunk(blockPos);

        if (chunk == null)
        {
            chunk = CreateChunk(blockPos);
        }

        chunk.SetBlock(block, blockPos);
    }

    public virtual Pos GetBlockPos(Vector3 point)
    {
        point -= transform.position;
        point = Quaternion.Inverse(transform.rotation) * point;

        point /= vm.components.chunks.blockSize;

        return new Pos(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.z));
    }

    public virtual Vector3 GetWorldPos(Pos blockPos)
    {
        return (transform.rotation * blockPos) + transform.position;
    }

    public virtual Pos GetChunkPos(Pos blockPos)
    {
        return new Pos((blockPos.x >> chunkPower) << chunkPower,
                               (blockPos.y >> chunkPower) << chunkPower,
                               (blockPos.z >> chunkPower) << chunkPower);
    }

    public virtual List<BaseChunk> GetChunks()
    {
        return new List<BaseChunk>();
    }

    public virtual byte[] Serialize(byte storeMode)
    {
        return new byte[0];
    }

    public virtual void Deserialize(byte[] data) { }

    public void ClearAllStaleBlocks()
    {
        foreach (var chunk in GetChunks())
        {
            chunk.ClearStaleBlocks();
        }
    }
}
