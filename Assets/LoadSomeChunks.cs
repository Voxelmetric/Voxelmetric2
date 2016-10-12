using UnityEngine;
using System.Collections.Generic;

public class LoadSomeChunks : MonoBehaviour
{
    public Voxelmetric vm;

    void Start()
    {

    }


    void Update()
    {
        var chunkAtLocation = vm.components.chunks.GetChunk(transform.position);
        if (chunkAtLocation == null || !chunkAtLocation.rendered)
        {
            int chunkSize = vm.components.chunks.chunkSize;

            List<BaseChunk> newChunks = new List<BaseChunk>();
            Profiler.BeginSample("Create Chunks");
            for (int y = -2; y <= 2; y++)
                newChunks.Add(vm.components.chunks.CreateChunk(new Pos((int)transform.position.x, y * chunkSize, (int)transform.position.z)));
            Profiler.EndSample();

            Profiler.BeginSample("Render Chunks");
            foreach (var chunk in newChunks)
            {
                chunk.Render();
            }
            Profiler.EndSample();
        }
    }
}
