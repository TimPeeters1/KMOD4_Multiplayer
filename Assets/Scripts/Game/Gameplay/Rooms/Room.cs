using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MutiplayerSystem;

public class Room : MonoBehaviour
{
    [System.Flags]
    public enum RoomDirections
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8
    }

    public RoomDirections currentDirections; //All directions the player can move to in this room.
    [SerializeField] GameObject[] directionDoors; //Door Gameobjects to switch on/off based on direction.
    public GameObject[] spawnPositions; //Player Spawn Positions

    private void Start()
    {
        for (int i = 0; i < directionDoors.Length; i++)
        {
            directionDoors[i].SetActive(false);
        }

        var color = (Color32)Color.white;
        uint encodedColor = (uint)DataUtilities.EncodeColor(color);
        Color newColor = DataUtilities.DecodeColor((int)encodedColor);

        currentDirections = (RoomDirections)UnityEngine.Random.Range((int)1, (int)15);
        

        //Debug.Log(Convert.ToString((int)currentDirections, 2).PadLeft(4, '0'));

        //TODO Clean this piece up with a switch or something? Idk.
        if(currentDirections.HasFlag(RoomDirections.North))
        {
            directionDoors[0].SetActive(true);
        }
        if (currentDirections.HasFlag(RoomDirections.East))
        {
            directionDoors[1].SetActive(true);
        }
        if (currentDirections.HasFlag(RoomDirections.South))
        {
            directionDoors[2].SetActive(true);
        }
        if (currentDirections.HasFlag(RoomDirections.West))
        {
            directionDoors[3].SetActive(true);
        }



    }
}

