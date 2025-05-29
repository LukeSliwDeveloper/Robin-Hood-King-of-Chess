using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField, HideInInspector] private Transform[] _guardMoveSquares;

    public static readonly float SquareSize = .625f;

    public bool IsPlayerTurn { get; private set; }

    private Dictionary<FigureType, Vector2Int[]> _guardMoves = new Dictionary<FigureType, Vector2Int[]>()
    {
        { FigureType.Knight, new Vector2Int[]
        {
            new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
            new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(2, 1), new Vector2Int(2, -1)
        }},
        { FigureType.King, new Vector2Int[]
        {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0),
            new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
        }},
        { FigureType.Bishop, new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1)
        }},
        { FigureType.Rook, new Vector2Int[]
        {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
        }},
        { FigureType.Queen, new Vector2Int[]
        {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1),
            new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1)
        }}
    };

    private Coroutine _turnsCoroutine;
    private int _movingFigureIndex;
    private Figure[,] _figuresOnBoard;
    private List<Figure> _figuresByPriority = new();

    public event Action OnAddCoin;
    public event Action<bool> OnGameOver;

    protected override bool Awake()
    {
        if (base.Awake())
            _figuresOnBoard = new Figure[_boardSize.x, _boardSize.y];
        return true;
    }

    private void Start()
    {
        _turnsCoroutine = StartCoroutine(TakeTurns());
    }

    public void AddToBoard(Figure figure)
    {
        _figuresOnBoard[figure.BoardPosition.x, figure.BoardPosition.y] = figure;
        if (!figure.IsStationary)
        {
            if (_figuresByPriority.Count == 0)
                _figuresByPriority.Add(figure);
            else
            {
                for (int i = 0; i < _figuresByPriority.Count; i++)
                {
                    if (figure.Priority >= _figuresByPriority[i].Priority)
                    {
                        _figuresByPriority.Insert(i, figure);
                        return;
                    }
                }
                _figuresByPriority.Add(figure);
            }
        }
    }

    public void MoveFigure(Figure figure, Vector2Int newPos)
    {
        _figuresOnBoard[figure.BoardPosition.x, figure.BoardPosition.y] = null;
        var newPosFigure = _figuresOnBoard[newPos.x, newPos.y];
        if(newPosFigure != null)
        {
            Destroy(newPosFigure.gameObject);
            if (newPosFigure.CompareTag("Player"))
            {
                EndGame(false);
                return;
            }
        }
        _figuresOnBoard[newPos.x, newPos.y] = figure;
    }

    public bool TrySpotPlayer(Figure figure, FigureType type, out Vector2Int targetPosition)
    {
        targetPosition = Vector2Int.left;
        bool stopSearch;
        Vector2Int searchedPosition = new();
        switch (type)
        {
            case FigureType.Knight:
                foreach (var move in _guardMoves[FigureType.Knight])
                {
                    searchedPosition.Set(figure.BoardPosition.x + move.x, figure.BoardPosition.y + move.y);
                    if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch))
                        targetPosition = searchedPosition;
                    if (stopSearch)
                        return targetPosition.x != -1;
                }
                return targetPosition.x != -1;
            case FigureType.Bishop:
                foreach (var move in _guardMoves[FigureType.Bishop])
                {
                    int rangeMultiplier = 1;
                    do
                    {
                        searchedPosition.Set(figure.BoardPosition.x + move.x * rangeMultiplier, figure.BoardPosition.y + move.y * rangeMultiplier);
                        rangeMultiplier++;
                        if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch, true))
                        {
                            targetPosition = searchedPosition;
                            if (stopSearch && _figuresOnBoard[targetPosition.x, targetPosition.y].CompareTag("Player"))
                                return true;
                        }
                        else if (stopSearch)
                            break;
                    }
                    while (true);
                }
                return targetPosition.x != -1;
            case FigureType.Rook:
                foreach (var move in _guardMoves[FigureType.Rook])
                {
                    int rangeMultiplier = 1;
                    do
                    {
                        searchedPosition.Set(figure.BoardPosition.x + move.x * rangeMultiplier, figure.BoardPosition.y + move.y * rangeMultiplier);
                        rangeMultiplier++;
                        if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch, true))
                        {
                            targetPosition = searchedPosition;
                            if (stopSearch && _figuresOnBoard[targetPosition.x, targetPosition.y].CompareTag("Player"))
                                return true;
                        }
                        else if (stopSearch)
                            break;
                    }
                    while (true);
                }
                return targetPosition.x != -1;
            case FigureType.Queen:
                foreach (var move in _guardMoves[FigureType.Queen])
                {
                    int rangeMultiplier = 1;
                    do
                    {
                        searchedPosition.Set(figure.BoardPosition.x + move.x * rangeMultiplier, figure.BoardPosition.y + move.y * rangeMultiplier);
                        rangeMultiplier++;
                        if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch, true))
                        {
                            targetPosition = searchedPosition;
                            if (stopSearch && _figuresOnBoard[targetPosition.x, targetPosition.y].CompareTag("Player"))
                                return true;
                        }
                        else if (stopSearch)
                            break;
                    }
                    while (true);
                }
                return targetPosition.x != -1;
            case FigureType.King:
                foreach (var move in _guardMoves[FigureType.King])
                {
                    searchedPosition.Set(figure.BoardPosition.x + move.x, figure.BoardPosition.y + move.y);
                    if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch))
                        targetPosition = searchedPosition;
                    if (stopSearch)
                        return targetPosition.x != -1;
                }
                return targetPosition.x != -1;
            default:
                break;
        }
        return false;
    }

    public Figure GetFigureAtPosition(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _boardSize.x && y < _boardSize.y)
            return _figuresOnBoard[x, y];
        else
            return null;
    }

    public Figure GetFigureAtPosition(int x, int y, out bool outOfBounds)
    {
        if (x >= 0 && y >= 0 && x < _boardSize.x && y < _boardSize.y)
        {
            outOfBounds = false;
            return _figuresOnBoard[x, y];
        }
        else
        {
            outOfBounds = true;
            return null;
        }
    }

    public bool CanReach(int x, int y, Figure figure, FigureType figureType)
    {
        if ((figureType == FigureType.Knight) || (figureType == FigureType.King))
        {
            if ((_figuresOnBoard[x, y] == null) || (_figuresOnBoard[x, y].IsTempting))
                return true;
            else
                return false;
        }
        else
        {
            Vector2Int currentPos = figure.BoardPosition;
            Vector2Int dir = new Vector2Int(x - figure.BoardPosition.x, y - figure.BoardPosition.y);
            dir.Clamp(-Vector2Int.one, Vector2Int.one);
            do
            {
                currentPos += dir;
                if ((_figuresOnBoard[currentPos.x, currentPos.y] != null) && (!_figuresOnBoard[currentPos.x, currentPos.y].IsTempting))
                    return false;
            }
            while (currentPos.x != x || currentPos.y != y);
            return true;
        }
    }

    public void ShowGuardMove(Guard guard, FigureType type, Vector2Int patrolPosition)
    {
        _guardMoveSquares[0].position = (Vector2)patrolPosition * SquareSize;
        _guardMoveSquares[0].gameObject.SetActive(true);
        Vector2Int position;
        Figure figure;
        int squareIndex = 1;
        int rangeMultiplier;
        bool stopOnObstacle = type != FigureType.Knight && type != FigureType.King;
        foreach (var move in _guardMoves[type])
        {
            rangeMultiplier = 1;
            do
            {
                position = guard.BoardPosition + move * rangeMultiplier;
                rangeMultiplier++;
                if (position == patrolPosition)
                    continue;
                figure = GetFigureAtPosition(position.x, position.y, out var outOfBounds);
                if (!outOfBounds && (figure == null || figure.IsTempting))
                {
                    _guardMoveSquares[squareIndex].position = (Vector2)position * SquareSize;
                    _guardMoveSquares[squareIndex].gameObject.SetActive(true);
                    squareIndex++;
                }
                else if (stopOnObstacle)
                {
                    break;
                }
            }
            while (stopOnObstacle);
        }
    }

    public void HideGuardMove()
    {
        foreach (var square in _guardMoveSquares)
        {
            if (!square.gameObject.activeSelf)
                break;
            square.gameObject.SetActive(false);
        }
    }

    public void EndGame(bool won)
    {
        StopCoroutine(_turnsCoroutine);
        OnGameOver?.Invoke(won);
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
            (_figuresByPriority[_movingFigureIndex] as Player).EndTurn();
    }

    public void AddCoin() => OnAddCoin?.Invoke();

    private bool IsWorthGoingTo(int positionX, int positionY, out bool stopSearch, bool stopOnObstacle = false)
    {
        stopSearch = false;
        if (positionX >= 0 && positionY >= 0 && positionX < _boardSize.x && positionY < _boardSize.y)
        {
            var checkedFigure = _figuresOnBoard[positionX, positionY];
            if (checkedFigure != null)
            {
                if (checkedFigure.IsTempting)
                {
                    if (checkedFigure.CompareTag("Player"))
                        stopSearch = true;
                    return true;
                }
                else if (stopOnObstacle)
                {
                    stopSearch = true;
                    return false;
                }
            }
        }
        else if (stopOnObstacle)
            stopSearch = true;
        return false;
    }

    private IEnumerator TakeTurns()
    {
        while (true)
        {
            if (_figuresByPriority[_movingFigureIndex].CompareTag("Player"))
                IsPlayerTurn = true;
            else
                IsPlayerTurn = false;
            yield return _figuresByPriority[_movingFigureIndex].TakeTurn();
            _movingFigureIndex = (_movingFigureIndex + 1) % _figuresByPriority.Count;
        }
    }
}
