using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// 2D grid graph
/// </summary>
public class Graph
{
    private class Node
    {
        public Edge Up, Right, Down, Left;
        public RoomType type;
        public IEnumerable<Edge> Edges()
        {
                yield return Up;
                yield return Right;
                yield return Down;
                yield return Left;
        }

        public Node() { }

        public Node(RoomType type)
        {
            this.type = type;
        }

        public int Degree
        {
            get => Edges().Count(e => e != null);
        }
    }
    private class Edge
    {
        public Node node1, node2;
        public bool vertical;
        public bool horizontal
        {
            get => !vertical;
            set => vertical = !value;
        }

        public Edge(Node node1, Node node2, bool vertical)
        {
            this.node1 = node1;
            this.node2 = node2;
            this.vertical = vertical;
            if (vertical)
            {
                node1.Down = this;
                node2.Up = this;
            }
            else
            {
                node1.Right = this;
                node2.Left = this;
            }
        }
    }

    private static readonly Vector2[] edgeVectors = new Vector2[]
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };

    List<Node> nodes;
    List<Edge> edges;

    public int StartNodeDistanceXInGraph { get; private set; }

    /// <summary>
    /// Create a graph containing 2 nodes and 1 edge between them
    /// </summary>
    public static Graph Simple()
    {
        Graph g = new Graph();


        Node start = new Node();
        start.type = RoomType.Start;
        g.nodes.Add(start);
        Node end = new Node();
        end.type = RoomType.End;
        g.nodes.Add(end);
        switch (MyRandom.Int(0, 4))
        {
            case 0:
                g.edges.Add(new Edge(start, end, false));
                break;
            case 1:
                g.edges.Add(new Edge(start, end, true));
                break;
            case 2:
                g.edges.Add(new Edge(end, start, false));
                break;
            case 3:
                g.edges.Add(new Edge(end, start, true));
                break;
            default:
                break;
        }

        return g;
    }

    /// <summary>
    /// Drunkard walk on graph
    /// </summary>
    /// <param name="target">the target position of drunkard</param>
    /// <param name="randomMoveChance">a value between 0 and 1. Chance of drunkard moving at random instead towards its target</param>
    /// <param name="numOfAgents">How many drunkard agents we send</param>
    /// <param name="banNegativeY">Block drunkard from the bottom half of the grid</param>
    /// <returns></returns>
    public static Graph WalkToTarget(Vector2 target,float randomMoveChance,int numOfAgents,bool banNegativeY = true)
    {
        if(banNegativeY && target.y<0)
        {
            Log.Error("Cant walk to negative y when banNegativeY is true", null);
            return null;
        }

        var g = new Graph();

        Dictionary<Vector2, Node> map = new Dictionary<Vector2, Node>();
        var startNode = new Node(RoomType.Start);
        map.Add(Vector2.zero, startNode);
        g.nodes.Add(startNode);

        for (int agent = 0; agent < numOfAgents; agent++)
        {
            Vector2 agentPosition = new Vector2(0, 0);
            Node currentNode = null;
            while (agentPosition != target) 
            {
                Vector2 move;
                if (randomMoveChance > MyRandom.Float(0,1))
                {
                    move = edgeVectors[MyRandom.Int(0, 4)];
                    if (banNegativeY && move == Vector2.down && move.y + agentPosition.y < 0)
                        continue;
                }
                else
                {
                    int dVertical = (int)(target.y - agentPosition.y);
                    int dHorizontal = (int)(target.x - agentPosition.x);
                    bool moveHorizontal = Mathf.Abs(dHorizontal) > MyRandom.Int(0, Mathf.Abs(dHorizontal) + Mathf.Abs(dVertical));
                    move = moveHorizontal ? edgeVectors[(dHorizontal > 0) ? 1 : 3] : edgeVectors[(dVertical > 0) ? 0 : 2];
                }
                Node prevNode = map[agentPosition];
                agentPosition += move;
                if (!map.TryGetValue(agentPosition, out currentNode))
                {
                    currentNode = new Node(RoomType.Hall);
                    map.Add(agentPosition, currentNode);
                    g.nodes.Add(currentNode);
                }
                if(move == Vector2.up && prevNode.Up is null)
                {
                    g.edges.Add(new Edge(currentNode, prevNode, true));
                }
                else if (move == Vector2.left && prevNode.Left is null)
                {
                    g.edges.Add(new Edge(currentNode, prevNode, false));
                }
                else if (move == Vector2.down && prevNode.Down is null)
                {
                    g.edges.Add(new Edge(prevNode, currentNode, true));
                }
                else if (move == Vector2.right && prevNode.Right is null)
                {
                    g.edges.Add(new Edge(prevNode, currentNode, false));
                }
            }
            currentNode.type = RoomType.End;
        }

        return g;
    }

    private Graph()
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }


    /// <summary>
    /// Generate a cell map for a node
    /// </summary>
    /// <param name="roomWidth">width of room represented by node</param>
    /// <param name="roomHeight">height of room represented by node</param>
    /// <param name="roomType">type  of room</param>
    /// <returns></returns>
    private CellMap NodeToCellMap(int roomWidth, int roomHeight, RoomType roomType)
    {
        CellMap cm = new CellMap(roomWidth, roomHeight);
        cm.AddNewRoom(roomType);
        for (int x = 0; x < roomWidth; x++)
            for (int y = 0; y < roomHeight; y++)
                cm.SetCell(x, y, TileType.Cobblestone);

        return cm;
    }

    private static RoomType ChooseRandomRoomType(RoomType a, RoomType b)
        => MyRandom.Float() > 0.5f ? a : b;
    /// <summary>
    /// Generate a cell map from a graph
    /// </summary>
    /// <param name="roomWidth">width of room represented by node</param>
    /// <param name="roomHeight">height of room represented by node</param>
    /// <param name="corridorWidth">width of corridor represented by edge</param>
    /// <param name="corridorLength">length of corridor represented by edge</param>
    /// <param name="roomMergeProbability">(0-1) chance of corridor collapsing into a room merge</param>
    /// <returns></returns>
    public CellMap GenerateCellMap(int roomWidth, int roomHeight, int corridorWidth, int corridorLength, float roomMergeProbability)
        => GenerateCellMap(roomWidth, roomHeight, corridorWidth, corridorLength, roomMergeProbability, ChooseRandomRoomType);

    public CellMap GenerateCellMap(int roomWidth, int roomHeight, int corridorWidth, int corridorLength, float roomMergeProbability,
        Func<RoomType, RoomType, RoomType> chooseRoomTypeMergePredicate)
    {
        Dictionary<Vector2, Node> map = new Dictionary<Vector2, Node>();
        List<CellMap> rooms = new List<CellMap>();
        List<Vector2> roomsPos = new List<Vector2>();
        Queue<Vector2> queue = new Queue<Vector2>();

        List<List<object>> mergeGroups = new List<List<object>>();

        var objRoomsIndex = new Dictionary<object, int>();

        Node start = nodes[0];
        map.Add(new Vector2(0, 0), start);
        queue.Enqueue(new Vector2(0, 0));

        //traverse graph and create rooms from nodes
        while (queue.Count > 0)
        {
            
            Vector2 pos = queue.Dequeue();
            Node node = map[pos];
            int i = 0;

            //create corridors
            foreach(Edge edge in node.Edges())
            {
                if (edge != null)
                {
                    Node adjecentNode = (edge.node1 != node ? edge.node1 : edge.node2);
                    Vector2 adjecentNodePos = pos + edgeVectors[i];
                    if (!map.ContainsKey(adjecentNodePos))
                    {
                        map.Add(adjecentNodePos, adjecentNode);
                        queue.Enqueue(adjecentNodePos);
                    }
                    if (i < 2)
                    {
                        bool mergeEdge = MyRandom.Float() < roomMergeProbability;
                        if (mergeEdge)
                        {
                            if (edge.vertical)
                            {
                                rooms.Add(NodeToCellMap(roomWidth, corridorLength, RoomType.Corridor));
                                roomsPos.Add(new Vector2(
                                    pos.x * (roomWidth + corridorLength),
                                    pos.y * (roomHeight + corridorLength) + roomHeight
                                    ));
                            }
                            else
                            {
                                rooms.Add(NodeToCellMap(corridorLength, roomHeight, RoomType.Corridor));
                                roomsPos.Add(new Vector2(
                                    pos.x * (roomWidth + corridorLength) + roomWidth,
                                    pos.y * (roomHeight + corridorLength)
                                    ));

                            }
                            mergeGroups.Add(new List<object>() { edge, edge.node1, edge.node2 });
                        }
                        else
                        {
                            if (edge.vertical)
                            {
                                rooms.Add(NodeToCellMap(corridorWidth, corridorLength, RoomType.Corridor));
                                roomsPos.Add(new Vector2(
                                    pos.x * (roomWidth + corridorLength) + roomWidth / 2 - corridorWidth / 2,
                                    pos.y * (roomHeight + corridorLength) + roomHeight
                                    ));
                            }
                            else
                            {
                                rooms.Add(NodeToCellMap(corridorLength, corridorWidth, RoomType.Corridor));
                                roomsPos.Add(new Vector2(
                                    pos.x * (roomWidth + corridorLength) + roomWidth,
                                    pos.y * (roomHeight + corridorLength) + roomWidth / 2 - corridorWidth / 2
                                    ));
                            }
                        }
                        objRoomsIndex.Add(edge, rooms.Count-1);
                    }
                }
                i++;
            }

            //create room
            rooms.Add(NodeToCellMap(roomWidth, roomHeight,node.type));
            roomsPos.Add(new Vector2(
                pos.x * (roomWidth + corridorLength),
                pos.y * (roomHeight + corridorLength)
                ));
            objRoomsIndex.Add(node, rooms.Count - 1);
        }

        int minWidth = (int)map.Keys.Min(v => v.x);
        int minHeight = (int)map.Keys.Min(v => v.y);

        int mapWidthInNodes = (int)Math.Abs(map.Keys.Max(v => v.x) - minWidth) + 1;
        int mapHeightInNodes = (int)Math.Abs(map.Keys.Max(v => v.y) - minHeight) + 1;

        CellMap wholeMap = new CellMap(
            mapWidthInNodes * roomWidth + (mapWidthInNodes - 1) * corridorLength,
            mapHeightInNodes * roomWidth + (mapHeightInNodes - 1) * corridorLength
            );

        for (int i = 0; i < rooms.Count; i++)
        {
            wholeMap.InsertMap(
                (int)roomsPos[i].x - minWidth * (roomWidth + corridorLength),
                (int)roomsPos[i].y - minHeight * (roomHeight + corridorLength),
                rooms[i]);
        }

        // help to find start room in cellmap
        StartNodeDistanceXInGraph = -minWidth;

        return wholeMap;
    }


}
