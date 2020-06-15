using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class DungeonExit : Interactable
    {
        public GameObject Arrow;

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

        public override void DoInteraction()
        {
            var leaveDungeon = new LeaveDungeonRequest();

            var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
            leaveDungeon.SerializeObject(ref writer);
            ClientBehaviour.Instance.m_Driver.EndSend(writer);
        }

    }
}