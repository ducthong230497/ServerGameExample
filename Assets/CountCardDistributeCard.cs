using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCardDistributeCard : MonoBehaviour {
    public Transform startDistributePos;
    public GameObject stackCardMe;
    public GameObject stackCardOpponent;
    public GameObject cardPrefab;
    public AnimationCurve flipCurve;
    [Header("test")]
    public Sprite test;
    

    [HideInInspector] public Client client;
    private ServerObject serverObject;
    private bool isHost;

    private List<GameObject> cards;
    private void Awake()
    {
        //client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        //client.onGetCard += onGetCard;
        //serverObject = new ServerObject();
        //serverObject.PutString("cmd", ConstantData.GET_CARD);
        //serverObject.PutInt("numPlayer", 2);
        //serverObject.PutInt("roomID", client.LastJoinRoom);
        //client.SendData(serverObject);

        //test
        cards = new List<GameObject>();
        for (int i = 0, j = 0; i < 6; i++)
        {
            GameObject card = Instantiate(cardPrefab, startDistributePos.position, Quaternion.identity);
            card.GetComponent<SpriteRenderer>().sortingOrder = i % 2 == 0 ? j % 3 : ((j++) % 3);
            cards.Add(card);
        }

        //StartCoroutine(cardMoveToPos(cards[0].transform, stackCardMe.transform.GetChild(0), true));
        StartCoroutine(DistributeCard());
    }

    private void onGetCard(List<int> listCardIndex)
    {
        if (client.IsHostRoom)
        {

        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateCard()
    {

    }

    IEnumerator DistributeCard()
    {
        for (int i = 0, j = 0; i < 6; i++)
        {
            if (i % 2 == 0)
            {
                StartCoroutine(cardMoveToPos(cards[i].transform, stackCardMe.transform.GetChild(j), true));
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(FlipCard(cards[i].transform, test));
            }
            else
            {
                StartCoroutine(cardMoveToPos(cards[i].transform, stackCardOpponent.transform.GetChild(j++), false));
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        
    }

    IEnumerator cardMoveToPos(Transform cardToMove, Transform destination, bool me)
    {
        float t = 0;
        float startX = cardToMove.position.x;
        float startY = cardToMove.position.y;
        float endX = destination.position.x;
        float endY = destination.position.y;
        Quaternion startRotation = cardToMove.rotation;
        while(t < 2)
        {
            t += Time.deltaTime * 3;
            float newX = Mathf.Lerp(startX, endX, t/2);
            float baseY = Mathf.Lerp(startY, endY, t/2);
            float arc = (0.5f * (newX - startX) * (newX - endX)) / ((me ? -0.25f : 0.25f) * Mathf.Pow(endX - startX, 2));
            cardToMove.position = new Vector2(newX, baseY + arc);
            cardToMove.rotation = Quaternion.Lerp(startRotation, destination.rotation, t/2);
            yield return null;
        }
        cardToMove.position = destination.position;
        cardToMove.rotation = destination.rotation;
    }

    IEnumerator FlipCard(Transform cardToFlip, Sprite newSprite)
    {
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        Vector3 up = Vector3.up + Vector3.forward;
        Vector3 down = Vector3.down + Vector3.forward;
        while (t < 1)
        {
            t += Time.deltaTime;
            float angle = 90 * flipCurve.Evaluate(t);
            //cardToFlip.localRotation = t < 0.5f ? Quaternion.AngleAxis(angle, transform.up) : Quaternion.AngleAxis(-angle, transform.up * -1);
            cardToFlip.transform.Rotate(t < 0.5 ? Vector3.up * Time.deltaTime * 180 : Vector3.up * Time.deltaTime * -180, Space.Self);
            if (t > 0.5) cardToFlip.GetComponent<SpriteRenderer>().sprite = newSprite;
            yield return new WaitForEndOfFrame();
        }
    }
}
