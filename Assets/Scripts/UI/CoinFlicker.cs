using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinFlicker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int _amount;
    [SerializeField, HideInInspector] private TMP_Text _amountLabel;
    [SerializeField, HideInInspector] private GameObject _coinPrefab;
    [SerializeField, HideInInspector] private AudioClip _clip;

    private Vector2 _idlePosition;
    private Vector2 _dragOffset;

    private void Awake()
    {
        _amountLabel.text = _amount.ToString();
        BoardManager.Instance.OnAddCoin += AddCoin;
    }

    private void Start()
    {
        _idlePosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_amount > 0 && BoardManager.Instance.IsPlayerTurn)
            _dragOffset = (Vector2)transform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_amount > 0 && BoardManager.Instance.IsPlayerTurn)
            transform.position = eventData.position + _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_amount <= 0 || !BoardManager.Instance.IsPlayerTurn)
            return;
        var realPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int pos = new(Mathf.RoundToInt(realPos.x / BoardManager.SquareSize), Mathf.RoundToInt(realPos.y / BoardManager.SquareSize));
        var figure = BoardManager.Instance.GetFigureAtPosition(pos.x, pos.y, out var outOfBounds);
        if (!outOfBounds && figure == null)
        {
            Instantiate(_coinPrefab, (Vector2)pos * BoardManager.SquareSize, Quaternion.identity);
            _amountLabel.text = (--_amount).ToString();
            BoardManager.Instance.EndPlayerTurn();
            GameManager.Instance.PlaySfx(_clip);
        }
        transform.position = _idlePosition;
    }

    private void AddCoin() => _amountLabel.text = (++_amount).ToString();
}
