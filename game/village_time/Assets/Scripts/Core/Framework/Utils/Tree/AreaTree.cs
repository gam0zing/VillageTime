using UnityEngine;

/// <summary>
/// 区块四叉树，维护地图区块的容器
/// </summary>
public class AreaTree<T> where T : IRectangleArea {

    protected AreaNode _root;

    protected ushort _areaSize;

    public AreaTree(ushort areaSize) {
        this._root = new AreaNode(this, null, 0, 0, 128, 128);

        this._areaSize = areaSize;
    }

    protected class AreaNode {
        protected AreaTree<T> _tree;
        protected AreaNode[] _children;

        protected T[] _values;

        protected WorldTransform _transform;

        public AreaNode(AreaTree<T> tree, AreaNode parent, int posX, int posY, int sizeX, int sizeY) : this(tree, parent, new Vector2Int(), new Vector2Int(         )) {

        }

        public AreaNode(AreaTree<T> tree, AreaNode parent, Vector2Int pos, Vector2Int size) {
            this._tree = tree;
            this._children = new AreaNode[4];
            this._values = new T[8];
            this._transform = new WorldTransform(pos, size);
        }

        public void Push(T value) {
            // 3种情况：
            // 1、自己是不是叶子，交给下一级处理
            // 2、自己是叶子但满员，交给下一级处理，把自己现有的值下放
            // 3、自己是叶子且未满员，添加
            if (this._children.Length > 0) {
                
            }
        }
    }

    public void Push(T value) {
        // 2种情况：
        // 1、根节点范围包含该值，调根节点Push
        // 2、根节点范围不包含该值，调自身ScaleOut，并再次调用自己
        this._root.Push(value);
    }
}