using System;
using UnityEngine;

[Serializable]
public class PlayerDataDto
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
    private string targetId;
    [SerializeField]
    private Position target;

    public string Id { get => id; set => id = value; }
    public Position Position { get => position; set => position = value; }
    public Rotation Rotation { get => rotation; set => rotation = value; }
    public string Action { get => action; set => action = value; }
    public Position Target { get => target; set => target = value; }
    public string TargetId { get => targetId; set => targetId = value; }

    public PlayerDataDto()
    {

    }

    public PlayerDataDto(string id, Position position, Rotation rotation, string action)
    {
        this.Id = id;
        this.Position = position;
        this.Rotation = rotation;
        this.Action = action;
    }

    public PlayerDataDto(string id, Position position, Rotation rotation, string action, string targetId, Position target) : this(id, position, rotation, action)
    {
        this.targetId = targetId;
        this.target = target;
    }

    public PlayerDataDto(string id, Position position, Rotation rotation, string action, Position target)
    {
        this.Id = id;
        this.Position = position;
        this.Rotation = rotation;
        this.Action = action;
        this.Target = target;
    }

    public PlayerDataDto(string id, Position position, string action)
    {
        this.Id = id;
        this.Position = position;
        this.Action = action;
    }

    public PlayerDataDto(Position position, Rotation rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }
}
