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

        public void SetRoomDirections(Room.RoomDirections _directions)
        {
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
    }
}