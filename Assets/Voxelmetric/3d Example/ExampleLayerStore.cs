using UnityEngine;
using System.Collections;
using System;

public class ExampleLayerStore : LayerStore
{

    public override TerrainLayer[] GetLayers(Voxelmetric vm)
    {
        return new TerrainLayer[] {
            new SimpleLayer() {
                baseHeight = 20,
                frequency = 0.01f,
                amplitude = 16,
                absolute = true,
                blockId = vm.components.blockLoader.GetId("rock")
            },
            new SimpleLayer() {
                baseHeight = 0,
                frequency = 0.1f,
                amplitude = 6,
                absolute = false,
                blockId = vm.components.blockLoader.GetId("rock")
            },
            new SimpleLayer() {
                baseHeight = 20,
                frequency = 0.05f,
                amplitude = 2,
                absolute = true,
                blockId = vm.components.blockLoader.GetId("dirt")
            },
            new SimpleLayer() {
                baseHeight = 2,
                frequency = 0.01f,
                amplitude = 2,
                absolute = false,
                blockId = vm.components.blockLoader.GetId("grass")
            },
        };
    }
}