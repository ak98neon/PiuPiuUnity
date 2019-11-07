using System;
using UnityEngine;

[Serializable]
public class ShootDataDto
{
    [SerializeField]
    private string id;
    [SerializeField]
    private string action;
    [SerializeField]
    private string targetId;
    [SerializeField]
    private Position target;

    public string Id { get => id; set => id = value; }
    public string Action { get => action; set => action = value; }
    public Position Target { get => target; set => target = value; }
    public string TargetId { get => targetId; set => targetId = value; }

    public ShootDataDto()
    {
    }

    public ShootDataDto(string id, string action, string targetId, Position target)
    {
        this.id = id;
        this.action = action;
        this.targetId = targetId;
        this.target = target;
    }

    public ShootDataDto(string id, string action, Position target)
    {
        this.id = id;
        this.action = action;
        this.target = target;
    }

    public static ShootDataDto parse(string data)
    {
        ShootDataDto response = JsonUtility.FromJson<ShootDataDto>(data);
        Debug.Log("Action response: " + response.Action);
        return response;
    }
}
