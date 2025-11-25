using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class TestMap : MonoBehaviour
{
    public RawImage miniMapImage;

    public Map map;

    public GameObject bot;
    private Vector3 _botLastPosition;

    public GameObject safeZone;

    private List<Cell> _currentPath;
    private Vector3 _lastTarget = Vector3.zero;
    private Texture2D baseTex;



    void Start()
    {
        map = new Map(50, 50, new Vector2(0, 0));

        MarkObstacles(map);
        MarkSafeZone(map);
        MarkPlant(map);

        baseTex = GenerateMiniMapTexture(map);
        miniMapImage.texture = baseTex;



        _botLastPosition = bot.transform.position;

        

    }

    private void Update()
    {
        bool botMoved = Vector3.Distance(_botLastPosition, bot.transform.position) > 0.01f;

        if (botMoved)
        {
            MarkBot(map);
            baseTex = GenerateMiniMapTexture(map);
        }

        if (_currentPath != null)
        {
            miniMapImage.texture = OverlayPathOnMiniMap(baseTex, _currentPath);
        }
        else
        {
            miniMapImage.texture = baseTex;
        }




    }


    // ============================
    //      LLAMAR A*
    // ============================
    public void RequestPathTo(Vector3 target)
    {
        // evitar recalcular si es el mismo punto

        /*
        if (Vector3.Distance(_lastTarget, target) < 0.1f)
            return;
        */


        //Debugs
        int sx, sy, tx, ty;

        bool okStart = map.WorldToIndex(new Vector2(bot.transform.position.x, bot.transform.position.z), out sx, out sy);
        bool okEnd = map.WorldToIndex(new Vector2(target.x, target.z), out tx, out ty);

        Debug.Log($"Start OK: {okStart}, Index=({sx},{sy})");
        Debug.Log($"End OK: {okEnd}, Index=({tx},{ty})");


        if (okStart)
            Debug.Log("Start state = " + map.matrix[sx][sy].state);

        if (okEnd)
            Debug.Log("End state = " + map.matrix[tx][ty].state);


        //Buscar el camino 
        _lastTarget = target;
        _currentPath = map.FindPath(new Vector2(bot.transform.position.x, bot.transform.position.z),
                                    new Vector2(target.x, target.z));

        Debug.Log("Peticion hecha. Mapa recorrido");
        string res="";

        foreach (Cell t in _currentPath)
        {
            res+= "(" + t.x + ", " + t.y + "), ";
        }
        Debug.Log(res);
    }

    public Texture2D OverlayPathOnMiniMap(Texture2D baseTex, List<Cell> path)
    {
        if (path == null) return baseTex;

        Texture2D tex = new Texture2D(baseTex.width, baseTex.height);
        tex.SetPixels(baseTex.GetPixels()); // copiar

        foreach (var c in path)
        {
            tex.SetPixel(c.x, c.y, Color.blue); // color del camino
        }

        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }







    //Cambia los tiles del mapa a Obstacle segun la posicion y dimension de los objetos en 
    // el juego marcados con la Tag "Obstacle"
    public void MarkObstacles(Map map)
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

    public void MarkPlant(Map map)
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

    public void MarkBot(Map map)
    {
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


    public void MarkSafeZone(Map map)
    {
            Vector3 obPos = safeZone.transform.position;
            Vector3 obSca = safeZone.transform.localScale;

            map.ChangeTilesInMap(new Vector2(obPos.x - (obSca.x / 2), obPos.z - (obSca.z / 2)),
                                 new Vector2(obPos.x + (obSca.x / 2), obPos.z + (obSca.z / 2)),
                                 CellState.SAFE_ZONE);
    }






    // Funciones para generar el minimapa visual

    public Texture2D GenerateMiniMapTexture(Map map)
    {
        Texture2D tex = new Texture2D(map.sizeX, map.sizeY);

        for (int x = 0; x < map.sizeX; x++)
        {
            for (int y = 0; y < map.sizeY; y++)
            {
                tex.SetPixel(x, y, GetColor(map.matrix[x][y].state));
            }
        }

        tex.filterMode = FilterMode.Point; // pixel-art look
        tex.Apply();

        return tex;
    }

    private Color GetColor(CellState state)
    {
        switch (state)
        {
            case CellState.GROUND: return Color.white;
            case CellState.OBSTACLE: return Color.black;
            case CellState.SAFE_ZONE: return Color.green;
            case CellState.DISCARD_ZONE: return Color.red;
            case CellState.UNKNOWN_PLANT: return Color.gray;
            case CellState.HEALTHY_PLANT: return new Color(0.2f, 0.8f, 0.2f);
            case CellState.SICK_PLANT: return Color.yellow;
            case CellState.HARVESTED_PLANT: return new Color(0.6f, 0.3f, 0f);
            case CellState.PURGED_PLANT: return Color.magenta;
            case CellState.PLAYER: return Color.cyan;
            default: return Color.white;
        }
    }



}

