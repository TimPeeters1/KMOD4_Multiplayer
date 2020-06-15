using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;

namespace MutiplayerSystem
{
    public class Chest : Interactable
    {
        public Room.RoomDirections DoorDirection;
        public GameObject SelectionItem;
        public ushort TreasureAmount;

        private void Start()
        {
            SelectionItem.SetActive(false);
        }

        public void Update()
        {
            if (IsHovered)
            {
                SelectionItem.SetActive(true);
                SelectionItem.GetComponentInChildren<UnityEngine.UI.Text>().text = "Claim Treasure\n" + TreasureAmount + "$";
            }
            else
            {
                SelectionItem.SetActive(false);
            }
        }

        public override void DoInteraction()
        {
            var claimRequest = new ClaimTreasureRequest();

            var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
            claimRequest.SerializeObject(ref writer);
            ClientBehaviour.Instance.m_Driver.EndSend(writer);
        }
    }
}