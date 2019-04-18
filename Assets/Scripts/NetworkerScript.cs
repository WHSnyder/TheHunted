using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;




public class NetworkerScript : NetworkManager
{

    public GameObject[] characters;
    private int count = 0;

    public override void OnServerConnect(NetworkConnection conn)
    {

        Debug.Log("A client connected to the server: " + conn);
        Debug.Log("\n# Connections: " + NetworkServer.connections.Count.ToString());
    }



    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {


        GameObject player;

        Vector3 startPos = GameObject.Find("Spawn").transform.position;


        if (NetworkServer.connections.Count == 1)
        {

            player = Instantiate(characters[0], startPos, Quaternion.identity) as GameObject;
        }
        else
        {
            player = Instantiate(characters[1], startPos, Quaternion.identity) as GameObject;

        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }



    public override void OnClientConnect(NetworkConnection conn)
    {

        // Create message to set the player
        //IntegerMessage msg = new IntegerMessage(0);

        Debug.Log("Client joining...");

        // Call Add player and pass the message
        ClientScene.AddPlayer(conn, 0);


    }


}


