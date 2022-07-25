using System;
using System.Collections.Generic;
using UnityEngine;

public class NullableVector3
{
    /// <summary>
    /// The implicit coercion operator, to assign Vector3 to NullableVector3.
    /// </summary>
    public static implicit operator NullableVector3(Vector3 v) => new NullableVector3(v);

    /// <summary>
    /// The x component of the vector.
    /// </summary>
    private float _x;

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    private float _y;

    /// <summary>
    /// The z component of the vector.
    /// </summary>
    private float _z;

    /// <summary>
    /// This property returns the x component of the vector.
    /// </summary>
    public float x
    {
        get
        {
            return _x;
        }
    }

    /// <summary>
    /// This property returns the y component of the vector.
    /// </summary>
    public float y
    {
        get
        {
            return _y;
        }
    }

    /// <summary>
    /// This property returns the z component of the vector.
    /// </summary>
    public float z
    {
        get
        {
            return _z;
        }
    }
    
    /// <summary>
    /// Returns the Vector3 corresponding to this nullable vector;
    /// </summary>
    public Vector3 ToVector3
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public NullableVector3(float x, float y, float z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public NullableVector3(Vector3 vector3)
    {
        _x = vector3.x;
        _y = vector3.y;
        _z = vector3.z;
    }
}
