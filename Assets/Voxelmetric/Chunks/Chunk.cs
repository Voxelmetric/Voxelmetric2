using UnityEngine;
using System.Collections.Generic;

// TODO: need to implement logic around reloading mesh and initial mesh load

public class Chunk : MonoBehaviour {

    public Pos pos;
    [HideInInspector]
    public ChunkController chunkController;

    public int   chunkSize { get { return chunkController.chunkSize; } }
    public float blockSize { get { return chunkController.blockSize; } }
    public bool rendered = false;

    protected MeshData meshData = new MeshData();

    bool _chunkIsFilled = false;
    /// <summary>
    /// Used to determine if the chunk has been populated by the chunk filler
    /// Some implementations may need this value to know if all neighboring
    /// chunks are filled before rendering
    /// </summary>
    public virtual bool chunkIsFilled {
        get { return _chunkIsFilled; }
        set { _chunkIsFilled = value; }
    }

    public virtual void VmStart(Pos position, ChunkController chunkController)
    {
        pos = position;
        this.chunkController = chunkController;

        chunkController.vm.components.chunkFiller.FillChunk(this);

        gameObject.SetActive(true);
    }

    public virtual void LateUpdate()
    {

    }

    /// <summary>
    /// Clears all data out of the chunk so that it can be returned to the chunk pool
    /// </summary>
    public virtual void Clear()
    {
    }

    /// <summary>
    /// Performs all necessary steps to generate and render the chunk's most up to date mesh
    /// </summary>
    public virtual void Render()
    {
        CreateChunkMesh(meshData);
        AssignMesh(meshData);

        meshData.Clear();
        rendered = true;
    }

    /// <summary>
    /// Regenerates the chunks mesh from the blocks it contains and assigns the mesh to
    /// be rendered.
    /// </summary>
    public virtual void UpdateChunk()
    {
        Render();
    }

    /// <summary>
    /// Creates a chunk mesh from the blocks the mesh contains. The mesh should use the
    /// chunk's position as it's origin point.
    /// </summary>
    protected virtual void CreateChunkMesh(MeshData meshData)
    {
    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    protected virtual void AssignMesh(MeshData meshData)
    {
    }

    /// <summary>
    /// Returns the block at the specified position
    /// </summary>
    public virtual Block GetBlock(Pos pos)
    {
        return new Block();
    }

    /// <summary>
    /// Replaces the block at the given location with the newBlock
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="pos">position to place the new block</param>
    /// <returns>Returns the block that was replaced</returns>
    public virtual Block SetBlock(Block newBlock, Pos pos)
    {
        newBlock.GetBlockType(chunkController.vm).OnCreate(this, pos, newBlock);

        return GetBlock(pos);
    }

    public virtual List<byte> SerializeChunk(byte storeMode)
    {
        return new List<byte>();
    }

    public virtual void DeserializeChunk(List<byte> data) { }
    public virtual void ClearStaleBlocks() { }
    public virtual void AddStaleBlock(Pos pos) { }

    public virtual bool HasStaleBlocks()
    {
        return false;
    }
}
