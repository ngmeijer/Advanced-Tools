using UnityEditor;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public string Type;
    public Vector3 Size;
    public Vector3 Center;
    public Color GizmoColor;

    public Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-Size.x / 2, Size.x / 2), Center.y, Random.Range(-Size.z / 2, Size.z / 2));
    }

    private void OnDrawGizmos()
    {
        if (GizmoColor.a == 0)
            return;

        Gizmos.color = GizmoColor;
        Gizmos.DrawWireCube(transform.position + Center, Size);
        Handles.Label(transform.position + new Vector3(Size.x / 2, 0,0) + Center, Type);
    }
}
