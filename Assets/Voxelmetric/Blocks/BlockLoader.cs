using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

/// <summary>
/// In initialization this class fetches all the block definitions from some
/// source and creates block types for all of them and makes them available
/// to create in game. MUST IMPLEMENT AIR (TYPE 0), VOID (TYPE 1).
/// </summary>
public class BlockLoader : MonoBehaviour {

    Voxelmetric vm;

    protected Dictionary<int, BlockAttributes> idAttributes;
    protected Dictionary<string, int> nameId;
    protected Dictionary<int, string> idName;
    protected Dictionary<int, Block> staticBlocks;

    public virtual void LoadBlocks(Voxelmetric vm)
    {
        this.vm = vm;

        idAttributes = new Dictionary<int, BlockAttributes>();
        nameId = new Dictionary<string, int>();
        idName = new Dictionary<int, string>();
        staticBlocks = new Dictionary<int, Block>();

        AddAttributes(AirAttributes());
        AddAttributes(VoidAttributes());

        var attributesToLoad = FindObjectOfType<BlockAttributeStore>().GetBlockAttributes(vm);

        foreach (var blockAttributes in attributesToLoad)
        {
            AddAttributes(blockAttributes);
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

    public virtual BlockAttributes GetAttributes(int id)
    {
        return idAttributes[id];
    }

    public virtual void AddAttributes(BlockAttributes attr)
    {
        attr.Initialize(vm);

        idAttributes.Add(attr.id, attr);
        nameId.Add(attr.blockName, attr.id);
        idName.Add(attr.id, attr.blockName);
    }

    public Block CreateBlock(int id)
    {
        var blockAttributes = vm.components.blockLoader.GetAttributes(id);
        Block block;

        if (blockAttributes.isStatic)
        {
            if (staticBlocks.TryGetValue(id, out block))
            {
                return block;
            }
        }

        block = (Block)Activator.CreateInstance(blockAttributes.GetBlockType());
        block.attributes = blockAttributes;

        if (blockAttributes.isStatic)
        {
            staticBlocks.Add(id, block);
        }

        return block;
    }

    public Block CreateBlock(string name)
    {
        return CreateBlock(GetId(name));
    }

    public BlockAttributes AirAttributes()
    {
        return new BlockAttributes()
        {
            vm = vm,
            blockName = "air",
            id = Block.AirId,
            blockType = typeof(Block),
            isSolid = false,
            isStatic = true
        };
    }

    public BlockAttributes VoidAttributes()
    {
        return new BlockAttributes()
        {
            vm = vm,
            blockName = "void",
            id = Block.VoidId,
            blockType = typeof(Block),
            isSolid = true,
            isStatic = true
        };
    }
}
