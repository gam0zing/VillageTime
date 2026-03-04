using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 专为无限地图设计的四叉树，高效率拓展覆盖范围
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class QuadTree<TValue> where TValue : class, ITreePoint {
    protected class AreaNode {
        public readonly RectInt rect;
        public readonly int left, right, bottom, top;

        public bool IsLeaf = true;

        public readonly QuadTree<TValue> tree;
        public AreaNode parent;

        private readonly Action<AreaNode> _setRootFunc;

        public readonly AreaNode[,] children = new AreaNode[2, 2];

        public readonly int forkNum;
        public readonly int mergeNum;

        public readonly List<TValue> values = new List<TValue>();

        public AreaNode(QuadTree<TValue> tree, AreaNode parent, Action<AreaNode> setRootFunc, int forkNum, int posX, int posY, int sizeX, int sizeY) {
            this.tree = tree;
            this.parent = parent;
            this._setRootFunc = setRootFunc;
            this.forkNum = forkNum;
            this.mergeNum = forkNum >> 1;

            this.rect = new RectInt(posX, posY, sizeX, sizeY);
            this.left = posX;
            this.right = posX + sizeX;
            this.bottom = posY;
            this.top = posY + sizeY;
        }

        public TValue Get(Vector2Int pos) {
            if (!this.CheckRange(pos)) return default;

            if (this.IsLeaf) {
                foreach (var v in this.values) {
                    if (v.Position == pos)
                        return v;
                }
                return default;
            } else {
                var partIdx = this.CheckPart(pos);
                var child = this.children[partIdx.x, partIdx.y];

                if (child != null) {
                    return child.Get(pos);
                } else {
                    return default;
                }
            }
        }

        public bool Remove(TValue value) {
            if (!this.CheckRange(value.Position)) return false;

            if (this.IsLeaf) {
                for (int i = 0; i < this.values.Count; i++) {
                    if (ReferenceEquals(this.values[i], value)) {
                        this.values.RemoveAt(i);
                        this.parent?.CheckMerge();
                        return true;
                    }
                }
                return false;
            } else {
                var partIdx = this.CheckPart(value.Position);
                var child = this.children[partIdx.x, partIdx.y];
                return child != null && child.Remove(value);
            }
        }

        public void Push(TValue value) {
            if (this.IsLeaf) {
                if (this.values.Count < this.forkNum) {
                    this.values.Add(value);
                    return;
                } else {
                    this.Fork();
                }
            }

            this.SubPush(value);
        }

        public void RootPush(TValue value) {
            if (this.CheckRange(value.Position)) {
                this.Push(value);
            } else {
                var dir = this.CheckDirection(value.Position);
                var newRoot = this.ExpandRange(dir);
                newRoot.RootPush(value);
            }
        }

        private void SubPush(TValue value) {
            var partIdx = this.CheckPart(value.Position);
            var child = this.children[partIdx.x, partIdx.y];

            if (child == null) {
                int width = this.rect.width;
                int height = this.rect.height;
                int posX = partIdx.x == 0 ? this.left : (this.left + this.right) >> 1;
                int posY = partIdx.y == 0 ? this.bottom : (this.bottom + this.top) >> 1;

                child = new AreaNode(this.tree, this, this._setRootFunc, this.forkNum, posX, posY, width >> 1, height >> 1);
                this.children[partIdx.x, partIdx.y] = child;
            }

            child.Push(value);
        }

        private void Fork() {
            if (this.right - this.left <= 2 && this.top - this.bottom <= 2) {
                Debug.LogWarning("警告：异常的节点内地图块数量，请检查地图块是否发生重合");
                return;
            }

            foreach (var value in this.values) {
                this.SubPush(value);
            }
            this.values.Clear();
            this.IsLeaf = false;
        }

        public void CheckMerge() {
            var allLeaf = true;
            int totalCount = 0;

            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 2; y++) {
                    var child = this.children[x, y];
                    if (child != null) {
                        if (!child.IsLeaf)
                            allLeaf = false;
                        totalCount += child.values.Count;
                    }
                }
            }

            if (allLeaf && totalCount <= this.mergeNum) {
                var mergedValues = new List<TValue>();
                for (int x = 0; x < 2; x++) {
                    for (int y = 0; y < 2; y++) {
                        var child = this.children[x, y];
                        if (child != null) {
                            mergedValues.AddRange(child.values);
                            child.parent = null;
                        }
                    }
                }

                this.Merge(mergedValues);
            }
        }

        private void Merge(List<TValue> values) {
            this.values.Clear();
            this.values.AddRange(values);

            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                    this.children[x, y] = null;

            this.IsLeaf = true;
        }

        private AreaNode ExpandRange(Vector2Int dir) {
            int newPosX = this.rect.xMin + this.rect.width * dir.x;
            int newPosY = this.rect.yMin + this.rect.height * dir.y;
            int newSizeX = this.rect.width * 2;
            int newSizeY = this.rect.height * 2;

            var newRoot = new AreaNode(this.tree, null, this._setRootFunc, this.forkNum, newPosX, newPosY, newSizeX, newSizeY);

            this._setRootFunc(newRoot);

            int childX = dir.x + 1;
            int childY = dir.y + 1;
            newRoot.children[childX, childY] = this;
            this.parent = newRoot;

            return newRoot;
        }

        private bool CheckRange(Vector2Int target) {
            return target.x >= this.left && target.x < this.right &&
                   target.y >= this.bottom && target.y < this.top;
        }

        private Vector2Int CheckPart(Vector2Int target) {
            int midX = (this.left + this.right) >> 1;
            int midY = (this.bottom + this.top) >> 1;
            int x = (target.x < midX) ? 0 : 1;
            int y = (target.y < midY) ? 0 : 1;
            return new Vector2Int(x, y);
        }

        private Vector2Int CheckDirection(Vector2Int target) {
            int dx = target.x < this.left ? -1 : 0;
            int dy = target.y < this.bottom ? -1 : 0;
            return new Vector2Int(dx, dy);
        }
    }

    // ========== AreaTree 成员 ==========
    protected AreaNode _root;
    public readonly int maxNum = 8;

    public QuadTree() {
        this._root = new AreaNode(this, null, this.SetRoot, this.maxNum, 0, 0, 128, 128);
    }

    protected void SetRoot(AreaNode root) {
        this._root = root;
    }

    public void Push(TValue value) {
        this._root.RootPush(value);
    }

    public void Remove(TValue value) {
        this._root.Remove(value);
    }

    public TValue Get(Vector2Int pos) {
        return this._root.Get(pos);
    }
}