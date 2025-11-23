using UnityEngine;


[System.Serializable]
public class Cell
{
    public int x; // índice en matriz
    public int y;

    public float worldX; // coordenada real en el mundo
    public float worldY;

    public CellState state;

    // A* data
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    public Cell parent;

    public Cell(int x, int y, float worldX, float worldY, CellState state)
    {
        this.x = x;
        this.y = y;
        this.worldX = worldX;
        this.worldY = worldY;
        this.state = state;
    }
}
