using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtil
{

    public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 } };

    public static UserUIManager Ui;



    public static void TrialTest(UserUIManager u) => Ui = u;

    public static Tuple<List<Tile>, List<Tile>> A_StarPathfinding(Tile[,] tileArray2D, Vector2Int start, Vector2Int end)
    {
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();

        AStar_Node start_node = new AStar_Node(tileArray2D[start.x,start.y]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.x,end.y]);

        int[,] childPosArry = new int[0, 0];

        childPosArry = childPosArry8Side;



        openList.Add(start_node);

        while (openList.Count > 0)
        {

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

                var allVisiteBasicTiles = new List<Tile>();
                foreach (var tile in openList)
                {
                    allVisiteBasicTiles.Add(tile.refToBasicTile);
                }

                return new Tuple<List<Tile>, List<Tile>>(pathOfBasicTiles, allVisiteBasicTiles);
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
                        //here an if statment also saying that walkable 
                        AStar_Node new_node = new AStar_Node(tileArray2D[node_position[0],node_position[1]]);
                        children.Add(new_node);
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



                    child.f = child.g + child.h + child.refToBasicTile.cost;   //added value here
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
