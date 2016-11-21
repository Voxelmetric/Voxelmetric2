using UnityEngine;
using System.Collections;

public class CubeBlockType : Block {

    protected override void AddMeshData(Chunk chunk, Pos pos, ref MeshData meshData)
    {
        foreach (var dir in DirectionUtils.Directions)
        {
            if (!chunk.GetBlock(pos + dir).IsSolid(DirectionUtils.Opposite(dir)))
            {
                meshData.verts.AddRange(MeshArrays.VertexCubeFaces(pos - chunk.pos, chunk.blockSize, dir));
                meshData.tris.AddRange(MeshArrays.TriCubeFaces(meshData.verts.Count));
                meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(attributes.textureSet.GetTexture(chunk, pos, dir)));
            }
        }
    }
}
