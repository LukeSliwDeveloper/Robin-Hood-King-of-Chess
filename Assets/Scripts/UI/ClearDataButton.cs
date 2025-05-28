using UnityEngine;

public class ClearDataButton : MonoBehaviour
{
    public void Clear() => PlayerPrefs.DeleteAll();
}
