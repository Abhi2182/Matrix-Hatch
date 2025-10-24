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
            Card card_A = flipQueue.Dequeue();
            Card card_B = flipQueue.Dequeue();

            // skip invalid cards
            if (card_A == null || card_B == null)
                continue;
            if (card_A.isMatched || card_B.isMatched)
                continue;
            if (card_A == card_B)
                continue;

            //CARDS MATCHED
            if (card_A.cardID == card_B.cardID)
            {
                // combo system
                float now = Time.time;
                if (now - lastMatchTime <= comboWindow)
                    comboCount++;
                else
                    comboCount = 0;

                lastMatchTime = now;

                int points = Mathf.RoundToInt(basePoints * (1 + comboCount * 0.25f));
                GameManager.Instance?.AddScore(points);

                AudioManager.Instance?.PlayMatch(); // play card match sound

                // mark as matched + animate
                StartCoroutine(card_A.OnMatchedAnimCoroutine());
                StartCoroutine(card_B.OnMatchedAnimCoroutine());

                // after short delay, check if all cards are matched
                yield return new WaitForSeconds(0.3f);
                GameManager.Instance?.CheckForGameOver();
            }
            else // CARDS NOT MATCHED — flip both back after delay
            {

                yield return new WaitForSeconds(mismatchDelay);
                AudioManager.Instance?.PlayMismatch(); // play card mismatch sound

                if (!card_A.isMatched && card_A.isFaceUp)
                    card_A.FlipBackImmediate();

                if (!card_B.isMatched && card_B.isFaceUp)
                    card_B.FlipBackImmediate();

                GameManager.Instance?.OnMismatch();
            }
            GameManager.Instance?.IncreaseMoveCount();
        }

        isProcessing = false;
    }
}
