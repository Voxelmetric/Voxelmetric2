using UnityEngine;
using System.Collections;

public abstract class BlockTypeStore : MonoBehaviour
{
    public abstract BlockType[] GetBlockTypes(Voxelmetric vm);
}
