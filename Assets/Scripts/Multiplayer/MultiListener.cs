using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System;
using System.Text;
using System.Globalization;

public class MultiListener : MonoBehaviour
{
    public GameObject player;
    public GameObject anotherPlayer;
    private StreamWriter writer;
    private NetworkStream stream;
    private string id;
    private string respawnTag = "Respawn";
    private string DELIMETER = "|";

    //private string ip = "52.15.155.25";
    private string ip = "localhost";
    private int port = 16000;
    
    void Start()
    {
        print("Connection");
        TcpClient client = new TcpClient(ip, port);
        stream = client.GetStream();
        stream.ReadTimeout = 5;
        stream.WriteTimeout = 3;
        
        if (stream.CanRead)
        {
            writer = new StreamWriter(stream);
            print("Writer created");
            readData();
        }
    }

    private void send(string json)
    {
        writer.Write(json + DELIMETER);
        writer.Flush();
    }

    void OnApplicationQuit()
    {
        Vector3 playerPos = player.transform.position;
        Quaternion playerRot = player.transform.rotation;
        Position pos = new Position(playerPos.x.ToString(), playerPos.y.ToString(), playerPos.z.ToString());
        Rotation rot = new Rotation(playerRot.x.ToString(), playerRot.y.ToString(), playerRot.z.ToString(), playerRot.w.ToString());

        string action = GetClientActionName(ClientAction.REMOVE);
        PlayerRequest request = new PlayerRequest(id, pos, rot, action);

        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void handleEvent(Vector3 position, Quaternion rotation, ClientAction action, Vector3 target)
    {
        handleEventWithShootParam(position, rotation, action, target);
    }

    public void handleEvent(Vector3 position, Quaternion rotation, ClientAction action)
    {
        handleEventWithShootParam(position, rotation, action, Vector3.zero);
    }

    private void handleEventWithShootParam(Vector3 position, Quaternion rotation, ClientAction action, Vector3 target)
    {
        Position pos = new Position(position.x.ToString(), position.y.ToString(), position.z.ToString());
        Position tarPosition = new Position(target.x.ToString(), target.y.ToString(), target.z.ToString());
        Rotation rot = new Rotation(rotation.x.ToString(), rotation.y.ToString(), rotation.z.ToString(), rotation.w.ToString());

        string actionStr = GetClientActionName(action);
        PlayerRequest request = new PlayerRequest(id, pos, rot, actionStr, tarPosition);

        string json = JsonUtility.ToJson(request);
        send(json);
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
                    byte[] buff = new byte[1024];
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

    void removePlayer(string id)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.removeClient(id);
    }

    void createPlayer()
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        Instantiate(player, resp.transform.position, resp.transform.rotation);
    }

    void createNewClient(string id, Vector3 pos, Quaternion rot)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.addClient(id, pos, rot, anotherPlayer);
    }

    void moveClient(string id, Vector3 pos, Quaternion rot)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.moveClient(id, pos, rot);
    }

    public string GetClientActionName(ClientAction value)
    {
        return Enum.GetName(typeof(ClientAction), value);
    }

    void anotherPlayerShoot(string id, Position target)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.shoot(id, target);
    }

    void parseData(string data)
    {
        PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(data);
        string action = response.GetAction();
        Debug.Log("action response" + response.GetAction());

        Single pX = CoordinatsUtil.parseCoordinations(response.GetPosition().GetX());
        Single pY = CoordinatsUtil.parseCoordinations(response.GetPosition().GetY());
        Single pZ = CoordinatsUtil.parseCoordinations(response.GetPosition().GetZ());
        Vector3 position = new Vector3(pX, pY, pZ);

        Single rX = CoordinatsUtil.parseCoordinations(response.GetRotation().GetX());
        Single rY = CoordinatsUtil.parseCoordinations(response.GetRotation().GetY());
        Single rZ = CoordinatsUtil.parseCoordinations(response.GetRotation().GetZ());
        Single rW = CoordinatsUtil.parseCoordinations(response.GetRotation().GetW());
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
            case "REMOVE":
                removePlayer(response.GetId());
                break;
            case "SHOOT":
                anotherPlayerShoot(response.GetId(), response.GetTarget());
                break;
        }
    }
}
