using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BSP
{
    private class SubDungeon
    {
        public int x, y, width, height;

        public SubDungeon(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }


    }
    // Basic BSP Dungeon generation
    public static CellMap Generate(int width,int height,int maxDepth,int corridorWidth,int minimumRoomSize)
    {

        SubDungeon dungeon = new SubDungeon(0, 0, width, height);
        List<BinaryTree<SubDungeon>.Node> treeLists;
        void SplitSubDungeon(BinaryTree<SubDungeon>.Node dungeonNode)
        {
            var dung = dungeonNode.item;
            float maxRatio = 1.25f;
            bool splitVertically;
            // choose split direction
            {
                bool verticalSplitAvailable = dung.width >= 2 * minimumRoomSize + 4;
                bool horizontalSplitAvailable = dung.height >= 2 * minimumRoomSize + 4;
                if (!verticalSplitAvailable && !horizontalSplitAvailable)
                    return;

                if (horizontalSplitAvailable && verticalSplitAvailable)
                    splitVertically = MyRandom.Int(0, dung.width + dung.height) < dung.width;
                else
                    splitVertically = verticalSplitAvailable;

                if (verticalSplitAvailable && dung.width >= dung.height * maxRatio)
                    splitVertically = true;
                else if (horizontalSplitAvailable && dung.height >= dung.width * maxRatio)
                    splitVertically = false;
            }

            if (splitVertically)
            {
                //start of new room
                int width1 = MyRandom.Int(minimumRoomSize + 2, dung.width - minimumRoomSize - 1);
                var subDung1 = new SubDungeon(dung.x,          dung.y, width1,              dung.height);
                var subDung2 = new SubDungeon(dung.x + width1, dung.y, dung.width - width1, dung.height);
                treeLists.Add(dungeonNode.SetChildLeft(subDung1));
                treeLists.Add(dungeonNode.SetChildRight(subDung2));
            }
            else
            {
                //start of new room
                int height1 = MyRandom.Int(minimumRoomSize + 2, dung.height - minimumRoomSize - 1);
                var subDung1 = new SubDungeon(dung.x, dung.y,           dung.width, height1              );
                var subDung2 = new SubDungeon(dung.x, dung.y + height1, dung.width, dung.height - height1);
                treeLists.Add(dungeonNode.SetChildLeft(subDung1));
                treeLists.Add(dungeonNode.SetChildRight(subDung2));
            }
        }
        var tree = new BinaryTree<SubDungeon>();
        tree.SetRoot(dungeon);
        treeLists = new List<BinaryTree<SubDungeon>.Node>() { tree.root };
        for (int i = 0; i < maxDepth; i++)
        {
            var oldTreeLists = treeLists;
            treeLists = new List<BinaryTree<SubDungeon>.Node>();
            foreach (var listNode in oldTreeLists)
                SplitSubDungeon(listNode);
            if (treeLists.Count == 0)
                break;
        }


        const int EMPTY = 0, HALL = 1, CORRIDOR = 2;
        int[,] map = new int[width, height];

        foreach(var node in tree)
        {
            if (!node.IsList)
                continue;
            var subdung = node.item;
            int roomWidth = MyRandom.Int(minimumRoomSize, subdung.width - 1);
            int roomHeight = MyRandom.Int(minimumRoomSize, subdung.height - 1);
            int roomX = MyRandom.Int(1, subdung.width - roomWidth-1) + subdung.x;
            int roomY = MyRandom.Int(1, subdung.height - roomHeight-1) + subdung.y;
            for (int i = 0; i < roomWidth; i++)
            {
                for (int j = 0; j < roomHeight; j++)
                {
                    map[roomX + i, roomY + j] = HALL;
                }
            }
        }


        var cm = new CellMap(width, height);
        cm.AddNewRoom(RoomType.Hall);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == HALL)
                    cm.SetCell(i, j, TileType.Cobblestone);
            }
        }

        return cm;
    }
}
