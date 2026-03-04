public class BlockChunk {
    protected BlockInstance[,,] _blocks;

    /// <summary>
    /// 区块默认构造函数，和mc一样区块大小是16x16
    /// </summary>
    public BlockChunk() : this(16, 16, 1) {
    }

    /// <summary>
    /// 生成自定义大小的区块。
    /// 警告：区块大小必须和地图规定保持一致，除非你明确知道自己在做什么，否则不建议调用这个构造器
    /// </summary>
    public BlockChunk(uint sizeX, uint sizeY, uint sizeZ) {
        this._blocks = new BlockInstance[sizeX, sizeY, sizeZ];
    }

    /// <summary>
    /// 注意：该区块将直接使用这个数组的引用
    /// </summary>
    public BlockChunk(BlockInstance[,,] blocks) {
        this._blocks = blocks;
    }
}