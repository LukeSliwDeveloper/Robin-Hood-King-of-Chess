using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Guard : Figure
{
    [SerializeField] private FigureType _type;

    private static float _animationSpeed = 1f;

    private bool _targetSpotted;
    private Vector2Int _targetPosition;

    public override IEnumerator TakeTurn()
    {
        Debug.Log("Guard");
        if (_targetSpotted)
        {
            _newBoardPosition = _targetPosition;
            var worldPosition = (Vector2)_targetPosition * BoardManager.SquareSize;
            var tweenSpeed = (worldPosition - (Vector2)transform.position).magnitude * _animationSpeed;
            DOTween.Sequence().Append(transform.DOMove(worldPosition, tweenSpeed)).AppendCallback(FinishTurn);
        }
        yield return base.TakeTurn();
        if (BoardManager.Instance.TrySpotPlayer(this, _type, out _targetPosition))
            SpotTarget();
        else
            _targetSpotted = false;
    }

    private void SpotTarget()
    {
        _targetSpotted = true;
        // Show spotted Exclamation Mark
    }

    private void OnDrawGizmosSelected()
    {
        
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