using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System;
using System.Text;
using System.Globalization;

public class MultiListener : MonoBehaviour
{
    private static GameObject hostClient;

    public GameObject player;
    public GameObject anotherPlayer;
    private StreamWriter writer;
    private NetworkStream stream;
    private string id;
    public static string respawnTag = "Respawn";
    private string DELIMETER = "|";

    //private const string ip = "52.15.155.25";
    private const string ip = "localhost";
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
        PlayerDefaultDto request = new PlayerDefaultDto(Id, pos, rot, action);

        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void handleEvent(Vector3 position, Quaternion rotation, ClientAction action)
    {
        Position pos = CoordinatsUtil.vectorToPosition(position);
        Rotation rot = CoordinatsUtil.quaternionToRotation(rotation);

        string actionStr = GetClientActionName(action);

        PlayerDefaultDto request = new PlayerDefaultDto(this.Id, pos, rot, actionStr);
        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void shoot(ClientAction action, Vector3 target)
    {
        Position tarPosition = CoordinatsUtil.vectorToPosition(target);
        string actionStr = GetClientActionName(action);

        ShootDataDto shoot = new ShootDataDto(this.Id, actionStr, tarPosition);
        string json = JsonUtility.ToJson(shoot);
        send(json);
    }

    public void hitPlayer(ClientAction action, string targetId, Vector3 target)
    {
        Position tarPosition = CoordinatsUtil.vectorToPosition(target);

        string actionStr = GetClientActionName(action);

        ShootDataDto shoot = new ShootDataDto(this.Id, actionStr, targetId, tarPosition);
        string json = JsonUtility.ToJson(shoot);
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
                //Debug.LogWarning(ex.Message);
            }
        }
    }

    void removePlayer(PlayerDefaultDto data)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.removeClient(data.Id);
    }

    void createPlayer(PlayerDefaultDto data)
    {
        this.Id = data.Id;

        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        hostClient = Instantiate(player, resp.transform.position, resp.transform.rotation);
        StatusPlayer status = hostClient.GetComponent<StatusPlayer>();
        status.Id = this.Id;
        status.IsClient = true;
    }

    void createNewClient(PlayerDefaultDto data)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.addClient(data.Id, data.positionToVector3(), data.rotationToQuaternion(), anotherPlayer);
    }

    void moveClient(PlayerDefaultDto data)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.moveClient(data.Id, data.positionToVector3(), data.rotationToQuaternion());
    }

    public string GetClientActionName(ClientAction value)
    {
        return Enum.GetName(typeof(ClientAction), value);
    }

    void anotherPlayerShoot(ShootDataDto dto)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        resp.shoot(dto.Id, dto.Target);
    }


    void hitPlayer(ShootDataDto dto)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        //TODO Change default one damage to damage from response
        if (dto.Action.Equals(ClientAction.HIT.ToString()))
        {
            if (dto.TargetId.Equals(this.Id))
            {
                StatusPlayer thisPlayer = hostClient.GetComponent<StatusPlayer>();
                thisPlayer.hpPlayerDamage(1);
            }
            else
            {
                resp.hit(dto.TargetId, 1);
            }
        }
        else
        {
            if (dto.TargetId.Equals(this.Id))
            {
                StatusPlayer thisPlayer = hostClient.GetComponent<StatusPlayer>();
                thisPlayer.HpPlayer = 0;
            }
            else
            {
                resp.removeClient(dto.TargetId);
            }
        }
    }

    void killPlayer(ShootDataDto dto)
    {
        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        if (dto.TargetId.Equals(this.Id))
        {
            StatusPlayer thisPlayer = hostClient.GetComponent<StatusPlayer>();
            thisPlayer.HpPlayer = 0;
        }
        else
        {
            resp.removeClient(dto.TargetId);
        }
    }

    void parseData(string data)
    {
        if (data.Contains(ClientAction.NEW_SESSION.ToString()))
        {
            PlayerDefaultDto parseData = PlayerDefaultDto.parse(data);
            createPlayer(parseData);
        } else if (data.Contains(ClientAction.NEW_CLIENT.ToString()))
        {
            PlayerDefaultDto newCLientData = PlayerDefaultDto.parse(data);
            createNewClient(newCLientData);
        } else if (data.Contains(ClientAction.MOVE.ToString()))
        {
            PlayerDefaultDto moveData = PlayerDefaultDto.parse(data);
            moveClient(moveData);
        } else if (data.Contains(ClientAction.REMOVE.ToString()))
        {
            PlayerDefaultDto removeData = PlayerDefaultDto.parse(data);
            removePlayer(removeData);
        } else if (data.Contains(ClientAction.SHOOT.ToString()))
        {
            ShootDataDto shoot = ShootDataDto.parse(data);
            anotherPlayerShoot(shoot);
        } else if (data.Contains(ClientAction.HIT.ToString()))
        {
            ShootDataDto hit = ShootDataDto.parse(data);
            hitPlayer(hit);
        } else if (data.Contains(ClientAction.KILL_CLIENT.ToString()))
        {
            ShootDataDto kill = ShootDataDto.parse(data);
            killPlayer(kill);
        }
    }
}
