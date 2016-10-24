using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VoxelmetricRouter: NetworkBehaviour {

    private Voxelmetric vm;

    public void Start()
    {
        //TODO: Find a way to pick vm when there is more than one
        SetVm(FindObjectOfType<Voxelmetric>());
    }

    public void SetVm(Voxelmetric vm)
    {
        this.vm = vm;
    }

    /// Set block
    #region Set Block
    public void SetBlock(Pos blockPos, string block)
    {
        SetBlock(blockPos, vm.components.blockLoader.GetId(block));
    }

    public void SetBlock(Pos blockPos, int blockId)
    {
        CmdSetBlock(blockPos.x, blockPos.y, blockPos.z, blockId);
    }

    [ClientRpc]
    public void RpcSetBlock(int x, int y, int z, int blockId)
    {
        vm.components.chunks.SetBlock(new Pos(x, y, z), new Block(blockId));
    }

    [Command]
    public void CmdSetBlock(int x, int y, int z, int blockId)
    {
        // TODO: Add checks and validation here
        vm.components.chunks.SetBlock(new Pos(x, y, z), new Block(blockId));
        //RpcSetBlock(x, y, z, blockId);
    }
    /// Block commands
    #endregion
    /// Set block

    public Block GetBlock(Pos blockPos)
    {
        return vm.components.chunks.GetBlock(blockPos);
    }

    public Pos GetBlockPos(Vector3 position)
    {
        return vm.components.chunks.GetBlockPos(position);
    }

    public Pos GetWorldPos(Pos pos)
    {
        return vm.components.chunks.GetWorldPos(pos);
    }

}
