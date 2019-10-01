﻿using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System;
using System.Text;

public class MultiListener : MonoBehaviour
{
    public GameObject player;
    private StreamWriter writer;
    private NetworkStream stream;
    private string id;
    private string respawnTag = "Respawn";
    
    void Start()
    {
        print("Connection");
        TcpClient client = new TcpClient("127.0.0.1", 16000);
        stream = client.GetStream();
        stream.ReadTimeout = 10;
        
        if (stream.CanRead)
        {
            writer = new StreamWriter(stream);
            print("Writer created");
            readData();
        }
    }

    public void handleEvent(Vector3 position, Quaternion rotation)
    {
        print(this.id);
        Position pos = new Position(position.x.ToString(), position.y.ToString(), position.z.ToString());
        Rotation rot = new Rotation(rotation.x.ToString(), rotation.y.ToString(), rotation.z.ToString(), rotation.w.ToString());
        PlayerRequest request = new PlayerRequest(id, pos, rot, "MOVE");

        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void sendMove(Vector3 move, bool crouch, bool jump)
    {
        Position pos = new Position(move.x.ToString(), move.y.ToString(), move.z.ToString());
        PlayerRequest request = new PlayerRequest(id, pos, "MoveChar");

        //json.AddField("crouch", crouch);
        //json.AddField("jump", jump);
        string json = JsonUtility.ToJson(request);
        send(json);
    }

    private void send(string json)
    {
        writer.Write(json);
        writer.Flush();
    }

    void Update()
    {
        readData();
    }

    void readData()
    {
        if (stream.CanRead)
        {
            try
            {
                byte[] bLen = new byte[4];
                int data = stream.Read(bLen, 0, 4);
                if (data > 0)
                {
                    int len = BitConverter.ToInt32(bLen, 0);
                    print("len = " + len);
                    byte[] buff = new byte[8096];
                    data = stream.Read(buff, 0, len);
                    if (data > 0)
                    {
                        string result = Encoding.ASCII.GetString(buff, 0, data);
                        Debug.Log("result: " + result);
                        stream.Flush();
                        Debug.Log(result);
                        parseData(result);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    void parseData(string data)
    {
        PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(data);
        string action = response.GetAction();
        Debug.Log("action response" + response.GetAction());

        Single pX = Convert.ToSingle(response.GetPosition().GetX());
        Single pY = Convert.ToSingle(response.GetPosition().GetY());
        Single pZ = Convert.ToSingle(response.GetPosition().GetZ());
        Vector3 position = new Vector3(pX, pY, pZ);
        
        Single rX = Convert.ToSingle(response.GetRotation().GetX());
        Single rY = Convert.ToSingle(response.GetRotation().GetY());
        Single rZ = Convert.ToSingle(response.GetRotation().GetZ());
        Single rW = Convert.ToSingle(response.GetRotation().GetW());
        Quaternion rotation = new Quaternion(rX, rY, rZ, rW);

        switch (action)
        {
            case "NEW_SESSION":
                this.id = response.GetId();
                createPlayer();
                break;
            case "NEW_CLIENT":
                createNewClient(response.GetId(), position, rotation);
                break;
            case "MOVE":
                moveClient(response.GetId(), position, rotation);
                break;
        }

    }

    void createPlayer()
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        Instantiate(player, resp.transform.position, resp.transform.rotation);
    }

    void createNewClient(string id, Vector3 pos, Quaternion rot)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.addClient(id, pos, rot, player);
    }

    void moveClient(string id, Vector3 pos, Quaternion rot)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.moveClient(id, pos, rot);
    }
}
