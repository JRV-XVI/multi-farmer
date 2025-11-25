using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Map
{
    public int sizeX { get; private set; }
    public int sizeY { get; private set; }

    public float originX { get; private set; }
    public float originY { get; private set; }

    public Cell[][] matrix { get; private set; }

    public Map(int sizeX, int sizeY, Vector2 worldCenter)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        originX = worldCenter.x - sizeX / 2f;
        originY = worldCenter.y - sizeY / 2f;

        matrix = new Cell[sizeX][];

        for (int x = 0; x < sizeX; x++)
        {
            matrix[x] = new Cell[sizeY];
            for (int y = 0; y < sizeY; y++)
            {
                float wx = originX + x + 0.5f;
                float wy = originY + y + 0.5f;

                matrix[x][y] = new Cell(x, y, wx, wy, CellState.GROUND);
            }
        }
    }



    public bool WorldToIndex(Vector2 worldPos, out int ix, out int iy)
    {
        ix = Mathf.FloorToInt(worldPos.x - originX);
        iy = Mathf.FloorToInt(worldPos.y - originY);

        return ix >= 0 && ix < sizeX && iy >= 0 && iy < sizeY;
    }


    public void ChangeTilesInMap(Vector2 start, Vector2 end, CellState newState)
    {
        if (!WorldToIndex(start, out int ix0, out int iy0)) return;
        if (!WorldToIndex(end, out int ix1, out int iy1)) return;

        int minX = Mathf.Clamp(Mathf.Min(ix0, ix1), 0, sizeX - 1);
        int maxX = Mathf.Clamp(Mathf.Max(ix0, ix1), 0, sizeX - 1);

        int minY = Mathf.Clamp(Mathf.Min(iy0, iy1), 0, sizeY - 1);
        int maxY = Mathf.Clamp(Mathf.Max(iy0, iy1), 0, sizeY - 1);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                matrix[x][y].state = newState;
    }


    // ===============================
    //           A* ALGORITHM
    // ===============================
   

    public List<Cell> FindPath(Vector2 startWorld, Vector2 endWorld)
    {
        if (!WorldToIndex(startWorld, out int sx, out int sy)) return null;
        if (!WorldToIndex(endWorld, out int ex, out int ey)) return null;

        Cell start = matrix[sx][sy];
        Cell end = matrix[ex][ey];

        MinHeap openSet = new MinHeap();
        HashSet<Cell> closedSet = new HashSet<Cell>();

        Debug.Log("FindPath() fue llamado correctamente");


        // Reset A* data
        foreach (var row in matrix)
            foreach (var c in row)
            {
                c.gCost = 999999;
                c.hCost = 0;
                c.parent = null;
            }

        start.gCost = 0;
        start.hCost = Vector2.Distance(startWorld, endWorld);

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Cell current = openSet.PopMin();

            if (current == end)
                return RetracePath(start, end);

            closedSet.Add(current);

            foreach (Cell neighbor in GetNeighbors(current))
            {
                bool isWalkable =
                neighbor.state == CellState.GROUND ||
                neighbor.state == CellState.SAFE_ZONE ||
                neighbor.state == CellState.PLAYER;

                // 👉 excepción: el destino es una planta
                if (!isWalkable && neighbor != end)
                    continue;


                if (closedSet.Contains(neighbor))
                    continue;

                float newCost = current.gCost + 1;

                if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = Mathf.Abs(neighbor.x - end.x) + Mathf.Abs(neighbor.y - end.y);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("FindPath() terminó sin camino → devolviendo null");

        return null;
    }


    List<Cell> RetracePath(Cell start, Cell end)
    {
        List<Cell> path = new List<Cell>();
        Cell current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    List<Cell> GetNeighbors(Cell c)
    {
        List<Cell> list = new List<Cell>();

        int[,] offsets = new int[,]
        {
            { 1, 0 },
            { -1, 0 },
            { 0, 1 },
            { 0, -1 }
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = c.x + offsets[i, 0];
            int ny = c.y + offsets[i, 1];

            if (nx >= 0 && nx < sizeX && ny >= 0 && ny < sizeY)
                list.Add(matrix[nx][ny]);
        }

        return list;
    }




    //Marcador general
    public void MarkAll()
    {
        MarkObstaclesByTag(this);
        MarkPlantByTag(this);
        MarkSafeZoneByTag(this);
    }



    //Marcadores 
    public void MarkObstaclesByTag(Map map)
    {
        GameObject[] obstaculos = GameObject.FindGameObjectsWithTag("Obstacle");

        Vector3 obsPos; //posicion de obstaculo
        Vector3 obsSca; //Escala de obstaculo

        foreach (GameObject obstacle in obstaculos)
        {
            obsPos = obstacle.transform.position;
            obsSca = obstacle.transform.localScale;

            map.ChangeTilesInMap(new Vector2(obsPos.x - (obsSca.x / 2), obsPos.z - (obsSca.z / 2)),
                                 new Vector2(obsPos.x + (obsSca.x / 2), obsPos.z + (obsSca.z / 2)),
                                 CellState.OBSTACLE);
        }
    }

    public void MarkTemporalObstacle(Map map, GameObject obstacle)
    {
        Vector3 obsPos = obstacle.transform.position;
        Vector3 obsSca = obstacle.transform.localScale;

        map.ChangeTilesInMap(new Vector2(obsPos.x - (obsSca.x / 2), obsPos.z - (obsSca.z / 2)),
                                 new Vector2(obsPos.x + (obsSca.x / 2), obsPos.z + (obsSca.z / 2)),
                                 CellState.OBSTACLE);

        WaitSeconds(5f);
        map.ChangeTilesInMap(new Vector2(obsPos.x - (obsSca.x / 2), obsPos.z - (obsSca.z / 2)),
                                 new Vector2(obsPos.x + (obsSca.x / 2), obsPos.z + (obsSca.z / 2)),
                                 CellState.GROUND);

    }




    public void MarkPlantByTag(Map map)
    {
        GameObject[] obstaculos = GameObject.FindGameObjectsWithTag("Plant");

        Vector3 obsPos; //posicion del tomate
        Vector3 obsSca; //Escala del tomate

        foreach (GameObject obstacle in obstaculos)
        {
            obsPos = obstacle.transform.position;
            obsSca = obstacle.transform.localScale;

            map.ChangeTilesInMap(new Vector2(obsPos.x - (obsSca.x / 2), obsPos.z - (obsSca.z / 2)),
                                 new Vector2(obsPos.x + (obsSca.x / 2), obsPos.z + (obsSca.z / 2)),
                                 CellState.HEALTHY_PLANT);
        }
    }




    /*
    public void MarkBotByTag(Map map)
    {
        GameObject bot = GameObject.FindGameObjectWithTag("Bot");

        Vector3 botPos = bot.transform.position;
        Vector3 botSca = bot.transform.localScale;

        map.ChangeTilesInMap(new Vector2(_botLastPosition.x - (botSca.x / 2), _botLastPosition.z - (botSca.z / 2)),
                             new Vector2(_botLastPosition.x + (botSca.x / 2), _botLastPosition.z + (botSca.z / 2)),
                             CellState.GROUND);



        map.ChangeTilesInMap(new Vector2(botPos.x - (botSca.x / 2), botPos.z - (botSca.z / 2)),
                             new Vector2(botPos.x + (botSca.x / 2), botPos.z + (botSca.z / 2)),
                             CellState.PLAYER);

        _botLastPosition = botPos;

    }
    */


    public void MarkSafeZoneByTag(Map map)
    {
        GameObject safeZone = GameObject.FindGameObjectWithTag("Zone");

        Vector3 obPos = safeZone.transform.position;
        Vector3 obSca = safeZone.transform.localScale;

        map.ChangeTilesInMap(new Vector2(obPos.x - (obSca.x / 2), obPos.z - (obSca.z / 2)),
                             new Vector2(obPos.x + (obSca.x / 2), obPos.z + (obSca.z / 2)),
                             CellState.SAFE_ZONE);
    }



    //Funcion para esperar tiempo
    public IEnumerator WaitSeconds(float sec)
    {
        Debug.Log("Bot empieza a esperar...");

        // Espera 5 segundos
        yield return new WaitForSeconds(sec);

        Debug.Log("Bot terminó de esperar, ahora actúa");
        // Aquí pones la acción que quieres que haga después de esperar
    }
}





