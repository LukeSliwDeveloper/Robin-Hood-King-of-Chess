using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PoiterHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent OnHover, OnStopHover;

    public void OnPointerEnter(PointerEventData eventData) => OnHover?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => OnStopHover?.Invoke();
}
