using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class LoadChunks : MonoBehaviour
{

    public Voxelmetric vm;
    public bool generateTerrain = true;
    public string layerFolder;
    private Pos objectPos;

    [Range(1, 64)]
    public int chunkLoadRadius = 8;
    Pos[] chunkPositions;

    //The distance is measured in chunks
    [Range(1, 64)]
    public int DistanceToDeleteChunks = (int)(8 * 1.25f);
    private int distanceToDeleteInUnitsSquared;

    //Every WaitBetweenDeletes frames load chunks will stop to remove chunks beyond DistanceToDeleteChunks
    [Range(1, 100)]
    public int WaitBetweenDeletes = 10;

    //Every frame LoadChunks is not deleting chunks or finding chunks it will 
    [Range(1, 16)]
    public int ChunksToLoadPerFrame = 4;

    // Loads the top chunk of the column on its own rather than along with the ChunksToLoadPerFrame other chunks
    // This is useful because world generates the terrain for the column when the top chunk is loaded so this gives
    // the terrain generation a frame on its own
    public bool RenderChunksInSeparateFrame = true;
    List<Pos> chunksToRender = new List<Pos>();

    int deleteTimer = 0;
    List<Pos> chunksToGenerate = new List<Pos>();
    int chunkSize;
    ChunkController chunks;

    void Start()
    {
        chunks = vm.components.chunks;
        chunkSize = chunks.chunkSize;
        chunkPositions = ChunkLoadOrder.ChunkPositions(chunkLoadRadius);
        distanceToDeleteInUnitsSquared = (int)(DistanceToDeleteChunks * chunkSize * chunks.blockSize);
        distanceToDeleteInUnitsSquared *= distanceToDeleteInUnitsSquared;
    }

    // Update is called once per frame
    void Update()
    {
        objectPos = transform.position;

        if (deleteTimer == WaitBetweenDeletes)
        {
            DeleteChunks();

            deleteTimer = 0;
            return;
        }
        else
        {
            deleteTimer++;
        }

        if (chunksToRender.Count != 0)
        {
            for (int i = 0; i < ChunksToLoadPerFrame; i++)
            {
                if (chunksToRender.Count == 0)
                {
                    break;
                }

                Pos pos = chunksToRender[0];
                var chunk = chunks.GetChunk(pos);
                if(chunk != null) chunk.Render();
                chunksToRender.RemoveAt(0);
            }

            if (RenderChunksInSeparateFrame)
            {
                return;
            }
        }

        if (chunksToGenerate.Count == 0)
        {
            FindChunksAndLoad();
        }

        for (int i = 0; i < ChunksToLoadPerFrame; i++)
        {
            if (chunksToGenerate.Count == 0)
            {
                break;
            }

            Pos pos = chunksToGenerate[0];
            chunks.CreateChunk(pos);

            chunksToGenerate.RemoveAt(0);
            chunksToRender.Add(pos);
        }

    }

    void DeleteChunks()
    {
        int posX = objectPos.x;
        int posZ = objectPos.z;

        var chunksToDelete = new List<Pos>();
        foreach (var chunk in chunks.GetChunks())
        {
            int xd = posX - chunk.pos.x;
            int yd = posZ - chunk.pos.z;

            if ((xd * xd + yd * yd) > distanceToDeleteInUnitsSquared)
            {
                chunksToDelete.Add(chunk.pos);
            }
        }

        for (int i = 0; i < chunksToDelete.Count; i++)// (var chunk in chunksToDelete)
        {
            chunks.Destroy(chunksToDelete[i]);
        }
    }

    bool FindChunksAndLoad()
    {
        //Cycle through the array of positions
        for (int i = 0; i < chunkPositions.Length; i++)
        {
            //Get the position of this gameobject to generate around
            Pos playerPos = chunks.GetChunkPos(transform.position);

            //translate the player position and array position into chunk position
            Pos newChunkPos = new Pos(
                chunkPositions[i].x * chunkSize + playerPos.x,
                0,
                chunkPositions[i].z * chunkSize + playerPos.z
                );

            if (chunksToGenerate.Contains(newChunkPos) || chunksToRender.Contains(newChunkPos))
                continue;

            //Get the chunk in the defined position
            Chunk newChunk = chunks.GetChunk(newChunkPos);

            //If the chunk already exists and it's already
            //rendered or in queue to be rendered continue
            if (newChunk != null)
            {
                if (newChunk.rendered) continue;

                for (int y = -32; y <= 32; y += chunkSize)
                    chunksToRender.Add(new Pos(newChunkPos.x, y, newChunkPos.z));

                return true;
            }

            for (int y = -32; y <= 32; y += chunkSize)
                chunksToGenerate.Add(new Pos(newChunkPos.x, y, newChunkPos.z));

            return true;
        }

        return false;
    }
}
