using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MutiplayerSystem
{
    public class Room : MonoBehaviour
    {
        public Vector2 GridPosition;
        public ushort TreasureAmount;
        public byte ContainsMonster;
        public byte ContainsExit;

        [Flags]
        public enum RoomDirections
        {
            None = 0,
            North = 1,
            East = 2,
            South = 4,
            West = 8
        }

        public RoomDirections possibleDirections; //All directions the player can move to in this room.

        public List<Player> PlayersInRoom = new List<Player>();

        public void GetNeighbours(Vector2 _gridPosition, int _gridSize)
        {
            GridPosition = _gridPosition;

            //Corners
            if (_gridPosition.x == 0 && _gridPosition.y == 0) //Linksonder
            {
                possibleDirections = RoomDirections.North | RoomDirections.East;
            }
            else if (_gridPosition.x == _gridSize - 1 && _gridPosition.y == 0) //Rechtsonder
            {
                possibleDirections = RoomDirections.North | RoomDirections.West;
            }
            else if (_gridPosition.x == 0 && _gridPosition.y == _gridSize - 1) //Linksboven
            {
                possibleDirections = RoomDirections.East | RoomDirections.South;
            }
            else if (_gridPosition.x == _gridSize - 1 && _gridPosition.y == _gridSize - 1) //Rechtsboven
            {
                possibleDirections = RoomDirections.West | RoomDirections.South;
            }

            //Row Bounds
            else if (_gridPosition.x == _gridSize - 1)
            {
                possibleDirections = RoomDirections.North | RoomDirections.South | RoomDirections.West;
            }
            else if (_gridPosition.y == _gridSize - 1)
            {
                possibleDirections = RoomDirections.East | RoomDirections.South | RoomDirections.West;
            }
            else if (_gridPosition.x == 0)
            {
                possibleDirections = RoomDirections.North | RoomDirections.East | RoomDirections.South;
            }
            else if (_gridPosition.y == 0)
            {
                possibleDirections = RoomDirections.North | RoomDirections.East | RoomDirections.West;
            }
            else
            {
                possibleDirections = (RoomDirections)15;
            }
        }

        public Vector2Int MoveToNeighbour(RoomDirections _direction)
        {
            Vector2Int result = new Vector2Int(0, 0);
            if (_direction == RoomDirections.North)
            {
                result = new Vector2Int(0, 1);
            }
            else if (_direction == RoomDirections.East)
            {
                result = new Vector2Int(1, 0);
            }
            else if (_direction == RoomDirections.South)
            {
                result = new Vector2Int(0, -1);
            }
            else if (_direction == RoomDirections.West)
            {
                result = new Vector2Int(-1, 0);
            }

            return result;
        }
    }
}