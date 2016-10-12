using UnityEngine;
using System.Collections;

public struct Pos {

    public int x;
    public int y;
    public int z;

    public Pos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
        z = 0;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ")";
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            hash = hash * 227 + z.GetHashCode();
            return hash * 227;
        }
    }

    public override bool Equals(object obj)
    {
        // If parameter cannot be cast to pos return false.
        Pos? p = obj as Pos?;
        if (!p.HasValue) return false;

        // Return true if the fields match:
        return (x == p.Value.x) && (y == p.Value.y) && (z == p.Value.z);
    }

    public static bool operator ==(Pos a, Pos b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Pos a, Pos b)
    {
        return !a.Equals(b);
    }

    public static Pos operator -(Pos a, Pos b)
    {
        return new Pos(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Pos operator +(Pos a, Pos b)
    {
        return new Pos(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Pos operator *(int i, Pos p)
    {
        return new Pos(p.x * i, p.y * i, p.z * i);
    }

    public static Pos operator *(Pos p, int i)
    {
        return i * p;
    }

    public static Pos operator /(int i, Pos p)
    {
        return new Pos(p.x / i, p.y / i, p.z / i);
    }

    public static implicit operator Vector3(Pos p)
    {
        return new Vector3(p.x, p.y, p.z);
    }

    public static implicit operator Pos(Vector3 v)
    {
        return new Pos(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }

    public static implicit operator Vector2(Pos p)
    {
        return new Vector2(p.x, p.y);
    }

    public static implicit operator Pos(Vector2 v)
    {
        return new Pos(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    public static implicit operator Pos(Direction dir)
    {
        switch (dir)
        {
            case Direction.north:
                return new Pos(0, 0, 1);
            case Direction.east:
                return new Pos(1, 0, 0);
            case Direction.south:
                return new Pos(0, 0, -1);
            case Direction.west:
                return new Pos(-1, 0, 0);
            case Direction.up:
                return new Pos(0, 1, 0);
            case Direction.down:
                return new Pos(0, -1, 0);
            default:
                return new Pos(0, 0, 1);
        }
    }

    public static int Distance(Pos a, Pos b)
    {

        var x = a.x - b.x;
        var y = a.y - b.y;
        var z = a.z - b.z;

        if (x < 0)
            x *= -1;

        if (y < 0)
            y *= -1;

        if (z < 0)
            z *= -1;

        return x + y + z;
    }

    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                default:
                    return 0;
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    return;
            }
        }
    }

}
