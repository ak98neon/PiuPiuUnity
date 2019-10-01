using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerResponse
{
    [SerializeField]
    private string id;
    [SerializeField]
    private Position position;
    [SerializeField]
    private Rotation rotation;
    [SerializeField]
    private string action;

    public PlayerResponse()
    {

    }

    public PlayerResponse(string id, Position position, Rotation rotation, string action)
    {
        this.id = id;
        this.position = position;
        this.rotation = rotation;
        this.action = action;
    }

    public PlayerResponse(Position position, Rotation rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public string GetId()
    {
        return this.id;
    }

    public Position GetPosition()
    {
        return this.position;
    }

    public Rotation GetRotation()
    {
        return this.rotation;
    }

    public string GetAction()
    {
        return this.action;
    }
}
