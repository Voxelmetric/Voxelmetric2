using UnityEngine;
using System.Collections.Generic;
using System;

public class ChunkDictController : ChunkController {

    protected Dictionary<Pos, Chunk> chunks = new Dictionary<Pos, Chunk>();
    protected List<Chunk> chunkPool = new List<Chunk>();

    [Tooltip("If SetBlock's coordinates are within an unloaded chunk, load the chunk and apply the set block command.")]
    public bool SetBlockCreateMissingChunks;
    [Tooltip("If GetBlock's coordinates are within an unloaded chunk, load the chunk and return the block at the coordinates.")]
    public bool GetBlockCreateMissingChunks;

    public int minChunkY = -32;
    public int maxChunkY = 64;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
    }

    public override Chunk CreateChunk(Pos pos)
    {
        pos = GetChunkPos(pos);
        if (pos.y > maxChunkY || pos.y < minChunkY) return null;

        Chunk chunk;

        if (chunks.ContainsKey(pos))
        {
            return chunks[pos];
        }

        if (chunkPool.Count != 0)
        {
            chunk = chunkPool[0];
            chunkPool.RemoveAt(0);
        }
        else
        {
            GameObject chunkGO = new GameObject("Chunk at " + pos, new Type[] {
                vm.components.chunkType.GetType()
            });

            chunk = chunkGO.GetComponent<Chunk>();
            chunk.transform.rotation = transform.rotation;
        }

        Utils.ProfileCall(() => { chunk.VmStart(pos, this); }, "Chunk vmStart");

        chunks.Add(pos, chunk);
        return chunk;
    }

    public override void Destroy(Chunk chunk)
    {
        chunk.Clear();
        chunks.Remove(chunk.pos);
        chunkPool.Add(chunk);
    }

    public override Chunk GetChunk(Pos chunkPos)
    {
        Chunk chunk = null;
        chunks.TryGetValue(GetChunkPos(chunkPos), out chunk);
        return chunk;
    }

    public override Block GetBlock(Pos blockPos)
    {
        Chunk chunk = null;
        Pos chunkPos = GetChunkPos(blockPos);
        chunks.TryGetValue(chunkPos, out chunk);

        if (chunk == null) // If the chunk does not exist
        {
            // If the GetBlockCreateMissingChunks is true
            // create the chunk the block was requested for
            if (GetBlockCreateMissingChunks)
            {
                chunk = CreateChunk(blockPos);
            }
        }

        if(chunk == null) return new Block(Block.VoidId);

        // Use the chunk's GetBlock function to return the block
        return chunk.GetBlock(blockPos);

    }

    public override List<Chunk> GetChunks()
    {
        return new List<Chunk>(chunks.Values);
    }

}
