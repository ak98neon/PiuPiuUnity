using System;
using UnityEngine;

[Serializable]
public class PlayerDefaultDto
{
    [SerializeField]
    private string id;
    [SerializeField]
    private Position position;
    [SerializeField]
    private Rotation rotation;
    [SerializeField]
    private string action;

    public string Id { get => id; set => id = value; }
    public Position Position { get => position; set => position = value; }
    public Rotation Rotation { get => rotation; set => rotation = value; }
    public string Action { get => action; set => action = value; }

    public PlayerDefaultDto()
    {
    }

    public PlayerDefaultDto(string id, Position position, Rotation rotation, string action)
    {
        this.Id = id;
        this.Position = position;
        this.Rotation = rotation;
        this.Action = action;
    }

    public PlayerDefaultDto(string id, Position position, string action)
    {
        this.Id = id;
        this.Position = position;
        this.Action = action;
    }

    public PlayerDefaultDto(Position position, Rotation rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }

    public static PlayerDefaultDto parse(string data)
    {
        PlayerDefaultDto response = JsonUtility.FromJson<PlayerDefaultDto>(data);
        Debug.Log("action response" + response.Action);
        return response;
    }

    public Vector3 positionToVector3()
    {
        Single pX = CoordinatsUtil.parseCoordinations(this.Position.GetX());
        Single pY = CoordinatsUtil.parseCoordinations(this.Position.GetY());
        Single pZ = CoordinatsUtil.parseCoordinations(this.Position.GetZ());
        return new Vector3(pX, pY, pZ);
    }

    public Quaternion rotationToQuaternion()
    {
        Single rX = CoordinatsUtil.parseCoordinations(this.Rotation.GetX());
        Single rY = CoordinatsUtil.parseCoordinations(this.Rotation.GetY());
        Single rZ = CoordinatsUtil.parseCoordinations(this.Rotation.GetZ());
        Single rW = CoordinatsUtil.parseCoordinations(this.Rotation.GetW());
        return new Quaternion(rX, rY, rZ, rW);
    }
}
