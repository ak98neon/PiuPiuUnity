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

    public string Id { get => id; set => id = value; }

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
        PlayerDataDto request = new PlayerDataDto(Id, pos, rot, action);

        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void handleHitAnotherPlayer(Vector3 position, Quaternion rotation, ClientAction action, string targetId, Vector3 target)
    {
        handleEventWithShootParam(position, rotation, action, targetId, target);
    }

    public void handleEvent(Vector3 position, Quaternion rotation, ClientAction action, Vector3 target)
    {
        handleEventWithShootParam(position, rotation, action, null, target);
    }

    public void handleEvent(Vector3 position, Quaternion rotation, ClientAction action)
    {
        handleEventWithShootParam(position, rotation, action, null, Vector3.zero);
    }

    private void handleEventWithShootParam(Vector3 position, Quaternion rotation, ClientAction action, string targetId, Vector3 target)
    {
        Position pos = new Position(position.x.ToString(), position.y.ToString(), position.z.ToString());
        Position tarPosition = new Position(target.x.ToString(), target.y.ToString(), target.z.ToString());
        Rotation rot = new Rotation(rotation.x.ToString(), rotation.y.ToString(), rotation.z.ToString(), rotation.w.ToString());

        string actionStr = GetClientActionName(action);
        PlayerDataDto request = null;
        if (targetId != null) {
            request = new PlayerDataDto(Id, pos, rot, actionStr, targetId, tarPosition);
        } else
        {
            request = new PlayerDataDto(Id, pos, rot, actionStr, tarPosition);
        }

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

    void createPlayer(string id)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        Instantiate(player, resp.transform.position, resp.transform.rotation);
        StatusPlayer status = GetComponent<StatusPlayer>();
        status.Id = id;
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


    void hitPlayer(string id)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        //Change default one damage to damage from response
        resp.hit(id, 1);
    }

    void parseData(string data)
    {
        PlayerDataDto response = JsonUtility.FromJson<PlayerDataDto>(data);
        string action = response.Action;
        Debug.Log("action response" + response.Action);

        Single pX = CoordinatsUtil.parseCoordinations(response.Position.GetX());
        Single pY = CoordinatsUtil.parseCoordinations(response.Position.GetY());
        Single pZ = CoordinatsUtil.parseCoordinations(response.Position.GetZ());
        Vector3 position = new Vector3(pX, pY, pZ);

        Single rX = CoordinatsUtil.parseCoordinations(response.Rotation.GetX());
        Single rY = CoordinatsUtil.parseCoordinations(response.Rotation.GetY());
        Single rZ = CoordinatsUtil.parseCoordinations(response.Rotation.GetZ());
        Single rW = CoordinatsUtil.parseCoordinations(response.Rotation.GetW());
        Quaternion rotation = new Quaternion(rX, rY, rZ, rW);

        switch (action)
        {
            case "NEW_SESSION":
                this.Id = response.Id;
                createPlayer(this.id);
                break;
            case "NEW_CLIENT":
                createNewClient(response.Id, position, rotation);
                break;
            case "MOVE":
                moveClient(response.Id, position, rotation);
                break;
            case "REMOVE":
                removePlayer(response.Id);
                break;
            case "SHOOT":
                anotherPlayerShoot(response.Id, response.Target);
                break;
            case "HIT":
                hitPlayer(response.Id);
                break;
        }
    }
}
