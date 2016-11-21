using UnityEngine;
using System.Collections.Generic;
using SimplexNoise;

public class CubeChunkFiller : ChunkFiller
{

    public string seed;

    public TerrainLayer[] layers;
    private Dictionary<Pos, Block[,,]> stored = new Dictionary<Pos, Block[,,]>();
    private int chunkSize;

    public int minChunkY = -32;
    public int maxChunkY = 32;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
        noise = new Noise(seed);

        layers = FindObjectOfType<LayerStore>().GetLayers(vm);

        Utils.ProfileCall(() =>
        {
            foreach (var layer in layers)
                layer.VmStart(this);
        }, "Initialize layers");
    }

    public override void FillChunk(Chunk chunk)
    {
        if (chunkSize == 0) chunkSize = chunk.chunkSize;

        Pos pos = chunk.pos;
        var chunk3d = (CubeChunk)chunk;

        Block[,,] storedValuesForChunk;
        if (stored.TryGetValue(pos, out storedValuesForChunk))
        {
            chunk3d.blocks = storedValuesForChunk;
            stored.Remove(pos);

            return;
        }

        for (pos.y = minChunkY; pos.y <= maxChunkY; pos.y += chunk.chunkSize)
        {
            stored.Add(pos, new Block[chunk.chunkSize, chunk.chunkSize, chunk.chunkSize]);
        }

        FillChunkColumn(pos, chunk.chunkSize);

        chunk3d.blocks = stored[chunk3d.pos];
        stored.Remove(chunk3d.pos);
    }

    private void FillChunkColumn(Pos columnPos, int chunkSize)
    {
        Pos pos = columnPos;

        for (pos.x = columnPos.x; pos.x < columnPos.x + chunkSize; pos.x++)
        {
            for (pos.z = columnPos.z; pos.z < columnPos.z + chunkSize; pos.z++)
            {
                FillColumn(pos.x, pos.z);
            }
        }
    }

    private void FillColumn(int x, int z)
    {
        int head = minChunkY;
        foreach (var layer in layers)
        {
            Utils.ProfileCall(() =>
            {
                head = layer.ApplyLayerCol(x, z, head);
            }, "Applying layer");
        }
    }

    /// <summary>
    /// Sets a column of chunks starting at startPlaceHeight and ending at endPlaceHeight.
    /// Usually faster than setting blocks one at a time from the layer.
    /// </summary>
    /// <param name="x">Column global x position</param>
    /// <param name="z">Column global z position</param>
    /// <param name="startPlaceHeight">First block's global y position</param>
    /// <param name="endPlaceHeight">Last block's global y position</param>
    /// <param name="blockToPlace">The block to fill this column with</param>
    public override void SetBlocks(int x, int z, int startPlaceHeight, int endPlaceHeight, int blockId)
    {
        Pos chunkPos = vm.components.chunks.GetChunkPos(new Pos(x, 0, z));

        // Loop through each chunk in the column
        for (chunkPos.y = minChunkY; chunkPos.y <= maxChunkY; chunkPos.y += chunkSize)
        {
            // And for each one loop through its height
            for (int y = 0; y < chunkSize; y++)
            {
                //and if this is above the starting height
                if (chunkPos.y + y >= startPlaceHeight)
                {
                    // And below or equal to the end height place the block using 
                    // localSetBlock which is faster than the non-local pos set block
                    if (chunkPos.y + y < endPlaceHeight)
                    {
                        //chunks[i].world.SetBlock(new BlockPos(x, y + chunks[i].pos.y, z), blockToPlace, false, false);
                        stored[chunkPos][x - chunkPos.x, y, z - chunkPos.z] = vm.components.blockLoader.CreateBlock(blockId);
                    }
                    else
                    {
                        // Return early, we've reached the end of the blocks to add
                        return;
                    }
                }
            }
        }
    }
}
