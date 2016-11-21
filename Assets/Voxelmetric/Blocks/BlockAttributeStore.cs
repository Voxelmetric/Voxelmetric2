using UnityEngine;
using System.Collections;

public abstract class BlockAttributeStore: MonoBehaviour
{
    public abstract BlockAttributes[] GetBlockAttributes(Voxelmetric vm);
}
