using System;

public struct GridPosition : IEquatable<GridPosition>
{
    #region //Variables
    public int x;
    public int z;
    #endregion


    #region //Constructors
    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    #endregion

    #region //Getter and Equality overrides
    public override string ToString()
    {
        return $"{x}, {z}";
    }

    public override bool Equals(object obj)
    {
        if(!(obj is GridPosition position)) return false;
        if(x != position.x) return false;
        return z == position.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }
    #endregion
}