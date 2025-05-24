using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    [SerializeField] private Vector2Int _boardSize;

    public static readonly float SquareSize = .625f;

    private Vector2Int[] _knightMoves = new Vector2Int[]
    {
        new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
        new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(2, 1), new Vector2Int(2, -1)
    };
    private Vector2Int[] _bishopMoves = new Vector2Int[]
    {
        new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1)
    };
    private Vector2Int[] _rookMoves = new Vector2Int[]
    {
        new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
    };

    private Coroutine _turnsCoroutine;
    private int _movingFigureIndex;
    private Figure[,] _figuresOnBoard;
    private List<Figure> _figuresByPriority = new();

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
            if (newPosFigure.CompareTag("Player"))
            {
                EndGame();
                return;
            }
            else
                Destroy(newPosFigure.gameObject);
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
                foreach (var move in _knightMoves)
                {
                    searchedPosition.Set(figure.BoardPosition.x + move.x, figure.BoardPosition.y + move.y);
                    if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch))
                        targetPosition = searchedPosition;
                    if (stopSearch)
                        return targetPosition.x != -1;
                }
                return targetPosition.x != -1;
            case FigureType.Bishop:
                foreach (var move in _bishopMoves)
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
                foreach (var move in _rookMoves)
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
                foreach (var move in _bishopMoves)
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
                foreach (var move in _rookMoves)
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
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        searchedPosition.Set(figure.BoardPosition.x + x, figure.BoardPosition.y + y);
                        if (IsWorthGoingTo(searchedPosition.x, searchedPosition.y, out stopSearch))
                            targetPosition = searchedPosition;
                        if (stopSearch)
                            return targetPosition.x != -1;
                    }
                }
                return targetPosition.x != -1;
            default:
                break;
        }
        return false;
    }

    public Figure GetFigureAtPosition(int x, int y) => _figuresOnBoard[x, y];

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

    private void EndGame()
    {
        StopCoroutine(_turnsCoroutine);
    }

    private IEnumerator TakeTurns()
    {
        while (true)
        {
            yield return _figuresByPriority[_movingFigureIndex].TakeTurn();
            _movingFigureIndex = (_movingFigureIndex + 1) % _figuresByPriority.Count;
        }
    }
}
