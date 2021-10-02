using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point p)
    {
        return (x == p.x && y == p.y);
    }

    public void Multiply(int m)
    {
        x *= m;
        y *= m;
    }

    public void Add(Point addingPoint)
    {
        x += addingPoint.x;
        y += addingPoint.y;
    }


    public static Point FromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point FromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point Multiply(Point point, int m)
    {
        return new Point(point.x * m, point.y * m);
    }

    public static Point Add(Point originPoint, Point addingPoint)
    {
        return new Point(originPoint.x + addingPoint.x, originPoint.y + addingPoint.y);
    }

    public static Point Clone(Point p)
    {
        return new Point(p.x, p.y);
    }

    public static Point Zero
    {
        get { return new Point(0, 0); }
    }

    public static Point One
    {
        get { return new Point(1, 1); }
    }

    public static Point Up
    {
        get { return new Point(0, 1); }
    }

    public static Point Down
    {
        get { return new Point(0, -1); }
    }

    public static Point Right
    {
        get { return new Point(1, 0); }
    }

    public static Point Left
    {
        get { return new Point(-1, 0); }
    }
}
