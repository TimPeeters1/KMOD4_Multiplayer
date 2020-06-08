using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class Door : MonoBehaviour
    {
        public Room.RoomDirections DoorDirection;
        public GameObject Arrow;
        public bool IsHovered;

        private void Start()
        {
            Arrow.SetActive(false);
        }

        public void Update()
        {
            if (IsHovered)
            {
                Arrow.SetActive(true);
            }
            else
            {
                Arrow.SetActive(false);
            }
        }

    }
}