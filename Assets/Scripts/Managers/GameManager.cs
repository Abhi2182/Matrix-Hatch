using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // layout config
    [Header("Layout Config")]
    public int rows = 4;
    public int cols = 4;

    [Space(10)]
    public int score = 0;
    public int movesMade = 0;
    public bool isGameOver = false;

    // Prefab & containers (assign in inspector)
    public GameObject cardPrefab;
    public Transform cardsContainer;

    // layout controller
    public LayoutController layoutController;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        // sanity: check components if not assigned
        if (layoutController == null) Debug.Log("LayoutController not assigned in GameManager!");

        StartNewGame(rows, cols);
    }

    public void StartNewGame(int r, int c)
    {
        isGameOver = false;
        score = 0;
        movesMade = 0;
        rows = r; cols = c;

        // generate deck
        int total = rows * cols;
        if (total % 2 != 0) { Debug.LogWarning("Total cards must be even. Incrementing total by 1."); total += 1; }
        var deck = GenerateDeck(total);

        // layout cards
        layoutController.ClearBoard();
        layoutController.SpawnDeck(deck, cardPrefab, cardsContainer);
    }

    // Generate numeric deck (pairs of numbers)
    public List<int> GenerateDeck(int totalCards)
    {
        if (totalCards % 2 != 0) totalCards++;
        int pairs = totalCards / 2;
        List<int> deck = new List<int>();
        for (int i = 1; i <= pairs; i++)
        {
            deck.Add(i);
            deck.Add(i);
        }
        Shuffle(deck);
        return deck;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        // TODO: update UI
    }

    public void OnMismatch()
    {
        // optional negative points for mismatch
        score = Mathf.Max(0, score - 5);
    }

    public bool CanPlayerFlip(Card _card)
    {
        if (isGameOver) return false;
        if (_card == null) return false;
        if (_card.isMatched) return false;
        if (_card.isFaceUp || _card.isFlipping) return false; // prevent flipping an already faceup card
        return true;
    }

    public void CheckForGameOver()
    {
        // find all Card objects in the scene
        Card[] allCards = FindObjectsOfType<Card>();

        foreach (var card in allCards)
        {
            if (!card.isMatched)
                return; // still unmatched cards left
        }

        // If we reach here -> all cards are matched
        OnGameComplete();
    }

    private void OnGameComplete()
    {
        isGameOver = true;
        Debug.Log("==> Game Completed!");
    }
}
