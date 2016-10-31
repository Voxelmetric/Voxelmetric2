using UnityEngine;
using System.Collections.Generic;

public abstract class Chunk : MonoBehaviour {

    public Pos pos { get; protected set; }
    public ChunkController chunkController { get; protected set; }
    public Voxelmetric vm { get; protected set; }

    public int   chunkSize { get { return chunkController.chunkSize; } }
    public float blockSize { get { return chunkController.blockSize; } }

    protected MeshData _meshData = new MeshData();

    /// <summary>
    /// True if the chunk has been rendered for the first time
    /// </summary>
    public bool rendered { get; protected set; }
    /// <summary>
    /// True if the contents of the chunk have been modified since last render
    /// </summary>
    public bool renderStale { get; protected set; }
    /// <summary>
    /// Used to determine if the chunk has been populated by the chunk filler
    /// </summary>
    public bool chunkIsFilled;

    /// <summary>
    /// Should initialize all the chunk's variables and fill it if necessary
    /// Keep in mind this could be a new game object or an old one returned
    /// from an object pool.
    /// </summary>
    public virtual void VmStart(Pos position, ChunkController chunkController)
    {
        pos = position;
        transform.position = position;

        this.chunkController = chunkController;
        vm = chunkController.vm;

        vm.components.chunkFiller.FillChunk(this);
        gameObject.SetActive(true);
    }

    public virtual void LateUpdate()
    {
        if (renderStale)
        {
            Render();
        }
    }

    /// <summary>
    /// Clears all data out of the chunk so that it can be returned to the chunk pool
    /// </summary>
    public virtual void Clear()
    {
        rendered = false;
        renderStale = false;
        chunkIsFilled = false;
        _meshData.Clear();
        ClearUnsavedBlocks();
    }

    /// <summary>
    /// Performs all necessary steps to generate and render the chunk's most up to date mesh
    /// </summary>
    public virtual void Render()
    {
        renderStale = false;
        CreateChunkMesh(_meshData);
        AssignMesh(_meshData);

        _meshData.Clear();
        rendered = true;
    }

    /// <summary>
    /// Flags the chunk for a render soon
    /// </summary>
    public virtual void RenderSoon()
    {
        renderStale = true;
    }

    /// <summary>
    /// Creates a chunk mesh from the blocks the mesh contains. The mesh should use the
    /// chunk's position as it's origin point.
    /// </summary>
    protected abstract void CreateChunkMesh(MeshData meshData);

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    protected abstract void AssignMesh(MeshData meshData);

    /// <summary>
    /// Returns the block at the specified position
    /// </summary>
    public abstract Block GetBlock(Pos pos);

    /// <summary>
    /// Replaces the block at the given location with the newBlock, should call OnCreate for the block created and
    /// OnDestroy for the block destroyed and if updateRender is true, should initialize a re-render of the chunk
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="pos">position to place the new block</param>
    /// <returns>Returns the block that was replaced</returns>
    public abstract Block SetBlock(Block newBlock, Pos pos, bool updateRender = true);

    public abstract List<byte> SerializeChunk(byte storeMode);
    public abstract void DeserializeChunk(List<byte> data);

    public abstract void ClearUnsavedBlocks();
    public abstract void AddUnsavedBlock(Pos pos);
    public abstract bool HasUnsavedBlocks();
}
