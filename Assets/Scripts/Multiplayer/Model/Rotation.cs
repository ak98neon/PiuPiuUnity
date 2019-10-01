using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rotation
{
    [SerializeField]
    private string x;
    [SerializeField]
    private string y;
    [SerializeField]
    private string z;
    [SerializeField]
    private string w;

    public Rotation()
    {

    }
    public Rotation(string x, string y, string z, string w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
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

    public string GetW()
    {
        return this.w;
    }
}
