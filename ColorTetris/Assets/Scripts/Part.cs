using UnityEngine;
using UnityEngine.Tilemaps;

public enum Pill
{
    RR,
    RB,
    RL,
    BB,
    BL,
    LL,
}

public enum SingleColor
{
    R,
    B,
    L,
}
public enum Rotation
{
    Horizontal1,
    Vertical1,
    Horizontal2,
    Vertical2,
}

[System.Serializable]
public struct PillData
{
    public Pill pill;
    public Tile firstHalf;
    public Tile secondHalf;
    public Vector2Int[] cells { get; private set; }
    public Matrix4x4[] rots { get; private set; }

    public void Initialize()
    {
        this.cells = Data.cells[0];
        this.rots = Data.rotations[0];
    }
}

[System.Serializable]
public struct VirusData
{
    public SingleColor virus;
    public Tile virusTile;
    public Vector2Int cells { get; private set; }

    public void Initialize()
    {
        this.cells = new Vector2Int(0,0);
    }
}

[System.Serializable]
public struct EmptyData
{
    public SingleColor empty;
    public Tile emptyTile;
    public Vector2Int cells { get; private set; }

    public void Initialize()
    {
        this.cells = new Vector2Int(0, 0);
    }
}

[System.Serializable]
public struct SinglePillData
{
    public SingleColor singlePill;
    public Tile singlePillTile;
    public Vector2Int cells { get; private set; }

    public void Initialize()
    {
        this.cells = new Vector2Int(0, 0);
    }
}