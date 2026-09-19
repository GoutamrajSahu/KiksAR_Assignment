using DG.Tweening;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public static GameHUD Instance;
    [SerializeField] public FilterBoxController filterBox;

    private void Awake()
    {
        Instance = this;
    }
}
