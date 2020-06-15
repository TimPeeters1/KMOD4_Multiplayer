using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class RoomObject : MonoBehaviour
    {
        public Room.RoomDirections CurrentDirections;

        [SerializeField] GameObject[] directionDoors; //Door Gameobjects to switch on/off based on direction.
        public GameObject[] spawnPositions; //Player Spawn Positions
        public GameObject ChestObject;
        public GameObject DungeonExitObject;
        public GameObject MonsterObject;

        public void SetRoomProperties(Room.RoomDirections _directions, ushort _treasureAmount, byte _containsMonster, byte _containsExit)
        {
            if(_treasureAmount > 0)
            {
                ChestObject.SetActive(true);
                ChestObject.GetComponent<Chest>().TreasureAmount = _treasureAmount;
            }
            else
            {
                ChestObject.SetActive(false);
            }

            if (_containsMonster > 0)
            {
                MonsterObject.SetActive(true);
                ResetMonster();
            }
            else
            {
                MonsterObject.SetActive(false);
            }

            if (_containsExit > 0)
            {
                DungeonExitObject.SetActive(true);
            }
            else
            {
                DungeonExitObject.SetActive(false);
            }

            CurrentDirections = _directions;
            //Debug.Log(Convert.ToString((int)CurrentDirections, 2).PadLeft(4, '0'));

            for (int i = 0; i < directionDoors.Length; i++)
            {
                directionDoors[i].SetActive(false);
            }

            //TODO Clean this piece up with a switch or something? Idk.
            if (CurrentDirections.HasFlag(Room.RoomDirections.North))
            {
                directionDoors[0].SetActive(true);
            }
            if (CurrentDirections.HasFlag(Room.RoomDirections.East))
            {
                directionDoors[1].SetActive(true);
            }
            if (CurrentDirections.HasFlag(Room.RoomDirections.South))
            {
                directionDoors[2].SetActive(true);
            }
            if (CurrentDirections.HasFlag(Room.RoomDirections.West))
            {
                directionDoors[3].SetActive(true);
            }
        }

        public void ResetMonster()
        {

        }
    }
}