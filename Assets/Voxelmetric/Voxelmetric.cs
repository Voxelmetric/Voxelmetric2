using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System.Threading;

/// <summary>
/// Voxelmetric is the main manager and initializer of all other vm components.
/// It is also where you can define which components they are using. This class
/// should contain very little game logic, instead it handles starting and linking
/// of other components.
/// </summary>
public class Voxelmetric : MonoBehaviour {

    /// <summary>
    /// Making this a struct and then including it as a property makes it collapsible
    /// in the Unity inspector. The components can be accessed with the read get only
    /// handles below.
    /// </summary>
    public struct Components
    {
        public ChunkController chunks;
        public BlockTypeLoader blockLoader;
        public ChunkFiller chunkFiller;
        public TextureSetLoader textureLoader;
        public Chunk chunkType;
    }

    private Components _components;
    private bool componentsLoaded = false;
    internal Components components
    {
        get
        {
            if (!componentsLoaded) VmStart();
            return _components;
        }
    }

    [System.Serializable]
    public struct Settings
    {
        public bool networking;
        public bool threading;
    }

    [SerializeField]
    public Settings settings = new Settings() { networking = true };


    /// <summary>
    ///  From here this class can initialize all other components.
    /// </summary>
	void VmStart()
    {
        componentsLoaded = true;

        _components.chunks = GetComponent<ChunkController>();
        _components.textureLoader = GetComponent<TextureSetLoader>();
        _components.blockLoader = GetComponent<BlockTypeLoader>();
        _components.chunkFiller = GetComponent<ChunkFiller>();
        _components.chunkType = GetComponent<Chunk>();

        _components.chunks.Initialize(this);
        _components.textureLoader.LoadTextures(this);
        _components.blockLoader.LoadBlocks(this);
        _components.chunkFiller.Initialize(this);
    }

    void Start()
    {
        if(!componentsLoaded) VmStart();
    }
}
