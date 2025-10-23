using System.Collections.Generic;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    [Header("Grid Settings")]
    public float spacing = 0.3f; // space between cards in world units
    public Vector2 startPosition = new Vector2(-3f, 3f); // top-left start position
    public bool centerGrid = true;

    private List<GameObject> spawnedCards = new List<GameObject>();

    public void ClearBoard()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null)
                Destroy(card);
        }
        spawnedCards.Clear();
    }

    public void SpawnDeck(List<int> deck, GameObject cardPrefab, Transform parent)
    {
        if (cardPrefab == null || deck == null || deck.Count == 0)
        {
            Debug.LogError("Card prefab or deck not assigned!");
            return;
        }

        ClearBoard();

        var rows = GameManager.Instance.rows;
        var cols = GameManager.Instance.cols;

        // figure out card size from its SpriteRenderer
        SpriteRenderer sr = cardPrefab.GetComponent<SpriteRenderer>();
        float cardWidth = sr.bounds.size.x;
        float cardHeight = sr.bounds.size.y;

        // calculate total grid size
        float gridWidth = cols * (cardWidth + spacing) - spacing;
        float gridHeight = rows * (cardHeight + spacing) - spacing;

        Vector2 startPos;
        if (centerGrid)
        {
            // center grid around (0,0)
            startPos = new Vector2(-gridWidth / 2f + cardWidth / 2f, gridHeight / 2f - cardHeight / 2f);
        }
        else
        {
            // use top-left position manually
            startPos = startPosition;
        }

        int total = rows * cols;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                if (index >= deck.Count)
                    break;

                int cardNumber = deck[index];

                GameObject newCard = Instantiate(cardPrefab, parent);
                newCard.name = $"Card_{cardNumber}_{index}";

                float x = startPos.x + c * (cardWidth + spacing);
                float y = startPos.y - r * (cardHeight + spacing);
                newCard.transform.localPosition = new Vector3(x, y, 0f);

                Card card = newCard.GetComponent<Card>();
                if (card != null)
                {
                    card.Initialize(cardNumber);
                }

                spawnedCards.Add(newCard);
            }
        }
    }
}
