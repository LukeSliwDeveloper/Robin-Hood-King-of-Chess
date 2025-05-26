using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Player : Figure
{
    [SerializeField, HideInInspector] SerializedDictionary<Vector2Int, GameObject> MovePositions;

    public override bool IsTempting => true;

    private float _animationSpeed = 2f;

    private bool _positionOutOfBounds;
    private Figure _blockingFigure;

    public void MoveToPosition(Transform target)
    {
        _spriteRenderer.sortingOrder = 1;
        foreach (var movePosition in MovePositions)
        {
            if (target.gameObject == movePosition.Value)
            {
                _newBoardPosition = BoardPosition + movePosition.Key;
                var worldPosition = (Vector2)_newBoardPosition * BoardManager.SquareSize;
                var tweenSpeed = (worldPosition - (Vector2)transform.position).magnitude / _animationSpeed;
                DOTween.Sequence().Append(transform.DOMove(worldPosition, tweenSpeed)).AppendCallback(FinishTurn);
            }
        }
        foreach (var movePosition in MovePositions)
            movePosition.Value.SetActive(false);
    }

    public override IEnumerator TakeTurn()
    {
        foreach (var movePosition in MovePositions)
        {
            _blockingFigure = BoardManager.Instance.GetFigureAtPosition(movePosition.Key.x + BoardPosition.x, movePosition.Key.y + BoardPosition.y, out _positionOutOfBounds);
            if ((_blockingFigure == null || _blockingFigure.IsTempting || _blockingFigure.CompareTag("Treasure")) && !_positionOutOfBounds)
                movePosition.Value.SetActive(true);
        }
        return base.TakeTurn();
    }

    public void EndTurn()
    {
        _newBoardPosition = BoardPosition;
        foreach (var movePosition in MovePositions)
            movePosition.Value.SetActive(false);
        FinishTurn();
    }

    protected override void FinishTurn()
    {
        _blockingFigure = BoardManager.Instance.GetFigureAtPosition(_newBoardPosition.x, _newBoardPosition.y);
        if (_blockingFigure != null)
        {
            if (_blockingFigure.CompareTag("Treasure"))
                BoardManager.Instance.EndGame(true);
            else if (_blockingFigure.IsTempting && _blockingFigure != this)
                BoardManager.Instance.AddCoin();
        }
        base.FinishTurn();
    }
}
