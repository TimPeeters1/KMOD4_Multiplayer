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
        public void GenerateGrid()
        {
            RoomGrid = new Room[GridSize, GridSize];

            for (int i = 0; i < RoomGrid.GetLength(0); i++)
            {
                for (int f = 0; f < RoomGrid.GetLength(1); f++)
                {
                    RoomGrid[i, f] = new Room();
                    RoomGrid[i, f].GetNeighbours(new Vector2(i, f), GridSize);
                    //Debug.Log(RoomGrid[i, f].currentDirections.ToString() + " | (" + i + "," + f + ")");
                }
            }
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