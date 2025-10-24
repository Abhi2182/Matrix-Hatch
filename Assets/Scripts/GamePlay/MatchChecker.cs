using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    public static MatchChecker Instance { get; private set; }

    private Queue<Card> flipQueue = new Queue<Card>();
    private bool isProcessing = false;

    [Header("Gameplay Settings")]
    public float mismatchDelay = 0.5f;  // wait before flipping mismatched cards back
    public int basePoints = 100;
    public float comboWindow = 3f;      // time window for combo scoring

    private int comboCount = 0;
    private float lastMatchTime = -999f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called by Card when flipped face-up.
    /// </summary>
    public void EnqueueCard(Card card)
    {
        if (card == null || card.isMatched)
            return;

        flipQueue.Enqueue(card);

        // Start processing queue
        if (!isProcessing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (flipQueue.Count >= 2)
        {
            Card cardA = flipQueue.Dequeue();
            Card cardB = flipQueue.Dequeue();

            // skip invalid cards
            if (!IsValidPair(cardA, cardB))
                continue;

            if (IsMatch(cardA, cardB))
                yield return HandleMatch(cardA, cardB);
            else
                yield return HandleMismatch(cardA, cardB);

            GameManager.Instance?.IncreaseMoveCount();
        }

        isProcessing = false;
    }

    private bool IsValidPair(Card a, Card b)
    {
        return a != null && b != null && a != b && !a.isMatched && !b.isMatched;
    }

    private bool IsMatch(Card a, Card b) => a.cardID == b.cardID;

    private IEnumerator HandleMatch(Card cardA, Card cardB)
    {
        // Combo logic
        float now = Time.time;
        comboCount = (now - lastMatchTime <= comboWindow) ? comboCount + 1 : 0;
        lastMatchTime = now;

        int points = Mathf.RoundToInt(basePoints * (1 + comboCount * 0.25f));
        GameManager.Instance?.AddScore(points);

        AudioManager.Instance?.PlayMatch();

        // Animate both cards
        StartCoroutine(cardA.OnMatchedAnimCoroutine());
        StartCoroutine(cardB.OnMatchedAnimCoroutine());

        // after short delay, check if all cards are matched
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance?.CheckForGameOver();
    }

    private IEnumerator HandleMismatch(Card cardA, Card cardB)
    {
        yield return new WaitForSeconds(mismatchDelay);

        AudioManager.Instance?.PlayMismatch();

        if (!cardA.isMatched && cardA.isFaceUp)
            cardA.FlipBackImmediate();

        if (!cardB.isMatched && cardB.isFaceUp)
            cardB.FlipBackImmediate();

        GameManager.Instance?.OnMismatch();
    }
}
