using UnityEngine;
using System.Collections;
using SimplexNoise;

public class SimpleLayer : TerrainLayer {

    public bool absolute;
    public int baseHeight;
    public float frequency;
    public float amplitude;
    public int offset;

    public int blockId;

    public override int ApplyLayerCol(int x, int z, int head)
    {
        int add = Noise(x, z, offset, frequency, amplitude) + baseHeight;

        int top = add;
        if (!absolute) top += head;
        if (absolute && head > add) return head;

        Utils.ProfileCall(() =>
        {
            filler.SetBlocks(x, z, head, top, new Block(blockId));
        }, "Set Blocks");
        return top;
    }
}
