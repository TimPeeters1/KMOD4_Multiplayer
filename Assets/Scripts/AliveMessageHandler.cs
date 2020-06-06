using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem 
{
    public class AliveMessageHandler : MonoBehaviour
    {
        public void InitializeMessage(float time, bool isClient)
        {
            StartCoroutine(SendPackage(time, isClient));
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        IEnumerator SendPackage(float _time, bool _isClient)
        {
            while (true)
            {
                if (_isClient)
                {
                    yield return new WaitForSeconds(_time);
                    //Debug.Log("Send Alive Message");
                    var writer = ClientBehaviour.Instance.m_Driver.BeginSend(ClientBehaviour.Instance.m_Connection);
                    writer.WriteUShort(0);
                    ClientBehaviour.Instance.m_Driver.EndSend(writer);
                }
                else
                {
                    yield return new WaitForSeconds(_time);
                    //Debug.Log("Send Alive Message");
                    for (int i = 0; i < ServerBehaviour.Instance.m_Connections.Length; i++)
                    {
                        var writer = ServerBehaviour.Instance.m_Driver.BeginSend(ServerBehaviour.Instance.m_Connections[i]);
                        writer.WriteUShort(0);
                        ServerBehaviour.Instance.m_Driver.EndSend(writer);
                    }
                }
            }
        }
    }
}
