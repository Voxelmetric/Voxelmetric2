using UnityEngine;
using System.Collections;
using SimplexNoise;

public class TerrainLayer {

    protected ChunkFiller filler;
    protected Noise noise;

    public virtual void VmStart(ChunkFiller chunkFiller)
    {
        filler = chunkFiller;
        noise = chunkFiller.noise;
    }

    public virtual int ApplyLayerCol(int x, int z, int head)
    {
        return head;
    }

    public int Noise(int x, int z, int offset, float frequency, float amplitude)
    {
        float n = noise.Generate(x * frequency, offset, z * frequency);
        n *= amplitude;
        n += offset;
        return Mathf.RoundToInt(n);
    }
}
