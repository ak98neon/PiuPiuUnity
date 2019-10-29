using System;
using UnityEngine;

[Serializable]
public class PlayerRequest
{
    [SerializeField]
    private string id;
    [SerializeField]
    private Position position;
    [SerializeField]
    private Rotation rotation;
    [SerializeField]
    private string action;
    [SerializeField]
    private Position target;

    public PlayerRequest()
    {

    }

    public PlayerRequest(string id, Position position, Rotation rotation, string action)
    {
        this.id = id;
        this.position = position;
        this.rotation = rotation;
        this.action = action;
    }

    public PlayerRequest(string id, Position position, Rotation rotation, string action, Position target)
    {
        this.id = id;
        this.position = position;
        this.rotation = rotation;
        this.action = action;
        this.target = target;
    }

    public PlayerRequest(string id, Position position, string action)
    {
        this.id = id;
        this.position = position;
        this.action = action;
    }

    public PlayerRequest(Position position, Rotation rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public void SetId(string id)
    {
        this.id = id;
    }
}
