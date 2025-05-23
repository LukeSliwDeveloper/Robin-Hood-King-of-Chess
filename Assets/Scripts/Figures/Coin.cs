using UnityEngine;

public class Coin : Figure
{
    public override bool IsStationary => true;
    public override bool IsTempting => true;
}
