using System;

/// <summary>
/// Locations on the game's grid
/// </summary>
public struct GridCell : IEquatable<GridCell>
{
    #region //Variables
    public int x;
    public int z;
    #endregion


    #region //Constructors
    public GridCell(int x, int z)
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
        if(!(obj is GridCell cell)) return false;
        if(x != cell.x) return false;
        return z == cell.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static bool operator ==(GridCell a, GridCell b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridCell a, GridCell b)
    {
        return !(a == b);
    }

    public static GridCell operator +(GridCell a, GridCell b)
    {
        return new GridCell(a.x + b.x, a.z + b.z);
    }

    public static GridCell operator -(GridCell a, GridCell b)
    {
        return new GridCell(a.x - b.x, a.z - b.z);
    }


    public bool Equals(GridCell other)
    {
        return this == other;
    }
    #endregion
}