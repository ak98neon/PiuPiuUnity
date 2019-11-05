using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatsUtil : MonoBehaviour
{
    public static float parseCoordinations(string param)
    {
        return Convert.ToSingle(param);
    }

    public static Position vectorToPosition(Vector3 position)
    {
        return new Position(position.x.ToString(), position.y.ToString(), position.z.ToString());
    }

    public static Rotation quaternionToRotation(Quaternion rotation)
    {
        return new Rotation(rotation.x.ToString(), rotation.y.ToString(), rotation.z.ToString(), rotation.w.ToString());
    }
}
