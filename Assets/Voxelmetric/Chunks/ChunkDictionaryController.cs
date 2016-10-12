using UnityEngine;
using System.Collections.Generic;
using System;

public class ChunkDictionaryController : ChunkController {

    protected Dictionary<Pos, BaseChunk> chunks = new Dictionary<Pos, BaseChunk>();
    protected List<BaseChunk> chunkPool = new List<BaseChunk>();

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
    }

    public override BaseChunk CreateChunk(Pos pos)
    {
        pos = GetChunkPos(pos);
        BaseChunk chunk;

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

            chunk = chunkGO.GetComponent<BaseChunk>();
            chunk.transform.rotation = transform.rotation;
        }

        Utils.ProfileCall(() => { chunk.VmStart(pos, this); }, "Chunk vmStart");

        chunks.Add(pos, chunk);
        return chunk;
    }

    public override void Destroy(BaseChunk chunk)
    {
        chunk.Clear();
        chunks.Remove(chunk.pos);
        chunkPool.Add(chunk);
    }

    public override BaseChunk GetChunk(Pos chunkPos)
    {
        BaseChunk chunk = null;
        chunks.TryGetValue(GetChunkPos(chunkPos), out chunk);
        return chunk;
    }

    public override Block GetBlock(Pos blockPos)
    {
        BaseChunk chunk;
        Pos chunkPos = GetChunkPos(blockPos);

        if (chunks.TryGetValue(chunkPos, out chunk))
        {
            return chunk.GetBlock(blockPos);
        }
        else
        {
            return new Block(Block.VoidId);
        }

    }

    public override List<BaseChunk> GetChunks()
    {
        return new List<BaseChunk>(chunks.Values);
    }

}
