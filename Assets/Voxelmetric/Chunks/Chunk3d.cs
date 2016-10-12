using UnityEngine;
using System.Collections;

public class Chunk3d : BaseChunk {

    public Block[,,] blocks;

    protected MeshFilter filter;
    protected MeshCollider col;

    public override void VmStart(Pos position, ChunkController chunkController)
    {
        Utils.ProfileCall(() =>
        {
            pos = position;
            transform.position = pos;
        }, "Set position");

        this.chunkController = chunkController;
        Utils.ProfileCall(() =>
        {
            blocks = new Block[chunkSize, chunkSize, chunkSize];
        }, "Create blocks array");

        Utils.ProfileCall(() =>
        {
            chunkController.vm.components.chunkFiller.FillChunk(this);
        }, "Fill Chunk");

        Utils.ProfileCall(() =>
        {
            if (filter == null)
            {
                col = gameObject.AddComponent<MeshCollider>();
                filter = gameObject.AddComponent<MeshFilter>();
                var renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.material = chunkController.vm.components.textureLoader.material;
            }
        }, "Adding components");

        Utils.ProfileCall(() =>
        {
            transform.parent = chunkController.transform;
        }, "Setting parent");

        Utils.ProfileCall(() =>
        {
            gameObject.SetActive(true);
        }, "Activating game object");
    }

    /// <summary>
    /// Clears all data out of the chunk so that it can be returned to the chunk pool
    /// </summary>
    public override void Clear()
    {
        meshData.Clear();
        blocks = new Block[0,0,0];
        filter.mesh = null;
        rendered = false;
    }

    public override void Render()
    {
        CreateChunkMesh(meshData);
        AssignMesh(meshData);

        meshData.Clear();
    }

    /// <summary>
    /// Creates a chunk mesh from the blocks the mesh contains. The mesh should use the
    /// chunk's position as it's origin point.
    /// </summary>
    protected override void CreateChunkMesh(MeshData meshData)
    {
        Profiler.BeginSample("Create neighbors");
        if (!rendered)
        {
            // Fist time this chunk is rendered - make sure that all neighboring
            // chunks are filled before this chunk can be rendered
            foreach (var dir in DirectionUtils.Directions)
            {
                chunkController.CreateChunk(pos + (chunkController.chunkSize * (Pos)dir));
            }

            rendered = true;
        }
        Profiler.EndSample();

        Profiler.BeginSample("Fill meshData");
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    blocks[x, y, z].GetBlockType(chunkController.vm).Render(this, new Pos(x, y, z) + pos, blocks[x, y, z], ref meshData);
                }
            }
        }
        Profiler.EndSample();
    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    protected override void AssignMesh(MeshData meshData)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();
        mesh.uv = meshData.uvs.ToArray();

        filter.mesh = mesh;

        mesh = new Mesh();
        mesh.vertices = meshData.colVerts.ToArray();
        mesh.triangles = meshData.colTris.ToArray();

        col.sharedMesh = mesh;

        meshData.Clear();
    }

    /// <summary>
    /// Returns the block at the specified position
    /// </summary>
    public override Block GetBlock(Pos blockPos)
    {
        if (blockPos.x > pos.x + chunkSize - 1 || blockPos.y > pos.y + chunkSize - 1 ||
            blockPos.z > pos.z + chunkSize - 1 || blockPos.x < pos.x ||
            blockPos.y < pos.y || blockPos.z < pos.z)
        {
            return chunkController.GetBlock(blockPos);
        }

        return blocks[
            blockPos.x - pos.x,
            blockPos.y - pos.y,
            blockPos.z - pos.z
        ];
    }

    /// <summary>
    /// Replaces the block at the given location with the newBlock
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="pos">position to place the new block</param>
    /// <returns>Returns the block that was replaced</returns>
    public override Block SetBlock(Block newBlock, Pos blockPos)
    {
        Block oldBlock = GetBlock(pos);
        oldBlock.GetBlockType(chunkController.vm).OnDestroy(this, pos, oldBlock, 0);

        blocks[
            blockPos.x - pos.x,
            blockPos.y - pos.y,
            blockPos.z - pos.z
        ] = newBlock.GetBlockType(chunkController.vm).OnCreate(this, pos, newBlock); ;

        return oldBlock;
    }
}
