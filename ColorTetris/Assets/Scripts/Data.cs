using UnityEngine;
using System.Collections.Generic;

public static class Data
{
    public static readonly Dictionary<Rotation,Vector2Int[]> cells = new Dictionary<Rotation, Vector2Int[]>()
    {
        {Rotation.Horizontal1,new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        {Rotation.Vertical1,new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0) } },
        {Rotation.Horizontal2,new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 0) } },
        {Rotation.Vertical2,new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) } },
    };

    public static readonly Dictionary<Rotation, Matrix4x4[]> rotations = new Dictionary<Rotation, Matrix4x4[]>()
    {
        {Rotation.Horizontal1,new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one) } },
        {Rotation.Vertical1,new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one) } },
        {Rotation.Horizontal2,new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one) } },
        {Rotation.Vertical2,new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one) } },
    };

    public static readonly Vector2Int[] horizontalToVertical = new Vector2Int[]
    {
        new Vector2Int(0,0), new Vector2Int(0,-1), new Vector2Int(1,1),new Vector2Int(1,-1)
    };

    public static readonly Vector2Int[] verticalToHorizontal = new Vector2Int[]
    {
        new Vector2Int(0,0), new Vector2Int(-1,0)
    };
}
