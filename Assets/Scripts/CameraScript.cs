using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float def = .055f;

    private void Awake()
    {
        I.cameraScript = this;
    }

    public IEnumerator RF75(Vector2 direction)
    {
        direction *= -1;
        float currentTime = 0;
        Vector3 newPos = transform.position;
        while (currentTime < def)
        {
            newPos.x += direction.x * def;
            newPos.y += direction.y * def;
            transform.position = newPos;
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(0, 0, -10);
        if (I.gm.currentPixels.getPosPixels() < -30)
        {
            Vector3 _c = transform.position;
            _c.y = I.gm.currentPixels.getPosPixels();
            transform.position = _c;
        }
    }
}
