using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Guard : Figure
{
    [SerializeField] private Vector2Int[] _path;
    [SerializeField, HideInInspector] private FigureType _type;
    [SerializeField, HideInInspector] private SpriteRenderer _spottedSpriteRenderer;
    [SerializeField, HideInInspector] private AudioClip _spottedAudioClip, _moveClip;

    private static float _animationSpeed = 2f;
    private static float _spottedAnimationTime = .5f;

    private int _pathPointIndex;
    private bool _newTargetSpotted, _oldTargetSpotted;
    private bool _isSpottedAnimationPlaying;
    private Vector2Int _newTargetPosition = Vector2Int.left, _oldTargetPosition = Vector2Int.down;
    private List<Vector2Int> _returnPath = new();
    private WaitUntil _spottedAnimationWait;

    protected override void Awake()
    {
        base.Awake();
        _spottedAnimationWait = new WaitUntil(() => !_isSpottedAnimationPlaying);
    }

    public override IEnumerator TakeTurn()
    {
        ShowPath(false);
        _spriteRenderer.sortingOrder = 1;
        if (BoardManager.Instance.TrySpotPlayer(this, _type, out _newTargetPosition) && _newTargetPosition != _oldTargetPosition)
        {
            SpotTarget(true);
            yield return _spottedAnimationWait;
        }
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
        var tweenSpeed = Mathf.Max((worldPosition - (Vector2)transform.position).magnitude / _animationSpeed, 0.01f);
        transform.DOPunchScale(Vector3.up * .1f, tweenSpeed, 0, 0f);
        GameManager.Instance.PlaySfx(_moveClip);
        DOTween.Sequence().Append(transform.DOMove(worldPosition, tweenSpeed)).AppendCallback(FinishTurn);
        yield return base.TakeTurn();
        if (BoardManager.Instance.TrySpotPlayer(this, _type, out _oldTargetPosition))
        {
            SpotTarget(false);
            yield return _spottedAnimationWait;
        }
        else
            _oldTargetSpotted = false;
        _newTargetSpotted = false;
    }

    public void ShowPath(bool show)
    {
        if (show) 
        { 
            Vector2Int targetPos;
            if (_oldTargetSpotted)
                targetPos = _oldTargetPosition;
            else if (_returnPath.Count > 0)
                targetPos = _returnPath[0];
            else
                targetPos = _path[_pathPointIndex];
            BoardManager.Instance.ShowGuardMove(this, _type, targetPos);
        }
        else
            BoardManager.Instance.HideGuardMove();
    }

    private void SpotTarget(bool onTurnStart)
    {
        GameManager.Instance.PlaySfx(_spottedAudioClip);
        if (onTurnStart)
            _newTargetSpotted = true;
        else
            _oldTargetSpotted = true;
        _isSpottedAnimationPlaying = true;
        _spottedSpriteRenderer.color = Color.white;
        transform.DOPunchScale(Vector3.one * .2f, _spottedAnimationTime, 0, 0f);
        _spottedSpriteRenderer.DOFade(0f, _spottedAnimationTime).OnComplete(() => _isSpottedAnimationPlaying = false);
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