using UnityEngine;
using UnityEngine.EventSystems;

public class GoLevel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int level;
    public void OnPointerClick(PointerEventData data)
    {
        I.gui.BtnAudioPlay();
        I.gm.NextLevel(2, level - 1);
    }
}
