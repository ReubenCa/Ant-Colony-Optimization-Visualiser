using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    private int X1;
    [SerializeField]
    private int Y1;
    [SerializeField]
    private int X2;
    [SerializeField]
    private int Y2;

    public void Start()
    {
        SimulationManager.instance.Walls.Add(this);
    }

    public void OnDestroy()
    {
        SimulationManager.instance.Walls.Remove(this);
    }   

    private void Update()
    {
        //Make the middle of two of the oppsite sides (X1, Y1) and (X2, Y2)
        Vector3 middle = new Vector3((X1 + X2) / 2, (Y1 + Y2) / 2, 0);
        //Make the length of the wall
        float length = Vector3.Distance(new Vector3(X1, Y1, 0), new Vector3(X2, Y2, 0));
        //Make the angle of the wall
        float angle = Mathf.Atan2(Y2 - Y1, X2 - X1) * Mathf.Rad2Deg;
        //Set the position, rotation and scale of the wall
        transform.position = middle;//new Vector3(X1,Y1);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = new Vector3(length, 1f, 1);
        //transform.localPosition = middle;

        Debug.DrawLine(new Vector3(X1, Y1), new Vector3(X2, Y2));
       

    }

    public bool Intersectswith(Vector3 pos1, Vector3 pos2)
    {
        Vector2 Point1 = new Vector2(X1, Y1);
        Vector2 Point2 = new Vector2(X2, Y2);

        Vector2 Point3 = new Vector2(pos1.x, pos1.y);
        Vector2 Point4 = new Vector2(pos2.x, pos2.y);

        // Check for intersection using vectors
        float denominator = ((Point4.y - Point3.y) * (Point2.x - Point1.x)) - ((Point4.x - Point3.x) * (Point2.y - Point1.y));
        if (denominator == 0)
        {
            // Lines are parallel, so they don't intersect
            return false;
        }

        float ua = (((Point4.x - Point3.x) * (Point1.y - Point3.y)) - ((Point4.y - Point3.y) * (Point1.x - Point3.x))) / denominator;
        float ub = (((Point2.x - Point1.x) * (Point1.y - Point3.y)) - ((Point2.y - Point1.y) * (Point1.x - Point3.x))) / denominator;

        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
        {
            // Intersection point lies on both segments
            return true;
        }

        // No intersection
        return false;
    }


    private static Vector2 Minus (Vector2 a, Vector2 b)
    {
        return new Vector2(a.x - b.x, a.y - b.y);
    }
}
