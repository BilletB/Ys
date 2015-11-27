﻿using UnityEngine;
using System.Collections;

public class SlingshotMenu : MonoBehaviour {
    Vector3 startPos;
    Vector3 mouseDownPos, mouseUpPos;
    GameObject creditPanel;
    GameObject optionPanel;
    GameObject mainTitle;
    GameObject titlePanel;
    public float dist = 2;
    public bool shoot = false;
    public bool action = false;
    bool inMainMenu = true;
    public float force ;
    bool isOnMenu = false;
    public float distanceMax = 7;
    bool isSlingShotting = false;
    LineRenderer lineRenderer;
    Vector3 velocity;
    float playerSpeed;
    
	// Use this for initialization
	void Start () {
        inMainMenu = true;
        startPos = transform.position;
        creditPanel = GameObject.Find("CreditPanel");
        creditPanel.SetActive(false);
        optionPanel = GameObject.Find("OptionPanel");
        optionPanel.SetActive(false);
        titlePanel = GameObject.Find("TitlePanel");
        titlePanel.SetActive(false);
        mainTitle = GameObject.Find("MainTitleImage");
        mainTitle.SetActive(true);
        force = 10;
        action = false;
        lineRenderer = transform.parent.gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
	}

    void reset()
    {
        transform.parent.position = startPos;
        creditPanel.SetActive(false);
        transform.GetComponent<SpriteRenderer>().enabled = true;
        isOnMenu = false;
        titlePanel.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        velocity = transform.GetComponentInParent<Rigidbody>().velocity;
        playerSpeed = velocity.magnitude;
        if(Input.GetMouseButtonDown(0))
        {
            //afk
        }
        if (playerSpeed <= 0.5f && !shoot)
        {
            action = false;
            transform.GetComponentInParent<Rigidbody>().velocity *=0;
            int layer = 1 << 20;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward, Mathf.Infinity, layer);
            Debug.DrawRay(transform.position, Vector3.forward, Color.red);
            if (!inMainMenu)
            {
                if (hit.collider != null)
                {
                    switch (hit.collider.name)
                    {
                        case "PlayCollider":
                            Debug.Log("play");
                            Application.LoadLevelAsync("test");
                            break;
                        case "OptionCollider":
                            Debug.Log("option");
                            titlePanel.SetActive(false);
                            optionPanel.SetActive(true);
                            isOnMenu = true;
                            break;
                        case "CreditCollider":
                            titlePanel.SetActive(false);
                            creditPanel.SetActive(true);
                            isOnMenu = true;
                            break;
                        case "QuitCollider":
                            Application.Quit();
                            break;
                        default:
                            Debug.Log("other");
                            break;
                    }
                }
            }
            else
            {
                if (hit.collider != null && hit.collider.name =="PlayCollider")
                {
                    reset();
                    inMainMenu = false;
                    mainTitle.SetActive(false);
                }
            }
        }
	}

    public void quitCreditPanel()
    {
        reset();
        creditPanel.SetActive(false);
    }

    public void quitOptionPanel()
    {
        reset();
        optionPanel.SetActive(false);
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && shoot && isSlingShotting) // si le joueur est en train de tirer
        {
            // on scale la jauge de puissance en fonction de la distance
            ScaleJauge();
        }
    }

    void ScaleJauge()
    {
        lineRenderer.enabled = true;
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorPos = transform.position;
        mousePosInWorld.z = 0;

        dist = Vector3.Distance(new Vector3(cursorPos.x, cursorPos.y, 0.0f), new Vector3(mousePosInWorld.x, mousePosInWorld.y, 0.0f));
        lineRenderer.SetPosition(0, new Vector3(cursorPos.x, cursorPos.y, 0.0f));
        Vector3 direction = mousePosInWorld - cursorPos;
        direction.Normalize();
        direction = Vector3.ClampMagnitude(direction, 1.0f);
        direction.z = 0;
        if (dist > distanceMax)
        {
            dist = distanceMax;
        }
        if (dist > 3)
        {
            lineRenderer.SetColors(Color.yellow, Color.yellow);
        }
        else
        {
            lineRenderer.SetColors(Color.red, Color.red);
        }
        lineRenderer.SetPosition(1, cursorPos + (direction * dist));


        
    }

    void OnMouseDown()
    {
        /*StartCoroutine(CountDown(0.5f));
        if (!action && myID == GameManagerScript.instance.currentId && !transform.parent.GetComponent<Player>().doubleTap && !isOnPlayer && !transform.parent.GetComponent<Player>().isMovingBump && !transform.parent.GetComponentInChildren<ejection>().ejectionAction)
        {
            shoot = true;
            playing = true;
            transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            mouseDownPos = transform.parent.position;

        }*/
        if (!action&& !isOnMenu)
        {
            shoot = true;
            isSlingShotting = true;
            transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            mouseDownPos = transform.parent.position;
        }

    }

    void OnMouseUp()
    {
        isSlingShotting = false;
        if (!action && !isOnMenu)
        {
            mouseDownPos.z = 0;
            mouseUpPos = Input.mousePosition; //on stock la position de d'arrivée
            mouseUpPos = Camera.main.ScreenToWorldPoint(mouseUpPos);
            mouseUpPos.z = 0;
            //mouseDownPos = Camera.main.WorldToScreenPoint(mouseDownPos);
            //mouseUpPos.y = 0;
            shoot = false;
            dist = Vector3.Distance(mouseDownPos, mouseUpPos);
            if (dist > distanceMax)
            {
                dist = distanceMax;
            }
            if (dist > 3)
            {
                transform.GetComponent<SpriteRenderer>().enabled = false;
                action = true;
                var direction = mouseDownPos - mouseUpPos;
                transform.parent.GetComponentInParent<Rigidbody>().AddForce(direction * dist * force);

                //transform.parent.GetComponent<Player>().startTypeZone = transform.parent.GetComponent<Player>().currentTypeZone;
                //StartCoroutine(Wait(0.1f));

            }
            lineRenderer.enabled = false;
        }
    }

    /*IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        playing = false;
    }*/
}
