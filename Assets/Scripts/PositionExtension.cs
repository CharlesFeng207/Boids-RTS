using UnityEngine;

public static class PositionExtension
{
    public static Vector2 ToLogicPos(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 ToWorldPos(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
}