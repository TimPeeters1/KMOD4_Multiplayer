﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace MutiplayerSystem
{
    class StartGame : Message
    {

        public ushort StartHP { get; set; }

        public StartGame()
        {
            Type = MessageType.StartGame;
        }

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteUShort(StartHP);
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            StartHP = reader.ReadUShort();
        }

    }
}

