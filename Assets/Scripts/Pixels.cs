using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixels : MonoBehaviour
{
    [SerializeField] private Transform endPoints;
    [SerializeField] private Transform pixels;
    [SerializeField] private GameObject help;


    private List<GameObject> listPixels;
    private List<GameObject> listEndPoin;
    private LayerMask layerBorder;
    private GameObject endPrefab;
    private float speed = 70f;
    private Vector2 target;
    private Coroutine coroutine;
    private int countSwipe;
    private Vector2 _direction;
    private bool _helpActive;

    public Vector2 direction { get => _direction; set => _direction = value; }
    public bool helpActive { get => _helpActive; set => _helpActive = value; }

    private void Start()
    {
        help.SetActive(false);
        listPixels = new List<GameObject>();
        listEndPoin = new List<GameObject>();
        layerBorder = I.gm.layerBorder;
        endPrefab = I.gm.endPrefab;
        foreach (Transform child in pixels)//добавляем в лист все "пиксели"
        {
            listPixels.Add(child.gameObject);
        }
        foreach (Transform child in endPoints)
        {
            listEndPoin.Add(child.gameObject);
        }
        if (I.levelHelp == I.currentLevel)
            OpenHelp();
    }

    private void LateUpdate()
    {
        if (direction != Vector2.zero)
            Move();
    }

    public void NewDirection(Vector2 newDir)
    {
        //если двигаемся или открыта панель EndGame или поворачиваемя
        if (direction != Vector2.zero || I.gm.stateGame == StateGame.EndGame || coroutine != null)
            return;

        Vector2 kuda = Vector2.zero;
        float distantion = 9999;
        float offset = 0;

        for (int i = 0; i < listPixels.Count; i++)
        {
            Vector2 a = listPixels[i].transform.position;
            Vector2 b = newDir;

            RaycastHit2D hit = Physics2D.Raycast(a, b, 111f, layerBorder);
            if (hit.collider)
            {
                if (newDir == Vector2.up)
                {
                    float newDistantion = hit.transform.position.y - listPixels[i].transform.position.y;
                    if(newDistantion < distantion)
                    {
                        kuda = new Vector2(pixels.transform.position.x, hit.transform.position.y);
                        distantion = newDistantion;
                        offset = listPixels[i].transform.position.y - pixels.transform.position.y;
                    }
                }
                else if (newDir == Vector2.down)
                {
                    float newDistantion = listPixels[i].transform.position.y - hit.transform.position.y;
                    if (newDistantion < distantion)
                    {
                        kuda = new Vector2(pixels.transform.position.x, hit.transform.position.y);
                        distantion = newDistantion;
                        offset = pixels.transform.position.y - listPixels[i].transform.position.y;
                    }
                }
                else if (newDir == Vector2.right)
                {
                    float newDistantion = hit.transform.position.x - listPixels[i].transform.position.x;
                    if (newDistantion < distantion)
                    {
                        kuda = new Vector2(hit.transform.position.x, pixels.transform.position.y);
                        distantion = newDistantion;
                        offset = listPixels[i].transform.position.x - pixels.transform.position.x;
                    }
                }
                else if (newDir == Vector2.left)
                {
                    float newDistantion = listPixels[i].transform.position.x - hit.transform.position.x;
                    if (newDistantion < distantion)
                    {
                        kuda = new Vector2(hit.transform.position.x, pixels.transform.position.y);
                        distantion = newDistantion;
                        offset = pixels.transform.position.x - listPixels[i].transform.position.x;
                    }
                }
            }
        }
        if (newDir == Vector2.up)
            kuda.y -= offset + 1;
        else if (newDir == Vector2.down)
            kuda.y += offset + 1;
        else if(newDir == Vector2.right)
            kuda.x -= offset + 1;
        else if (newDir == Vector2.left)
            kuda.x += offset + 1;

        if ((Vector2)pixels.transform.position == kuda)//если в сторону свайпа есть стенка
            return;

        //если есть интернет
        if (countSwipe == 31)
        {
            countSwipe = 75;
            I.gui.Flag(true);
        }
        else if(countSwipe < 31)
            countSwipe++;

        target = kuda;
        direction = newDir;

        I.gm.firstSwipeInLevel = true;
    }

    private void Move()
    {
        pixels.transform.position = Vector2.MoveTowards(pixels.transform.position, target, Time.deltaTime * speed);
        if ((Vector2)pixels.transform.position == target)
            Stop();
    }

    private void Stop()
    {
        for (int i = 0; i < listPixels.Count; i++)
        {
            for (int j = 0; j < listEndPoin.Count; j++)
            {
                if ((Vector2)listPixels[i].transform.position == (Vector2)listEndPoin[j].transform.position)
                {
                    Vector3 _pos = listPixels[i].transform.position;
                    _pos.z = -5;
                    Instantiate(endPrefab, _pos, Quaternion.identity, endPoints);
                    Destroy(listPixels[i]);
                    listPixels.Remove(listPixels[i]);
                    listEndPoin.Remove(listEndPoin[j]);
                    Stop();
                    return;
                }
            }
        }

        if (listPixels.Count == 0 && listEndPoin.Count == 0)
        {
            I.audioManager.Play("endlevel");
            I.gm.Completed();
        }
        else
            I.audioManager.Play("stop");

        if(I.jiggle)
            StartCoroutine(I.cameraScript.RF75(direction));//тряска

        direction = Vector2.zero;
    }

    public void OpenHelp()
    {
        helpActive = true;
        help.SetActive(true);
    }

    public void GoRotation()
    {
        I.audioManager.Play("rotation");
        if (!I.gm.rotation)
        {
            I.gm.rotation = true;
        }
        if (coroutine == null)
            coroutine = StartCoroutine(CoroutineRotation());
        //pixels.rotation = Quaternion.Euler(0, 0, pixels.eulerAngles.z - 90);
    }

    private IEnumerator CoroutineRotation()
    {
        float speed = 1500f;
        float startAngle = pixels.eulerAngles.z;
        float currentAngle = startAngle;
        float targetAngle = currentAngle - 90;

        while (currentAngle > targetAngle)
        {
            currentAngle -= speed * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle, targetAngle, startAngle);
            pixels.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }
        coroutine = null;
    }

    public float getPosPixels()
    {
        return pixels.position.y;
    }
}