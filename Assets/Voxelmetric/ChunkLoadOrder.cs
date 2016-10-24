﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class ChunkLoadOrder
{
    public static Pos[] ChunkPositions(int chunkLoadRadius)
    {
        var chunkLoads = new List<Pos>();
        for (int x = -chunkLoadRadius; x <= chunkLoadRadius; x++)
        {
            for (int z = -chunkLoadRadius; z <= chunkLoadRadius; z++)
            {
                chunkLoads.Add(new Pos(x, 0, z));
            }
        }

        // limit how far away the blocks can be to achieve a circular loading pattern
        float maxRadius = chunkLoadRadius * 1.55f;

        //sort 2d vectors by closeness to center
        return chunkLoads
                .Where(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.z) < maxRadius)
                .OrderBy(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.z)) //smallest magnitude vectors first
                .ThenBy(pos => Mathf.Abs(pos.x)) //make sure not to process e.g (-10,0) before (5,5)
                .ThenBy(pos => Mathf.Abs(pos.z))
                .ToArray();
    }

}