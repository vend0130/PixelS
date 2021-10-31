using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector2 last;
    private new Camera camera;
    private GM gm;

    private void Start()
    {
        camera = Camera.main;
        gm = I.gm;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        last = camera.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!I.gm.firstSwipeInLevel)
        {
            if (!gm.firstSwipeInLevel)
            {
                gm.currentPixels.GoRotation();
                I.gui.LearningToPlay(true);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (I.gm.currentPixels.direction != Vector2.zero)
            return;

        Vector2 current = camera.ScreenToWorldPoint(eventData.position);
        Vector2 distance = current - last;

        if (distance.magnitude < .25f)
        {
            last = current;
            return;
        }

        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            if (distance.x > 0)
                Go(Vector2.right);
            else
                Go(Vector2.left);
        }
        else
        {
            if (distance.y > 0)
                Go(Vector2.up);
            else
                Go(Vector2.down);
        }
        last = current;
    }

    private void Go(Vector2 _go)
    {
        gm.currentPixels.NewDirection(_go);
        I.gui.LearningToPlay(true);
    }
}