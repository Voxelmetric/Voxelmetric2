using UnityEngine;
using System.Collections;

public class CubeBlockType : BlockType {

    protected TextureSet textureSet;
    public string textureName;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
        textureSet = vm.components.textureLoader.GetByName(textureName);
    }

    protected override void AddMeshData(Chunk chunk, Pos pos, Block block, ref MeshData meshData)
    {
        foreach (var dir in DirectionUtils.Directions)
        {
            if (!chunk.GetBlock(pos + dir).GetBlockType(vm).IsSolid(chunk,pos, block, DirectionUtils.Opposite(dir)))
            {
                meshData.verts.AddRange(MeshArrays.VertexCubeFaces(pos - chunk.pos, chunk.blockSize, dir));
                meshData.tris.AddRange(MeshArrays.TriCubeFaces(meshData.verts.Count));
                meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(textureSet.GetTexture(chunk, pos, dir)));
            }
        }
    }
}
