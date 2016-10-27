using UnityEngine;
using System;
using System.Threading;

public class CubeChunk : Chunk {

    public Block[,,] blocks;

    protected MeshFilter filter;
    protected MeshCollider col;

    protected bool rendering;
    protected bool meshDataReady;

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
        if (filter.mesh != null) filter.mesh.Clear();
        if(col.sharedMesh!=null) col.sharedMesh.Clear();
        blocks = new Block[0,0,0];
        filter.mesh = null;
        rendered = false;
    }

    public override void Render()
    {
        if (rendering) return;
        rendering = true;

        if (!rendered)
        {
            CreateChunkNeighbors();
        }

        // Set the rendered flag to true even though the mesh isn't rendered yet because all
        // the work for it is done and does not need to be called again.
        rendered = true;

        //ThreadPool.QueueUserWorkItem(new WaitCallback(CreateChunkMeshDelegate), meshData);
        CreateChunkMesh(meshData);
    }

    public override void LateUpdate()
    {
        if (meshDataReady)
        {
            Utils.ProfileCall(() =>
            {
                AssignMesh(meshData);
            }, "Assign mesh");

            Utils.ProfileCall(() =>
            {
                meshData.Clear();
            }, "Clear mesh");

            rendering = false;
            meshDataReady = false;
        }
    }

    protected void CreateChunkNeighbors()
    {
        Utils.ProfileCall(() =>
        {
            // First time this chunk is rendered - make sure that all neighboring
            // chunks are filled before this chunk can be rendered
            foreach (var dir in DirectionUtils.Directions)
            {
                chunkController.CreateChunk(pos + (chunkController.chunkSize * (Pos)dir));
            }
        }, "Create neighbors");
    }

    void CreateChunkMeshDelegate(System.Object meshData)
    {
        CreateChunkMesh((MeshData)meshData);
    }

    /// <summary>
    /// Creates a chunk mesh from the blocks the mesh contains. The mesh should use the
    /// chunk's position as it's origin point.
    /// </summary>
    protected override void CreateChunkMesh(MeshData meshData)
    {
        lock (meshData)
        {
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        blocks[x, y, z].GetBlockType(chunkController.vm).Render(this, new Pos(x, y, z) + pos, blocks[x, y, z], meshData);
                    }
                }
            }

            meshDataReady = true;
        }

    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    protected override void AssignMesh(MeshData meshData)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Mesh for " + name;
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();
        mesh.uv = meshData.uvs.ToArray();

        filter.mesh = mesh;

        mesh = new Mesh();
        mesh.name = "Collision mesh for " + name;
        //mesh.vertices = meshData.colVerts.ToArray();
        //mesh.triangles = meshData.colTris.ToArray();
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();

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
