using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class Door : Interactable
    {
        public Room.RoomDirections DoorDirection;
        public GameObject Arrow;

        private void Start()
        {
            Arrow.SetActive(false);
        }

        public void Update()
        {
            if (IsHovered && !GameManager.Instance.RoomGameObject.MonsterObject.activeSelf)
            {
                Arrow.SetActive(true);
            }
            else
            {
                Arrow.SetActive(false);
            }
        }

        public override void DoInteraction()
        {
            if (!GameManager.Instance.RoomGameObject.MonsterObject.activeSelf)
            {
                var moveRequest = new MoveRequest()
                {
                    MoveDirection = DoorDirection
                };

                var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
                moveRequest.SerializeObject(ref writer);
                ClientBehaviour.Instance.m_Driver.EndSend(writer);
            }
        }

    }
}