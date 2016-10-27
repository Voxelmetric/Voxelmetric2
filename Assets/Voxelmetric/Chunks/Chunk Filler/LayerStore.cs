using UnityEngine;
using System.Collections;

public abstract class LayerStore : MonoBehaviour
{

    public abstract TerrainLayer[] GetLayers(Voxelmetric vm);

}