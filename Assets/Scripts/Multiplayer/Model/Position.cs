using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Position
{
    [SerializeField]
    private string x;
    [SerializeField]
    private string y;
    [SerializeField]
    private string z;

    public Position()
    {

    }
    public Position(string x, string y, string z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public string GetX()
    {
        return this.x;
    }

    public string GetY()
    {
        return this.y;
    }

    public string GetZ()
    {
        return this.z;
    }
}
