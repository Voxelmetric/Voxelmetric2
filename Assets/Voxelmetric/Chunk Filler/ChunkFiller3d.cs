using UnityEngine;
using System.Collections.Generic;
using SimplexNoise;

public class ChunkFiller3d : BaseChunkFiller {

    public string seed;

    public TerrainLayer[] layers;
    private Dictionary<Pos, Block[,,]> stored = new Dictionary<Pos, Block[,,]>();
    private int chunkSize;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
        noise = new Noise(seed);
        layers = new TerrainLayer[] {
            new SimpleLayer() {
                baseHeight = 30,
                frequency = 0.01f,
                amplitude = 4,
                absolute = true,
                blockId = vm.components.blockLoader.GetId("rock")
            },
            //new SimpleLayer() {
            //    baseHeight = 30,
            //    frequency = 0.1f,
            //    amplitude = 4,
            //    absolute = true,
            //    offset = 10000,
            //    blockId = vm.components.blockLoader.GetId("stone")
            //}
        };

        Utils.ProfileCall(() =>
        {
            foreach (var layer in layers)
                layer.VmStart(this);
        }, "Initialize layers");
    }

    public override void FillChunk(BaseChunk chunk)
    {
        if (chunkSize == 0) chunkSize = chunk.chunkSize;

        Pos pos = chunk.pos;
        var chunk3d = (Chunk3d)chunk;

        Block[,,] storedValuesForChunk;
        if (stored.TryGetValue(pos, out storedValuesForChunk))
        {
            chunk3d.blocks = storedValuesForChunk;
            stored.Remove(pos);

            return;
        }

        for (pos.y = -48; pos.y <= 64; pos.y += chunk.chunkSize)
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
        int head = -32;
        foreach (var layer in layers)
        {
            Utils.ProfileCall(() =>
            {
                head = layer.ApplyLayerCol(x, z, head);
            }, "Applying layer");
        }
    }

    //Sets a column of chunks starting at startPlaceHeight and ending at endPlaceHeight using localSetBlock for speed
    public override void SetBlocks(int x, int z, int startPlaceHeight, int endPlaceHeight, Block blockToPlace)
    {
        Pos chunkPos = vm.components.chunks.GetChunkPos(new Pos(x, 0, z));

        // Loop through each chunk in the column
        for (chunkPos.y = -32; chunkPos.y <= 64; chunkPos.y += chunkSize)
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
                        stored[chunkPos][x - chunkPos.x, y, z - chunkPos.z] = blockToPlace;
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
