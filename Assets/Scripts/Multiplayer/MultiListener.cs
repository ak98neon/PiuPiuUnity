using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Collections.Concurrent;

public class MultiListener : MonoBehaviour
{
    public GameObject player;
    public GameObject anotherPlayer;
    private string id;
    private string respawnTag = "Respawn";
    private int receivePort = 16000;
    private int remotePort = 9092;
    private string remoteAddress = "127.0.0.1";

    private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

    void Start()
    {
        Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
        receiveThread.Start();

        Respawn resp = GameObject.FindGameObjectWithTag(respawnTag).GetComponent<Respawn>();
        Position pos = new Position(resp.transform.position.x.ToString(), resp.transform.position.y.ToString(), resp.transform.position.z.ToString());
        Rotation rot = new Rotation(resp.transform.rotation.x.ToString(), resp.transform.rotation.y.ToString(), resp.transform.rotation.z.ToString(), resp.transform.rotation.w.ToString());
        PlayerRequest request = new PlayerRequest(null, pos, rot, "NEW_SESSION");
        string json = JsonUtility.ToJson(request);
        send(json);
    }

    void Update()
    {
        if (_queue.Count > 0)
        {
            string message = null;
            _queue.TryDequeue(out message);
            Debug.Log("Message from listener thread: " + message);

            parseData(message);
        }
    }

    private void send(string json)
    {
        UdpClient sender = new UdpClient();
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            sender.Send(data, data.Length, remoteAddress, remotePort);
            //Debug.Log("Send: " + json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            sender.Close();
        }
    }

    private void ReceiveMessage()
    {
        UdpClient receiver = new UdpClient(receivePort);

        IPEndPoint remoteIp = null;

        try
        {
            while(true)
            {
                byte[] data = receiver.Receive(ref remoteIp);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("Client: {0}" + message);
                _queue.Enqueue(message);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            receiver.Close();
        }
    }

    void OnApplicationQuit()
    {
        Vector3 playerPos = player.transform.position;
        Quaternion playerRot = player.transform.rotation;
        Position pos = new Position(playerPos.x.ToString(), playerPos.y.ToString(), playerPos.z.ToString());
        Rotation rot = new Rotation(playerRot.x.ToString(), playerRot.y.ToString(), playerRot.z.ToString(), playerRot.w.ToString());
        PlayerRequest request = new PlayerRequest(id, pos, rot, "REMOVE");

        string json = JsonUtility.ToJson(request);
        send(json);
    }

    public void handleEvent(Vector3 position, Quaternion rotation)
    {
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

        // json.AddField("crouch", crouch);
        // json.AddField("jump", jump);
        string json = JsonUtility.ToJson(request);
        send(json);
    }

    void parseData(string data)
    {
        PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(data);
        string action = response.GetAction();
        Debug.Log("action response: " + response.GetAction());

        Single pX = parseCoordinations(response.GetPosition().GetX());
        Single pY = parseCoordinations(response.GetPosition().GetY());
        Single pZ = parseCoordinations(response.GetPosition().GetZ());
        Vector3 position = new Vector3(pX, pY, pZ);
        
        Single rX = parseCoordinations(response.GetRotation().GetX());
        Single rY = parseCoordinations(response.GetRotation().GetY());
        Single rZ = parseCoordinations(response.GetRotation().GetZ());
        Single rW = parseCoordinations(response.GetRotation().GetW());
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
        }

    }

    float parseCoordinations(string param)
    {
        return Convert.ToSingle(param);
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
}
