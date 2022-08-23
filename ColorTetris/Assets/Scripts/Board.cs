using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public PillData[] pills;
    public VirusData[] viruses;
    public EmptyData[] empties;
    public SinglePillData[] singles;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(8, 16);
    [SerializeField] private int virusLevel;
    public Vector2Int virusSpawnSize = new Vector2Int(8, 13);
    private Queue<Vector3Int> DeleteQue = new Queue<Vector3Int>();
    bool waitForLineClears = true;
    private bool hasEmptyTiles = false;
    private bool deleteMethodCalled = false;
    public int virusCount = 0;
    public bool gameover = false;
    PillData nextPillData;
    public InGameUI inGameUI;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    public RectInt VirusBounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.virusSpawnSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.viruses.Length; i++)
        {
            this.viruses[i].Initialize();
        }

        for (int i = 0; i < this.pills.Length; i++)
        {
            this.pills[i].Initialize();
        }

        this.nextPillData = GetNextPill();
    }

    private void Start()
    {
        virusLevel = LoadGameValues.virusLevel;
        switch (LoadGameValues.Dificulty)
        {
            case LoadGameValues.Dificulties.Slow: 
                this.activePiece.stepDelay = 1f;
                break;
            case LoadGameValues.Dificulties.Medium:
                this.activePiece.stepDelay = 0.75f;
                break;
            case LoadGameValues.Dificulties.Fast:
                this.activePiece.stepDelay = 0.5f;
                break;
        }

        SpawnVirusesOnBoard();
        SetNextPiece();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        IsGameOver();
        if (!gameover)
        {
            PillData currentData = nextPillData;
            this.activePiece.Initialize(this, this.spawnPosition, currentData);
            Set(this.activePiece);
            nextPillData = GetNextPill();
            SetNextPiece();
        }
    }

    private void SetNextPiece()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3Int tilePosition = new Vector3Int(spawnPosition.x + 8 + i, spawnPosition.y + 1, spawnPosition.z);
            if (i == 0)
            {
                this.tilemap.SetTile(tilePosition, nextPillData.firstHalf);
                this.tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one));
            }
            else
            {
                this.tilemap.SetTile(tilePosition, nextPillData.secondHalf);
                this.tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one));
            }
        }
    }

    private PillData GetNextPill()
    {
        int randomPill = Random.Range(0, this.pills.Length);
        PillData data = this.pills[randomPill];

        return data;
    }

    public void IsGameOver()
    {
        Vector3Int secondSpawnPos = new Vector3Int(spawnPosition.x + 1, spawnPosition.y, spawnPosition.z);
        if (this.tilemap.HasTile(spawnPosition) || this.tilemap.HasTile(secondSpawnPos))
        {
            gameover = true;
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.possition;           
            if (i == 0)
            {
                this.tilemap.SetTile(tilePosition, piece.data.firstHalf);
            }
            else
            {
                this.tilemap.SetTile(tilePosition, piece.data.secondHalf);
            }
            SetTileRotation(tilePosition, piece, i);
        }
    }

    public void SetTileRotation(Vector3Int position, Piece piece, int tileIncrement)
    {
        this.tilemap.SetTransformMatrix(position, piece.tileRotations[tileIncrement]);
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.possition;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    private void SpawnVirusesOnBoard()
    {
        int numberOfViruses = (this.virusLevel * 4) + 4;
        int emptyCellFillAmount = CalculateEmptyFillAmount(numberOfViruses);
        int diversitySpawnCount = 2;
        int rowsMaxCoord = VirusBounds.height - 8;
        int colsMaxCoord = VirusBounds.width / 2;

        while (numberOfViruses > 0)
        {
            for (int row = VirusBounds.position.y; row < rowsMaxCoord; row++)
            {
                for (int col = VirusBounds.position.x; col < colsMaxCoord; col++)
                {
                    int randomVirus = Random.Range(0, this.viruses.Length + emptyCellFillAmount);

                    if (randomVirus < this.viruses.Length)
                    {
                        Vector3Int virusTilePosition = new Vector3Int(col, row, 0);

                        if (numberOfViruses > 0)
                        {
                            if (diversitySpawnCount >= 0)
                            {
                                VirusData data = this.viruses[diversitySpawnCount];
                                this.tilemap.SetTile(virusTilePosition, data.virusTile);
                            }
                            else
                            {
                                VirusData data = this.viruses[randomVirus];
                                this.tilemap.SetTile(virusTilePosition, data.virusTile);
                            }
                            
                            diversitySpawnCount--;
                            numberOfViruses--; 
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            } 
        }
    }

    private int CalculateEmptyFillAmount(int virusLevel)
    {
        int fillAmount = (int)Mathf.Round(104 / virusLevel);
        return fillAmount;
    }

    public bool LineClears()
    {
        hasEmptyTiles = false;
        waitForLineClears = true;
        VerticalLineCheck();
        HorizontalLineCheck();

        if (hasEmptyTiles)
        {
            if (!deleteMethodCalled)
            {
                Invoke(nameof(DeleteMarkedCells), 2);
                deleteMethodCalled = true; 
            }
        }
        else
        {
            waitForLineClears = false;
            deleteMethodCalled = false;
        }
        
        return waitForLineClears;
    }

    public int VirusCounter()
    {
        RectInt bounds = this.Bounds;
        virusCount = 0;
        for (int col = bounds.xMin; col <= bounds.xMax; col++)
        {
            for (int row = bounds.yMin; row <= bounds.yMax; row++)
            {
                Vector3Int cellPosition = new Vector3Int(col, row, 0);
                if (this.tilemap.HasTile(cellPosition))
                {
                    string curCellName = this.tilemap.GetTile(cellPosition).name;
                    if (curCellName.Contains("Virus"))
                    {
                        virusCount++;
                    }
                }
            }
        }
        inGameUI.ScoreSetup(virusCount);
        return virusCount;
    }
    private void VerticalLineCheck()
    {
        RectInt bounds = this.Bounds;
        Queue<Vector3Int> verticalDelete = this.DeleteQue;
        string oldCellName = null;

        for (int col = bounds.xMin; col <= bounds.xMax; col++)
        {
            for (int row = bounds.yMin; row <= bounds.yMax; row++)
            {
                if (row == bounds.yMin)
                {
                    verticalDelete.Clear();
                }
                
                Vector3Int cellPosition = new Vector3Int(col, row, 0);
                if (this.tilemap.HasTile(cellPosition))
                {
                    string curCellName = this.tilemap.GetTile(cellPosition).name;
                    if (oldCellName == null)
                    {
                        oldCellName = curCellName;
                        verticalDelete.Enqueue(cellPosition);
                    }
                    else
                    {
                        if (oldCellName.Substring(0, 3) == curCellName.Substring(0, 3))
                        {
                            verticalDelete.Enqueue(cellPosition);
                        }
                        else
                        {
                            if (verticalDelete.Count >= 4)
                            {
                                SetEmptyTiles(oldCellName, verticalDelete);
                            }
                            
                            oldCellName = curCellName;
                            verticalDelete.Clear();
                            verticalDelete.Enqueue(cellPosition);
                        }
                    }
                }
                else
                {
                    if (verticalDelete.Count >= 4)
                    {
                        SetEmptyTiles(oldCellName, verticalDelete);
                    }
                    
                    oldCellName = null;
                    verticalDelete.Clear();
                }
            }
            oldCellName = null;
        }
    }

    private void HorizontalLineCheck()
    {
        RectInt bounds = this.Bounds;
        Queue<Vector3Int> horizontalDelete = this.DeleteQue;
        string oldCellName = null;

        for (int row = bounds.yMin; row <= bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col <= bounds.xMax; col++)
            {
                if (col == bounds.xMin)
                {
                    horizontalDelete.Clear();
                }
                Vector3Int cellPosition = new Vector3Int(col, row, 0);
                if (this.tilemap.HasTile(cellPosition))
                {
                    string curCellName = this.tilemap.GetTile(cellPosition).name;

                    if (oldCellName == null)
                    {
                        oldCellName = curCellName;
                        horizontalDelete.Enqueue(cellPosition);
                    }
                    else
                    {
                        if (oldCellName.Substring(0, 3) == curCellName.Substring(0, 3))
                        {
                            horizontalDelete.Enqueue(cellPosition);
                        }
                        else
                        {
                            if (horizontalDelete.Count >= 4)
                            {
                                SetEmptyTiles(oldCellName, horizontalDelete);
                            }
                            
                            oldCellName = curCellName;
                            horizontalDelete.Clear();
                            horizontalDelete.Enqueue(cellPosition);
                        }
                    }
                }
                else
                {
                    if (horizontalDelete.Count >= 4)
                    {
                        SetEmptyTiles(oldCellName, horizontalDelete);
                    }
                    
                    oldCellName = null;
                    horizontalDelete.Clear();
                }
            }
            oldCellName = null;
        }
    }

    private void SetEmptyTiles(string cellName, Queue<Vector3Int> deleteQueue)
    {
        hasEmptyTiles = true;
        int emptyColor = ColorTranslator(cellName[0]);
        EmptyData data = this.empties[emptyColor];
        
        foreach (var cell in deleteQueue)
        {
            string currentCellInQue = this.tilemap.GetTile(cell).name;
            if (currentCellInQue.Contains("HalfPill"))
            {
                SetSingleTiles(cell);
            }
            this.tilemap.SetTile((Vector3Int)cell, data.emptyTile);
        }
    }

    private void SetSingleTiles(Vector3Int cell)
    {
        string tileRotation = GetTileRotation(cell);
        if (tileRotation == "Horizontal1")
        {
            Vector3Int rightHalf = new Vector3Int(cell.x + 1, cell.y, cell.z);
            ChangeCorespondingTile(rightHalf);
        }
        else if (tileRotation == "Horizontal2")
        {
            Vector3Int leftHalf = new Vector3Int(cell.x - 1, cell.y, cell.z);
            ChangeCorespondingTile(leftHalf);
        }
        else if (tileRotation == "Vertical1")
        {
            Vector3Int bottomtHalf = new Vector3Int(cell.x, cell.y - 1, cell.z);
            ChangeCorespondingTile(bottomtHalf);
        }
        else
        {
            Vector3Int topHalf = new Vector3Int(cell.x, cell.y + 1, cell.z);
            ChangeCorespondingTile(topHalf);
        }
    }

    private void ChangeCorespondingTile(Vector3Int corespondingTile)
    {
        string singleHalfPillName = this.tilemap.GetTile(corespondingTile).name;
        int singleColor = ColorTranslator(singleHalfPillName[0]);
        SinglePillData data = this.singles[singleColor];
        this.tilemap.SetTile(corespondingTile, data.singlePillTile);
    }

    private int ColorTranslator(char c)
    {
        int returnNumber;
        switch (c)
        {
            case 'R': returnNumber = 0; break;
            case 'B': returnNumber = 1; break;
            case 'L': returnNumber = 2; break;
            default: Debug.Log("Tile name is empty"); returnNumber = 5; break;
        }
        return returnNumber;
    }

    private void DeleteMarkedCells()
    {
        RectInt bounds = this.Bounds;
        bool isNotCleared = false;

        for (int row = bounds.yMin; row <= bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col <= bounds.xMax; col++)
            {
                Vector3Int curCellPosition = new Vector3Int(col, row, 0);
                if (this.tilemap.HasTile(curCellPosition))
                {
                    string curCellName = this.tilemap.GetTile(curCellPosition).name;
                    bool cellToDelete = curCellName.Contains("Empty");
                    if (cellToDelete)
                    {
                        this.tilemap.SetTile(curCellPosition, null);
                        isNotCleared = true;
                    }
                }
            }
        }
        if (isNotCleared)
        {
            this.waitForLineClears = true;
            this.deleteMethodCalled = false;
            ManageFallingCells();
        }
        else
        {
            this.waitForLineClears = false;
        }
    }

    private void ManageFallingCells()
    {
        RectInt bounds = this.Bounds;

        for (int row = bounds.yMin; row <= bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col <= bounds.xMax; col++)
            {
                Vector3Int curCellPosition = new Vector3Int(col, row, 0);
                if (this.tilemap.HasTile(curCellPosition))
                {
                    string curCellName = this.tilemap.GetTile(curCellPosition).name;
                    bool curCellToStay = curCellName.Contains("Virus");
                    if (!curCellToStay)
                    {
                        bool curCellToFall = curCellName.Contains("Single");
                        for (int rowDown = row; rowDown > bounds.yMin; rowDown--)
                        {
                            Vector3Int cellBelowPosition = new Vector3Int(col, rowDown - 1, 0);
                            if (!this.tilemap.HasTile(cellBelowPosition))
                            {
                                TileBase curCellTileBase = this.tilemap.GetTile(curCellPosition);
                                Matrix4x4 curCellRotation = this.tilemap.GetTransformMatrix(curCellPosition);
                                if (curCellToFall)
                                {
                                    this.tilemap.SetTile(cellBelowPosition, curCellTileBase);
                                    this.tilemap.SetTile(curCellPosition, null);
                                    curCellPosition = cellBelowPosition;
                                }
                                else
                                {
                                    string tileRotation = GetTileRotation(curCellPosition);
                                    if (tileRotation == "Horizontal1")
                                    {
                                        Vector3Int curCellCounterpartPosition = new Vector3Int(col + 1, rowDown, 0);
                                        Vector3Int cellCounterpartBelowPosition = new Vector3Int(col + 1, rowDown - 1, 0);
                                        if (!this.tilemap.HasTile(cellCounterpartBelowPosition))
                                        {
                                            TileBase curCellCounterpartTileBase = this.tilemap.GetTile(curCellCounterpartPosition);
                                            Matrix4x4 curCellCounterpartRotation = this.tilemap.GetTransformMatrix(curCellCounterpartPosition);
                                            this.tilemap.SetTile(cellBelowPosition, curCellTileBase);
                                            this.tilemap.SetTransformMatrix(cellBelowPosition, curCellRotation);
                                            this.tilemap.SetTile(cellCounterpartBelowPosition, curCellCounterpartTileBase);
                                            this.tilemap.SetTransformMatrix(cellCounterpartBelowPosition, curCellCounterpartRotation);
                                            this.tilemap.SetTile(curCellPosition, null);
                                            this.tilemap.SetTile(curCellCounterpartPosition, null);
                                            curCellPosition = cellBelowPosition;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else if (tileRotation.Contains("Vertical2"))
                                    {
                                        Vector3Int curCellCounterpartPosition = new Vector3Int(col, rowDown + 1, 0);
                                        TileBase curCellCounterpartTileBase = this.tilemap.GetTile(curCellCounterpartPosition);
                                        Matrix4x4 curCellCounterpartRotation = this.tilemap.GetTransformMatrix(curCellCounterpartPosition);
                                        this.tilemap.SetTile(cellBelowPosition, curCellTileBase);
                                        this.tilemap.SetTransformMatrix(cellBelowPosition, curCellRotation);
                                        this.tilemap.SetTile(curCellPosition, curCellCounterpartTileBase);
                                        this.tilemap.SetTransformMatrix(curCellPosition, curCellCounterpartRotation);
                                        this.tilemap.SetTile(curCellCounterpartPosition, null);
                                        curCellPosition = cellBelowPosition;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private string GetTileRotation(Vector3Int cell)
    {
        float tileRotationMagnitude = this.tilemap.GetTransformMatrix(cell).rotation.eulerAngles.magnitude;
        if (tileRotationMagnitude == 90 || tileRotationMagnitude == 270)
        {
            if (tileRotationMagnitude == 90)
            {
                return "Horizontal1";
            }
            else
            {
                return "Horizontal2";
            }
        }
        else
        {
            if (tileRotationMagnitude == 0)
            {
                return "Vertical1";
            }
            else
            {
                return "Vertical2";
            }
        }
    }
}
