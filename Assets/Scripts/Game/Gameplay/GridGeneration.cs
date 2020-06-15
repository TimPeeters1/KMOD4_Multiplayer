using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] int GridSize;

        public Room[,] RoomGrid;

        public ushort[] TreasureAmounts = { 0, 10, 20};

        public void GenerateGrid()
        {
            RoomGrid = new Room[GridSize, GridSize];



            for (int i = 0; i < RoomGrid.GetLength(0); i++)
            {
                for (int f = 0; f < RoomGrid.GetLength(1); f++)
                {
                    RoomGrid[i, f] = new Room();
                    RoomGrid[i, f].GetNeighbours(new Vector2(i, f), GridSize);
                    RoomGrid[i, f].TreasureAmount = TreasureAmounts[UnityEngine.Random.Range(0, TreasureAmounts.Length - 1)];
                    RoomGrid[i, f].ContainsMonster = (byte)UnityEngine.Random.Range(0, 2);
        
                    //Debug.Log(RoomGrid[i, f].ContainsMonster + " | (" + i + "," + f + ")");
                }

            }

            RoomGrid[0, 0].TreasureAmount = 0;
            RoomGrid[0, 0].ContainsMonster = 0;
            RoomGrid[GridSize - 1, GridSize - 1].ContainsExit = 1;
        }

        //private void FixedUpdate()
        //{
        //    for (int i = 0; i < RoomGrid.GetLength(0); i++)
        //    {
        //        for (int f = 0; f < RoomGrid.GetLength(1); f++)
        //        {
        //            Debug.Log(RoomGrid[i, f].PlayersInRoom.Count + " | (" + i + "," + f + ")");
        //        }
        //    }
        //}
    }
}