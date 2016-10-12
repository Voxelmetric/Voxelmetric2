using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// In initialization this class fetches all the block definitions from some
/// source and creates block types for all of them and makes them available
/// to create in game. MUST IMPLEMENT AIR (TYPE 0), VOID (TYPE 1).
/// </summary>
public class BlockTypeLoader : MonoBehaviour {

    Voxelmetric vm;

    protected Dictionary<int, BlockType> idAttributes;
    protected Dictionary<string, int> nameId;
    protected Dictionary<int, string> idName;

    public virtual void LoadBlocks(Voxelmetric vm)
    {
        this.vm = vm;

        idAttributes = new Dictionary<int, BlockType>();
        nameId = new Dictionary<string, int>();
        idName = new Dictionary<int, string>();

        AddType(AirAttributes());
        AddType(VoidAttributes());

        var typesToLoad = FindObjectOfType<BlockTypesStore>().GetBlockAttrs(vm);

        foreach (var attr in typesToLoad)
        {
            AddType(attr);
        }

        Debug.Log(string.Format("Voxelmetric Block Type Loader loaded {0} block types", idName.Keys.Count));
    }

    public virtual string GetName(int id)
    {
        return idName[id];
    }

    public virtual int GetId(string name)
    {
        return nameId[name];
    }

    public virtual BlockType BlockAttr(int id)
    {
        return idAttributes[id];
    }

    public virtual void AddType(BlockType attr)
    {
        attr.Initialize(vm);

        idAttributes.Add(attr.id, attr);
        nameId.Add(attr.blockName, attr.id);
        idName.Add(attr.id, attr.blockName);
    }

    public BlockType AirAttributes()
    {
        return new BlockType()
        {
            vm = vm,
            blockName = "air",
            id = Block.AirId,
            solid = false,
            canBeWalkedOn = false,
            canBeWalkedThrough = true,
        };
    }

    public BlockType VoidAttributes()
    {
        return new BlockType()
        {
            vm = vm,
            blockName = "void",
            id = Block.VoidId,
            solid = false,
            canBeWalkedOn = false,
            canBeWalkedThrough = true,
        };
    }
}
