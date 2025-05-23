using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Player : Figure
{
    public override bool IsTempting => true;

    public override IEnumerator TakeTurn()
    {
        Debug.Log("Player");
        _newBoardPosition = BoardPosition;
        DOTween.Sequence().AppendInterval(1f).AppendCallback(FinishTurn);
        return base.TakeTurn();
    }
}
