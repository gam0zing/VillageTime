using UnityEngine;

public struct WorldTransform {
    /// <summary>
    /// 规范：将该位置视作方形区域的左下角，即最小坐标处
    /// </summary>
    public Vector2Int position;
    public Vector2Int size;

    public WorldTransform(Vector2Int position, Vector2Int size) {
        this.position = position;
        this.size = size;
    }

    public WorldTransform(int posX, int posY, int sizeX, int sizeY) {
        this.position = new Vector2Int(posX, posY);
        this.size = new Vector2Int(sizeX, sizeY);
    }
}