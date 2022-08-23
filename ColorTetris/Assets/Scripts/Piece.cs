using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public PillData data { get; private set; }
    public Vector3Int possition { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Matrix4x4[] tileRotations { get; private set; }
    public int rotationIndex { get; private set; }

    public float waitBeforeStart = 1f;
    public bool gameHasBegun = false;

    public float stepDelay;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    public bool pauseForLineClears = false;

    public WinGame winGame;
    public LoseGame loseGame;
    public PauseMenuUI pauseMenu;

    private bool isGameOver = false;
    public bool isPaused = false;

    public void Initialize(Board board, Vector3Int possition, PillData data)
    {
        this.board = board;
        this.data = data;
        this.possition = possition;
        this.rotationIndex = 0;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
            this.tileRotations = new Matrix4x4[this.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
            this.tileRotations[i] = data.rots[i];
        }
    }

    private void Update()
    {
        int numberOfViruses = this.board.VirusCounter();
        isGameOver = this.board.gameover;

        HandlePauseInput();

        if (numberOfViruses > 0 && !isGameOver)
        {
            if (!gameHasBegun)
            {
                if (Time.time > waitBeforeStart)
                {
                    gameHasBegun = true;
                }
            }

            if (!pauseForLineClears)
            {
                this.board.Clear(this);

                this.lockTime += Time.deltaTime;

                HandleRotationInput();

                HandleMovementInput();

                HandleDropInput();

                if (Time.time >= this.stepTime)
                {
                    Step();
                }

                if (!pauseForLineClears)
                {
                    this.board.Set(this);
                }
            }
            else
            {
                BeginLineClearLogic();
            } 
        }
        else
        {
            this.board.inGameUI.HideUI();
            if (!isGameOver)
            {
                WinGame();
            }
            else
            {
                LoseGame();
            }
        }
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                isPaused = false;
            }
            else
            {
                isPaused = true;
            }
            PauseGame();
        }
    }

    private void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
    }

    private void HandleRotationInput()
    {
        // Counter-Clockwise rotation
        if (Input.GetKeyDown(KeyCode.C))
        {
            Rotate(-1);
        }
        // Clockwise rotation
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Rotate(1);
        }
    }

    private void HandleDropInput()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        pauseForLineClears = true;
        this.board.Set(this);
        BeginLineClearLogic();
    }

    private void BeginLineClearLogic()
    {
        pauseForLineClears = this.board.LineClears();
        if (!pauseForLineClears)
        {
            this.board.SpawnPiece();
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.possition;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.possition = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotation(this.rotationIndex);

        if (!TestWallKicks(this.rotationIndex))
        {
            this.rotationIndex = originalRotation;
            ApplyRotation(this.rotationIndex);
        }
    }

    private void ApplyRotation(int rotationIndex)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            cells[i] = (Vector3Int)Data.cells[(Rotation)this.rotationIndex][i];
            tileRotations[i] = Data.rotations[(Rotation)this.rotationIndex][i];
            this.board.SetTileRotation(this.possition, this, i);
        }
    }

    private bool TestWallKicks(int rotationIndex)
    {
        Vector2Int translation;

        if (rotationIndex == 0 || rotationIndex == 2)
        {
            for (int i = 0; i < Data.verticalToHorizontal.Length; i++)
            {
                translation = Data.verticalToHorizontal[i];

                if (Move(translation))
                {
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < Data.horizontalToVertical.Length; i++)
            {
                translation = Data.horizontalToVertical[i];
                
                if (Move(translation))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void WinGame()
    {
        winGame.Setup();
    }

    private void LoseGame()
    {
        loseGame.Setup();
    }

    private void PauseGame()
    {
        if (isPaused)
        {
            pauseMenu.Setup();
        }
        else
        {
            pauseMenu.Resume();
        }
    }
}
