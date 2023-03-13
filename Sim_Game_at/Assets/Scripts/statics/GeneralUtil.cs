using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneralUtil
{

    #region names
    public static string[] femaleNames = new string[] { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Mia", "Charlotte", "Amelia", "Evelyn", "Abigail", "Harper", "Emily", "Elizabeth", "Avery", "Sofia", "Ella", "Madison", "Scarlett", "Victoria", "Aria", "Grace", "Chloe", "Camila", "Penelope", "Riley", "Layla", "Lillian", "Nora", "Zoey", "Mila", "Aubrey", "Hannah", "Lily", "Addison", "Eleanor", "Natalie", "Luna", "Savannah", "Brooklyn", "Leah", "Zoe", "Stella", "Hazel", "Ellie", "Paisley", "Audrey", "Skylar", "Violet", "Claire" };

    public static string[] maleNames = new string[] { "Liam", "Noah", "William", "James", "Oliver", "Benjamin", "Elijah", "Lucas", "Mason", "Logan", "Alexander", "Ethan", "Jacob", "Michael", "Daniel", "Henry", "Jackson", "Sebastian", "Aiden", "Matthew", "Samuel", "David", "Joseph", "Carter", "Owen", "Wyatt", "John", "Jack", "Luke", "Jayden", "Dylan", "Grayson", "Levi", "Isaac", "Gabriel", "Julian", "Mateo", "Anthony", "Jaxon", "Lincoln", "Joshua", "Christopher", "Andrew", "Theodore", "Caleb", "Ryan", "Asher", "Nathan", "Thomas", "Leo" };

    #endregion


    public static Color colorBrown = new Color(165.0f / 255, 42.0f / 255, 42.0f / 255, 1);

    public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 } };

    public static UserUIManager Ui;
    public static MapCreation map;
    public static ResourceBank resourceBank;
    public static DataHolder dataBank;
    public static TimeCycle timeCycle;
    public static Graph graphRef;
    public static MapInteraction mapInteraction;

    public static BuildingTypes buildingScritpable;



    public static Dictionary<Tile, BuildingIdentifier> entranceTileDict = new Dictionary<Tile, BuildingIdentifier>();

    public static Tuple<List<Tile>, List<Tile>> A_StarPathfinding(Vector2Int start, Vector2Int end, AgentData npc, bool forced = false)
    {
        var tileArray2D = map.tilesArray;

        if (end.x < 0 || end.y < 0 || end.x >= tileArray2D.GetLength(1) || end.y >= tileArray2D.GetLength(0))
        {
            return null;
        }

        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();

        AStar_Node start_node = new AStar_Node(tileArray2D[start.x, start.y]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.x, end.y]);

        int[,] childPosArry = new int[0, 0];

        childPosArry = childPosArry4Side;

        openList.Add(start_node);

        List<AStar_Node> allChecked = new List<AStar_Node>();

        int iter = 0;

        while (openList.Count > 0)
        {
            iter++;

            AStar_Node currNode = openList[0];
            int currIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < currNode.f)
                {
                    currNode = openList[i];
                    currIndex = i;
                }
            }

            openList.RemoveAt(currIndex);

            closedList.Add(currNode);

            allChecked.Add(currNode);

            if (currNode.refToBasicTile.coord.x == end_node.refToBasicTile.coord.x && currNode.refToBasicTile.coord.y == end_node.refToBasicTile.coord.y)
            {
                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }

                var pathOfBasicTiles = new List<Tile>();
                var allCehckedTiles = new List<Tile>();

                foreach (var tile in path)
                {
                    pathOfBasicTiles.Add(tile.refToBasicTile);
                }

                foreach (var tile in allChecked)
                {
                    allCehckedTiles.Add(tile.refToBasicTile);
                }

               // Debug.Log($"{npc.name} has taken {iter} iters to workout its destination");

                pathOfBasicTiles.Reverse();
                return Tuple.Create(pathOfBasicTiles, allCehckedTiles);
            }
            else
            {
                List<AStar_Node> children = new List<AStar_Node>();

                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.refToBasicTile.coord.x + x_buff, currNode.refToBasicTile.coord.y + y_buff };

                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= tileArray2D.GetLength(1) || node_position[1] >= tileArray2D.GetLength(0))
                    {
                        continue;
                    }
                    else
                    {
                        AStar_Node new_node = new AStar_Node(tileArray2D[node_position[0], node_position[1]]);
                        children.Add(new_node);
                    }
                }

                foreach (var child in children)
                {
                    bool alreadyThere = false;

                    for (int i = 0; i < closedList.Count; i++)
                    {
                        if (child.refToBasicTile.coord.x == closedList[i].refToBasicTile.coord.x && child.refToBasicTile.coord.y == closedList[i].refToBasicTile.coord.y)
                        {
                            alreadyThere = true;
                            break;
                        }
                    }

                    if (alreadyThere == false) 
                    {
                        child.g = currNode.g + 0.5f;
                        child.h = EuclideanDistance2D(new Vector2(end_node.refToBasicTile.coord.x, end_node.refToBasicTile.coord.y), new Vector2(child.refToBasicTile.coord.x, child.refToBasicTile.coord.y));

                        child.f = child.g + child.h + tileCosts[child.refToBasicTile.tileType];   //added value here
                        child.parent = currNode;

                        bool alreadyThereAgain = false;

                        foreach (var openListItem in openList)
                        {
                            if (child.refToBasicTile.coord.x == openListItem.refToBasicTile.coord.x && child.refToBasicTile.coord.y == openListItem.refToBasicTile.coord.y && child.g > openListItem.g)// 
                            {
                                alreadyThereAgain = true;
                                break;
                            }
                        }

                        if (alreadyThereAgain == false)
                            openList.Add(child);
                    }
                }
            }
        }

        return null;
    }

    public static float EuclideanDistance2D(Vector2 point1, Vector2 point2)
    {
        return MathF.Sqrt(MathF.Pow((point1.x - point2.x), 2) + MathF.Pow((point1.y - point2.y), 2));
    }

    public static bool AABBCol(Vector3 player, Tile tile)
    {
        if (player.x >= tile.BotLeft.x && player.x < tile.TopRight.x)
        {
            if (player.z >= tile.BotLeft.z && player.z < tile.TopRight.z)
            {
                return true;
            }
        }
        return false;
    }

    public static Tile ReturnTile(Vector2Int pos) 
    {
        return map.tilesArray[pos.x, pos.y];
    }

    public static Tile RandomTileAround(int range, Vector2Int centerPos, List<int> AllowedTypes = null, int tries = 10) 
    {
        for (int i = 0; i < tries; i++)
        {
            int ranAdditionX = Random.Range(-range, range);
            int ranAdditionY = Random.Range(-range, range);

            var newDesiredPos = new Vector2Int(centerPos.x + ranAdditionX, centerPos.y + ranAdditionY);

            if (newDesiredPos.x < 0  ||  newDesiredPos.y < 0 ||  newDesiredPos.x >= map.tilesArray.GetLength(0) || newDesiredPos.y >= map.tilesArray.GetLength(0)) 
            {
                // out of range
            }
            else 
            {
                var tile = ReturnTile(newDesiredPos);
                if (AllowedTypes.Count == 0) // prb not needed as i can just for loop and does it its self
                {
                    return tile;
                }
                else 
                {
                    for (int j = 0; j < AllowedTypes.Count; j++)
                    {
                        if ((int)tile.tileType == AllowedTypes[j]) 
                        {
                            return tile;
                        }
                    }
                }
            }
        }

        return ReturnTile(centerPos);
    }


    
    public static int GetRandomNumberExcludingRange(int min, int max, int excludeStart, int excludeEnd)
    {
        int randomNumber = Random.Range(min, max);

        while (randomNumber >= excludeStart && randomNumber <= excludeEnd)
        {
            randomNumber = Random.Range(min, max);
        }

        return randomNumber;
    }

    /// <summary>
    /// returns true if it contain the wanted type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool PathContainsTileType(TileType type, List<Tile> path)
    {
        foreach (var tile in path)
        {
            if (tile.tileType == type)
                return true;
        }

        return false;
    }

    public static Tile WorldPosToTile(Vector3 point) 
    {
        float pointX = point.x;
        float pointY = point.z;

        float tileSize = Vector3.Distance( map.tilesArray[1,1].BotRight, map.tilesArray[1, 1].BotLeft);
        int tileX = Mathf.FloorToInt(pointX / tileSize);
        int tileY = Mathf.FloorToInt(pointY / tileSize);

        return map.tilesArray[tileX, tileY];
    }

    public static Tile Vector2Tile(Vector2Int cord) { return map.tilesArray[cord.x, cord.y]; }

    // not used
    public static List<Tile> GetResourcesCloseSpiral(Vector2Int start, int range)
    {
        var tileCloseBy = new List<Tile>();
        tileCloseBy.Add(map.tilesArray[start.x, start.y]);

        int dir = 0;
        // 0 right
        // 1 up
        // 2 left
        // 3 down
        
        int countingDirection = 0;  // this counts the amoutn of ups
        bool countedThisCycle = false;

        int currentDirectionCycle = -1;
        int step = 1;
        //untill counting direction is biggar than range
        var currentHeadPos = start;

        while (true)
        {

            if (countingDirection >= range) 
                break;

            currentDirectionCycle++;    

            for (int i = 0; i < step; i++)
            {
                switch (dir)
                {
                    case 0:
                        currentHeadPos = new Vector2Int(currentHeadPos.x + 1, currentHeadPos.y);

                        if (countedThisCycle == false)
                        {
                            countingDirection++;
                            countedThisCycle = true;
                        }

                        break;

                    case 1:
                        currentHeadPos = new Vector2Int(currentHeadPos.x, currentHeadPos.y + 1);
                        countedThisCycle = false;
                        break;

                    case 2:
                        currentHeadPos = new Vector2Int(currentHeadPos.x - 1, currentHeadPos.y);

                        break;

                    case 3:
                        currentHeadPos = new Vector2Int(currentHeadPos.x, currentHeadPos.y - 1);

                        break;

                    default:
                        break;
                }


                if (currentHeadPos.x < 0 || currentHeadPos.y < 0 || currentHeadPos.x >= map.tilesArray.GetLength(1) || currentHeadPos.y >= map.tilesArray.GetLength(0)) { }
                else
                {
                    tileCloseBy.Add(map.tilesArray[currentHeadPos.x, currentHeadPos.y]);
                }

            }
            dir++;

            if (dir >= 4)
                dir = 0;

            if (currentDirectionCycle == 1)
            {
                currentDirectionCycle = -1;
                step++;

            }
        }

        return tileCloseBy;
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }


    #region Dicts
    //the cost of pathfidnign for each tile
    public static Dictionary<TileType, float> tileCosts = new Dictionary<TileType, float>()
    {
        {TileType.GRASS, 1.2f},
        {TileType.HILL, 3f},
        {TileType.SNOW, 6f},
        {TileType.WATER, 8f},
        {TileType.NULL, 30},
        {TileType.PATH, 0.3f},
        {TileType.BLOCKED, 30},
        {TileType.ENTRANCE, 0f}
    };

    #endregion
}

public class DistanceComparer : IComparer<Collider>
{
    private Transform referenceTransform;

    public DistanceComparer(Transform referenceTransform)
    {
        this.referenceTransform = referenceTransform;
    }

    public int Compare(Collider a, Collider b)
    {
        float distanceToA = Vector3.Distance(referenceTransform.position, a.transform.position);
        float distanceToB = Vector3.Distance(referenceTransform.position, b.transform.position);

        return distanceToA.CompareTo(distanceToB);
    }
}

public class AStar_Node
{
    public Tile refToBasicTile;
    public AStar_Node parent;

    public float g = 0;
    public float f = 0;
    public float h = 0;

    public AStar_Node(Tile basicTile)
    {
        refToBasicTile = basicTile;
    }
}
