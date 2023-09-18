using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

// Card deal animation and assign data to each card
public class CardDeal : MonoBehaviourPun
{
    const int ALL_CARD_NUMBER = 6;

    [SerializeField] float moveDuration = 0.1f;
    [SerializeField] Vector3 instantiatePoint = new Vector3(9, 5, 0);
    [SerializeField] GameObject[] cardHolder = new GameObject[ALL_CARD_NUMBER];

    [Space]

    [SerializeField] CardRecordSO cardRecordSO;

    [Space]

    [SerializeField] AudioSource cardAudio;
    [SerializeField] AudioClip dealCardAS;
    [SerializeField] AudioClip flipCardAS;

    List<CardsSO> cardSet = new List<CardsSO>();
    int cardsDealDoneCount = 0;
    GameObject[] cardPointer = new GameObject[ALL_CARD_NUMBER];
    Quaternion flip90 = Quaternion.Euler(0, 90, 0);
    Quaternion flip180 = Quaternion.Euler(0, 0, 0);

    // Queue for one by one animation
    Queue<(GameObject card, Vector3 holderPosition, CardInfo cardInfo)> cardQueue = new Queue<(GameObject, Vector3, CardInfo)>();

    // For Photon Share Data
    PhotonView photonView;
    string normalCardPath = "CardCover_Pho/NormalCard";
    string answerCardPath = "CardCover_Pho/AnswerCard";
    int[] randomCardData = new int[ALL_CARD_NUMBER];
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("CardDeal_Pho PV is NULL");
            return;
        }

        cardSet = Resources.LoadAll<CardsSO>("Card").ToList(); // Load 52 scriptable object
    }
    private void OnEnable()
    {
        Events.onDeal.AddListener(Deal);
        Events.onReset.AddListener(Reset);
    }

    private void OnDisable()
    {
        Events.onDeal.RemoveListener(Deal);
        Events.onReset.RemoveListener(Reset);
    }

    void Deal()
    {
        Events.onWaiting.Invoke(false);
        if (cardPointer[ALL_CARD_NUMBER - 1] == null)
        {
            for (int i = 0; i < ALL_CARD_NUMBER; i++)
            {
                GameObject cardTMP;

                if (i == ALL_CARD_NUMBER - 1)
                {
                    cardTMP = PhotonNetwork.Instantiate(answerCardPath, instantiatePoint, Quaternion.identity);
                }
                else
                {
                    cardTMP = PhotonNetwork.Instantiate(normalCardPath, instantiatePoint, Quaternion.identity);
                }
                CardInfo cardInfo = cardTMP.GetComponent<CardInfo>();

                cardPointer[i] = cardTMP;

                cardQueue.Enqueue((cardTMP, cardHolder[i].transform.position, cardInfo));
            }
            if (PhotonNetwork.IsMasterClient)
            {
                MasterGetRandomNumber();
            }
        }
    }

    private void MasterGetRandomNumber()
    {
        for (int i = 0; i < ALL_CARD_NUMBER; i++)
        {
            int randomNumber = Random.Range(0, cardSet.Count);
            while (randomCardData.Contains(randomNumber))
            {
                randomNumber = Random.Range(0, cardSet.Count);
            }
            randomCardData[i] = randomNumber;
        }
        photonView.RPC("PassRandomCardNumber", RpcTarget.All, randomCardData);

    }
    
    [PunRPC]
    void PassRandomCardNumber(int[] _randomCardData)
    {
        randomCardData = _randomCardData;
        StartCoroutine(DequeueAnimation());
    }


    private IEnumerator DequeueAnimation() // it will only Dequeue when the last one's CardAnimation is done
    {
        while (cardQueue.Count > 0)
        {
            (GameObject card, Vector3 holderPosition, CardInfo carInfo) = cardQueue.Dequeue();
            yield return StartCoroutine(CardAnimation(card, holderPosition, carInfo));
        }
    }
    private IEnumerator CardAnimation(GameObject card, Vector3 holderPosition, CardInfo cardInfo)
    {
        yield return StartCoroutine(CardMoveAnimation(card, holderPosition)); // move to holder place
        yield return StartCoroutine(RotateLerp(card.transform, flip90));
        AssignOneCardData(card, cardInfo);
        yield return StartCoroutine(RotateLerp(card.transform, flip180));
    }
    private void AssignOneCardData(GameObject card, CardInfo cardInfo)
    {
        int randomNumberTmp = randomCardData[cardsDealDoneCount];
        CardsSO cardTmp = cardSet[randomNumberTmp];

        Sprite sprite = cardTmp._sprite; //OOR => shit cuz i remove card first
        cardInfo.SpriteUpdate(sprite);
        cardInfo.CardIndexUpdate(cardTmp._index);
        if (card.name.Contains("Answer"))
        {
            cardRecordSO.AnswerCardUpdate(cardInfo.cardIndex);
        }
    }
    private IEnumerator CardMoveAnimation(GameObject card, Vector3 endPoint)
    {
        float startTime = Time.time;
        cardAudio.clip = dealCardAS;
        while (Time.time - startTime < moveDuration)
        {
            float t = (Time.time - startTime) / moveDuration;
            card.transform.position = Vector3.Lerp(instantiatePoint, endPoint, t);
            yield return null;
        }
        card.transform.position = endPoint;
    }

    private IEnumerator RotateLerp(Transform target, Quaternion endRot)
    {
        if(endRot == flip90)
        {
            cardAudio.clip = flipCardAS;
            cardAudio.Play();
        }
        var startRot = target.rotation; // current rotation
        for (float timer = 0; timer < moveDuration; timer += Time.deltaTime)
        {
            target.rotation = Quaternion.Slerp(startRot, endRot, timer / moveDuration);
            yield return null;
        }
        if (endRot == flip180)
        {
            cardsDealDoneCount++;

            if (cardsDealDoneCount == ALL_CARD_NUMBER - 1)
            {
                MasterClientChangeStateAfterDeal();
            }
        }
        target.rotation = endRot;
    }
    private void MasterClientChangeStateAfterDeal()
    {
        Events.onMasterStateChange.Invoke(GameState.Calc);
    }
    private void Reset()
    {
        foreach (GameObject card in cardPointer)
        {
            Destroy(card);
        }
        cardPointer = new GameObject[ALL_CARD_NUMBER];
        cardsDealDoneCount = 0;
        randomCardData = new int[ALL_CARD_NUMBER];
        Events.onResetCount.Invoke();
    }
}
