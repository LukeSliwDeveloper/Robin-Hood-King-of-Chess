using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Figure
{
    [SerializeField] private Vector2Int[] _path;
    [SerializeField, HideInInspector] private FigureType _type;

    private static float _animationSpeed = 2f;

    private int _pathPointIndex;
    private bool _newTargetSpotted, _oldTargetSpotted;
    private Vector2Int _newTargetPosition = Vector2Int.left, _oldTargetPosition = Vector2Int.down;
    private List<Vector2Int> _returnPath = new();

    public override IEnumerator TakeTurn()
    {
        Debug.Log("Guard");
        if (BoardManager.Instance.TrySpotPlayer(this, _type, out _newTargetPosition) && _newTargetPosition != _oldTargetPosition)
            SpotTarget(true);
        if (_newTargetSpotted)
        {
            _newBoardPosition = _newTargetPosition;
            _returnPath.Insert(0, BoardPosition);
        }
        else if (_oldTargetSpotted && BoardManager.Instance.CanReach(_oldTargetPosition.x, _oldTargetPosition.y, this, _type))
        {
            _newBoardPosition = _oldTargetPosition;
            _returnPath.Insert(0, BoardPosition);
        }
        else
        {
            if (_returnPath.Count > 0)
            {
                var figure = BoardManager.Instance.GetFigureAtPosition(_returnPath[0].x, _returnPath[0].y);
                if (figure != null && !figure.IsTempting)
                    _newBoardPosition = BoardPosition;
                else
                {
                    _newBoardPosition = _returnPath[0];
                    _returnPath.RemoveAt(0);
                }
            }
            else
            {
                var figure = BoardManager.Instance.GetFigureAtPosition(_path[_pathPointIndex].x, _path[_pathPointIndex].y);
                if (figure != null && !figure.IsTempting)
                    _newBoardPosition = BoardPosition;
                else
                {
                    _newBoardPosition = _path[_pathPointIndex];
                    _pathPointIndex = (_pathPointIndex + 1) % _path.Length;
                }
            }
        }
        var worldPosition = (Vector2)_newBoardPosition * BoardManager.SquareSize;
        var tweenSpeed = (worldPosition - (Vector2)transform.position).magnitude / _animationSpeed;
        DOTween.Sequence().Append(transform.DOMove(worldPosition, tweenSpeed)).AppendCallback(FinishTurn);
        yield return base.TakeTurn();
        if (BoardManager.Instance.TrySpotPlayer(this, _type, out _oldTargetPosition))
            SpotTarget(false);
        else
            _oldTargetSpotted = false;
        _newTargetSpotted = false;
    }

    private void SpotTarget(bool onTurnStart)
    {
        if (onTurnStart)
            _newTargetSpotted = true;
        else
            _oldTargetSpotted = true;
        // Show spotted Exclamation Mark
    }

    private void OnDrawGizmosSelected()
    {
        Vector3[] points = new Vector3[_path.Length];
        for (int i = 0; i < _path.Length; i++)
            points[i] = new Vector3(_path[i].x * BoardManager.SquareSize, _path[i].y * BoardManager.SquareSize, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawLineStrip(points, true);
        if (points.Length > 0)
            Gizmos.DrawSphere(points[points.Length - 1], .1f);
    }
}

public enum FigureType
{
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}