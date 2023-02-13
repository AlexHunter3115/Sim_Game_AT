using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtil
{

    #region names
    public static string[] femaleNames = new string[] { "Emily", "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Mia", "Charlotte", "Amelia", "Harper", "Evelyn", "Abigail", "Emily", "Elizabeth", "Sofia", "Avery", "Ella", "Madison", "Scarlett", "Victoria", "Aria", "Grace", "Chloe", "Camila", "Penelope", "Riley", "Layla", "Lillian", "Natalie", "Hazel", "Aubrey", "Lucy", "Audrey", "Sadie", "Makayla", "Aaliyah", "Aurora", "Ellie", "Arianna", "Allison", "Savannah", "Nora", "Reagan", "Adalynn", "Brooklynn", "Leah", "Anna", "Aurora", "Scarlet", "Mila", "Everly" };

    public static string[] maleNames = new string[] { "Liam", "Noah", "William", "James", "Oliver", "Benjamin", "Elijah", "Lucas", "Mason", "Logan", "Alexander", "Ethan", "Jacob", "Michael", "Daniel", "Henry", "Jackson", "Sebastian", "Aiden", "Matthew", "Samuel", "David", "Joseph", "Carter", "Owen", "Wyatt", "John", "Jack", "Luke", "Jayden", "Dylan", "Grayson", "Levi", "Isaac", "Gabriel", "Julian", "Mateo", "Anthony", "Jaxon", "Lincoln", "Joshua", "Christopher", "Andrew", "Theodore", "Caleb", "Ryan", "Asher", "Nathan", "Thomas", "Leo" };

    #endregion




    public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 } };

    public static UserUIManager Ui;
    public static MapCreation map;
    public static ResourceBank bank;
    public static DataHolder dataBank;
    



    public static List<Tile> A_StarPathfinding(Vector2Int start, Vector2Int end)
    {
        var tileArray2D = map.tilesArray;

        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();

        AStar_Node start_node = new AStar_Node(tileArray2D[start.x,start.y]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.x,end.y]);

        int[,] childPosArry = new int[0, 0];

        childPosArry = childPosArry4Side;



        openList.Add(start_node);

        int iter = 0;

        while (openList.Count > 0)
        {


            if (iter > 1000) 
            {
                //break;
            }

            //iter++;

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

                foreach (var tile in path)
                {
                    pathOfBasicTiles.Add(tile.refToBasicTile);
                }

                pathOfBasicTiles.Reverse();
                return pathOfBasicTiles;
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
                        if (tileArray2D[node_position[0], node_position[1]].tileType == TileType.NULL  || tileArray2D[node_position[0], node_position[1]].tileType == TileType.BLOCKED) 
                        {
                        
                        }
                        else
                        { 
                            //here an if statment also saying that walkable 
                            AStar_Node new_node = new AStar_Node(tileArray2D[node_position[0], node_position[1]]);
                            children.Add(new_node);
                        }
                    }
                }

                foreach (var child in children)
                {
                    foreach (var closedListItem in closedList)
                    {
                        if (child.refToBasicTile.coord.x == closedListItem.refToBasicTile.coord.x && child.refToBasicTile.coord.y == closedListItem.refToBasicTile.coord.y)
                        {
                            continue;
                        }
                    }


                    child.g = currNode.g + 0.5f;
                    child.h = EuclideanDistance2D(new Vector2(end_node.refToBasicTile.coord.x, end_node.refToBasicTile.coord.y), new Vector2(child.refToBasicTile.coord.x, child.refToBasicTile.coord.y));



                    child.f = child.g + child.h + tileCosts[child.refToBasicTile.tileType];   //added value here
                    child.parent = currNode;
                    
                   


                    foreach (var openListItem in openList)
                    {
                        if (child.refToBasicTile.coord.x == openListItem.refToBasicTile.coord.x && child.refToBasicTile.coord.y == openListItem.refToBasicTile.coord.y && child.g > openListItem.g)// 
                        {
                            continue;
                        }
                    }

                    openList.Add(child);

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







    #region Dicts

    public static Dictionary<BuildingData.BUILDING_TYPE, List<int>> ResourcesWSFSUpKeep = new Dictionary<BuildingData.BUILDING_TYPE, List<int>>
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new List<int>() {20,5,2,2 }  },
        {BuildingData.BUILDING_TYPE.HOUSE, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.SAWMILL, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.MINE, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.DOCK, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.FARM, new List<int>() {0,0,0,0 }  },
    };

    public static Dictionary<BuildingData.BUILDING_TYPE, List<int>> ResourcesWSFSStart = new Dictionary<BuildingData.BUILDING_TYPE, List<int>>
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new List<int>() {250,30,20,10 }  },
        {BuildingData.BUILDING_TYPE.HOUSE, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.SAWMILL, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.MINE, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.DOCK, new List<int>() {0,0,0,0 }  },
        {BuildingData.BUILDING_TYPE.FARM, new List<int>() {0,0,0,0 }  },
    };


    public static Dictionary<BuildingData.BUILDING_TYPE, List<int>> allowedDict = new Dictionary<BuildingData.BUILDING_TYPE, List<int>>()
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new List<int>() {0,1 }  },
        {BuildingData.BUILDING_TYPE.FARM, new List<int>() {0 }  },
        {BuildingData.BUILDING_TYPE.MINE, new List<int>() {1,2 }  },
        {BuildingData.BUILDING_TYPE.HOUSE, new List<int>() {0,1 }  },
        {BuildingData.BUILDING_TYPE.SAWMILL, new List<int>() {0 }  },
        {BuildingData.BUILDING_TYPE.DOCK, new List<int>() {0,3 }  }
    };

    public static Dictionary<BuildingData.BUILDING_TYPE, Vector2Int> buildingSize = new Dictionary<BuildingData.BUILDING_TYPE, Vector2Int>()
    {
        {BuildingData.BUILDING_TYPE.COUNCIL, new Vector2Int(5,5)  },
        {BuildingData.BUILDING_TYPE.FARM,  new Vector2Int(5,10)  },
        {BuildingData.BUILDING_TYPE.MINE,  new Vector2Int(3,5)  },
        {BuildingData.BUILDING_TYPE.HOUSE,  new Vector2Int(5,5)  },
        {BuildingData.BUILDING_TYPE.SAWMILL,  new Vector2Int(3,5)  },
        {BuildingData.BUILDING_TYPE.DOCK,  new Vector2Int(3,7)  }
    };


    public static Dictionary<TileType, float> tileCosts = new Dictionary<TileType, float>()
    {
        {TileType.GRASS, 0.15f},
        {TileType.HILL, 0.3f},
        {TileType.SNOW, 0.6f},
        {TileType.WATER, 0.9f},
        {TileType.NULL, 10000f},
        {TileType.PATH, 0},
        {TileType.BLOCKED, 10000f}
    };


    #endregion


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
