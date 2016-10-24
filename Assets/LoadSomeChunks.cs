using UnityEngine;
using System.Collections.Generic;

public class LoadSomeChunks : MonoBehaviour
{
    public Voxelmetric vm;
    Pos pos = new Pos(0, 0, 0);

    void Start()
    {

    }

    void Update()
    {
        if (pos.x < 16)
        {
            pos.x++;
        }
        else if (pos.z < 16)
        {
            pos.x = 0;
            pos.z++;
        }

        var chunkAtLocation = vm.components.chunks.GetChunk(transform.position);
        if (chunkAtLocation == null || !chunkAtLocation.rendered)
        {
            int chunkSize = vm.components.chunks.chunkSize;

            List<Chunk> newChunks = new List<Chunk>();
            Utils.ProfileCall(() =>
            {
                for (int y = -2; y <= 2; y++)
                    newChunks.Add(vm.components.chunks.CreateChunk(new Pos(pos.x * chunkSize, y * chunkSize, pos.z * chunkSize)));
            }, "Create chunks");

            Utils.ProfileCall(() =>
            {
                foreach (var chunk in newChunks)
                {
                    chunk.Render();
                }
            }, "Render chunks");
        }
    }
}
