using UnityEngine;
using System.Collections;

public class ExampleBlockTypeStore: BlockAttributeStore
{
    public override BlockAttributes[] GetBlockAttributes(Voxelmetric vm)
    {
        return new BlockAttributes[] {
            new BlockAttributes() {
                blockName = "rock",
                id = 2,
                isSolid = true,
                isStatic = true,
                textureSet = vm.components.textureLoader.GetByName("rock"),
                blockType = typeof(CubeBlockType)
            },
            new BlockAttributes() {
                blockName = "dirt",
                id = 3,
                isSolid = true,
                isStatic = true,
                textureSet = vm.components.textureLoader.GetByName("dirt"),
                blockType = typeof(CubeBlockType)
            },
            new BlockAttributes() {
                blockName = "grass",
                id = 4,
                isSolid = true,
                isStatic = true,
                textureSet = vm.components.textureLoader.GetByName("grass"),
                blockType = typeof(CubeBlockType)
            },
            new BlockAttributes() {
                blockName = "sand",
                id = 5,
                isSolid = true,
                isStatic = true,
                textureSet = vm.components.textureLoader.GetByName("sand"),
                blockType = typeof(CubeBlockType)
            }
        };
    }
}
