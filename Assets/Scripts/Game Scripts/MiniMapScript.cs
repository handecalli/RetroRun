using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class MiniMapScript : PunBehaviour {

	public Transform startZone;
	public Transform finishZone;
	
	public float slideWidth = 0.1f;

	public Image mmChar1;
	public Image mmChar2;

    GameObject player1ImageObject;
    GameObject player2ImageObject;
	
	public GameObject target1;
	public GameObject target2;

    private GameObject[] players;

    private string otherPlayerID;

	private float mapEnd;
	private float mapStart;
	private float mapLenght;
	private float mapPlayer1;
	private float mapPos1;
	private float mapPlayer2;
	private float mapPos2;
	
    private bool moveMiniMap = false;

	Vector2 slideAnchorMin1;
	Vector2 slideAnchorMax1;
	Vector2 slideAnchorMin2;
	Vector2 slideAnchorMax2;

    void OnEnable()
    {
        PhotonPlayerScript.InitMinimap += InitializeMiniMapChar;
        PhotonPlayerScript.StopMinimap += StopMiniMap;
    }

    void OnDisable()
    {
        PhotonPlayerScript.InitMinimap -= InitializeMiniMapChar;
        PhotonPlayerScript.StopMinimap -= StopMiniMap;
    }

    void InitializeMiniMapChar()
    {
        FindMapLength();

        FindPlayers();

        SetImages();

        UIResolution();
    }

    void Update()
    {
        if (moveMiniMap)
        {
            MovePlayer1();
            MovePlayer2();
        }
    }

    void UIResolution()
    {
        mmChar1.rectTransform.anchorMin = slideAnchorMin1;
        mmChar1.rectTransform.anchorMax = slideAnchorMax1;
        mmChar2.rectTransform.anchorMin = slideAnchorMin2;
        mmChar2.rectTransform.anchorMax = slideAnchorMax2;

        slideAnchorMin1.y = -0.2f;
        slideAnchorMax1.y = 1.2f;
        slideAnchorMin2.y = -0.2f;
        slideAnchorMax2.y = 1.2f;

        moveMiniMap = true;
    }

    void SetImages()
    {
        player1ImageObject = GameObject.Find("mmP1");
        player2ImageObject = GameObject.Find("mmP2");

        mmChar1 = player1ImageObject.GetComponent<Image>();
        mmChar2 = player2ImageObject.GetComponent<Image>();

        mmChar1.sprite = target1.GetComponent<SpriteRenderer>().sprite;

        mmChar2.sprite = target2.GetComponent<SpriteRenderer>().sprite;
    }

    void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        target1 = players[0];
        target2 = players[1];
    }

    void FindMapLength()
    {
        startZone = GameObject.Find("StartZone").transform;
        finishZone = GameObject.Find("FinishZone").transform;

        mapStart = startZone.position.x;
        mapEnd = finishZone.position.x;

        mapLenght = mapEnd - mapStart;
    }

    void MovePlayer1()
    {
        if (target1 != null)
        {
            mapPlayer1 = target1.transform.position.x;
            mapPos1 = (mapPlayer1 - mapStart) / mapLenght;

            slideAnchorMin1.x = mapPos1 - slideWidth / 2;
            slideAnchorMax1.x = mapPos1 + slideWidth / 2;

            mmChar1.rectTransform.anchorMin = slideAnchorMin1;
            mmChar1.rectTransform.anchorMax = slideAnchorMax1;
        }
    }

    void MovePlayer2()
    {
        if (target2 != null)
        {
            mapPlayer2 = target2.transform.position.x;
            mapPos2 = (mapPlayer2 - mapStart) / mapLenght;

            slideAnchorMin2.x = mapPos2 - slideWidth / 2;
            slideAnchorMax2.x = mapPos2 + slideWidth / 2;

            mmChar2.rectTransform.anchorMin = slideAnchorMin2;
            mmChar2.rectTransform.anchorMax = slideAnchorMax2;
        }
    }

    void StopMiniMap()
    {
        moveMiniMap = false;
    }
}