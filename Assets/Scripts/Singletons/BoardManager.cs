using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    [SerializeField] private Vector2Int _boardSize;

    public static readonly float SquareSize = .625f;

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
        StartCoroutine(TakeTurns());
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
        int positionX, positionY;
        Figure checkedFigure;
        Figure foundFigure = null;
        switch (type)
        {
            case FigureType.Knight:
                break;
            case FigureType.Bishop:
                break;
            case FigureType.Rook:
                break;
            case FigureType.Queen:
                break;
            case FigureType.King:
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        positionX = figure.BoardPosition.x + x;
                        positionY = figure.BoardPosition.y + y;
                        if (positionX < 0 || positionY < 0 || positionX >= _boardSize.x || positionY >= _boardSize.y)
                            continue;
                        checkedFigure = _figuresOnBoard[positionX, positionY];
                        if (checkedFigure != null && checkedFigure.IsTempting)
                        {
                            if (checkedFigure.CompareTag("Player"))
                            {
                                targetPosition = checkedFigure.BoardPosition;
                                return true;
                            }
                            else
                                foundFigure = checkedFigure;
                        }
                    }
                }
                if (foundFigure != null)
                {
                    targetPosition = foundFigure.BoardPosition;
                    return true;
                }
                break;
            default:
                break;
        }
        targetPosition = new Vector2Int();
        return false;
    }

    private void EndGame()
    {

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
