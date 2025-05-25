using System.Collections;
using UnityEngine;

public abstract class Figure : MonoBehaviour
{
    [SerializeField, HideInInspector] protected SpriteRenderer _spriteRenderer;
    [field: SerializeField] public int Priority { get; private set; }

    public Vector2Int BoardPosition { get; private set; }
    public virtual bool IsStationary => false;
    public virtual bool IsTempting => false;

    protected Vector2Int _newBoardPosition;

    private bool _finishedTurn;
    private WaitUntil _takeTurnWait;

    private void Awake()
    {
        BoardPosition = new ((int)(transform.position.x / BoardManager.SquareSize), (int)(transform.position.y / BoardManager.SquareSize));
        _takeTurnWait = new WaitUntil(FinishedTurn);
        BoardManager.Instance.AddToBoard(this);
    }
    
    public virtual IEnumerator TakeTurn()
    {
        _finishedTurn = false;
        yield return _takeTurnWait;
    }

    protected virtual void FinishTurn()
    {
        _spriteRenderer.sortingOrder = 0;
        BoardManager.Instance.MoveFigure(this, _newBoardPosition);
        BoardPosition = _newBoardPosition;
        _finishedTurn = true;
    }

    private bool FinishedTurn() => _finishedTurn;
}
