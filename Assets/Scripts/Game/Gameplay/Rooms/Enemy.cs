using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutiplayerSystem
{
    public class Enemy : Interactable
    {
        public GameObject UI;

        private void Start()
        {
            UI.SetActive(false);
        }

        public void Update()
        {
            if (IsHovered)
            {
                UI.SetActive(true);
            }
            else
            {
                UI.SetActive(false);
            }
        }

        public override void DoInteraction()
        {
           
        }

        public void DoAttack()
        {
            var attackMessage = new AttackRequest();

            var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
            attackMessage.SerializeObject(ref writer);
            ClientBehaviour.Instance.m_Driver.EndSend(writer);
        }

        public void DoDefense()
        {
            var defenseMessage = new DefendRequest();

            var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
            defenseMessage.SerializeObject(ref writer);
            ClientBehaviour.Instance.m_Driver.EndSend(writer);
        }

    }
}