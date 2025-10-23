using System.Collections.Generic;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    [Header("Grid Settings")]
    public float spacing = 0.3f; // space between cards
    public float padding = 1f; // extra space at screen edges
    private List<GameObject> spawnedCards = new List<GameObject>();
    [SerializeField]private Camera mainCam;

    public void ClearBoard()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null)
                Destroy(card);
        }
        spawnedCards.Clear();
    }

    //Spawn cards in grid layout, scaling to fit screen
    public void SpawnDeck(List<int> deck, GameObject cardPrefab, Transform parent)
    {
        if (cardPrefab == null || deck == null || deck.Count == 0)
        {
            Debug.LogError("Card prefab or deck not assigned!");
            return;
        }

        ClearBoard();

        int cols = GameManager.Instance.cols;
        int rows = GameManager.Instance.rows;
        int totalCards = deck.Count;

        // Card size at scale 1
        SpriteRenderer sr = cardPrefab.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Card prefab missing SpriteRenderer!");
            return;
        }
        Vector2 spriteSize = sr.bounds.size;

        float spacingX = spacing;
        float spacingY = spacing;

        // Screen usable size
        float screenHeight = mainCam.orthographicSize * 2f - padding * 2f;
        float screenWidth = screenHeight * mainCam.aspect - padding * 2f;

        // Total grid size at scale 1
        int maxCols = Mathf.Min(cols, totalCards);
        float gridWidth = maxCols * spriteSize.x + (maxCols - 1) * spacingX;
        float gridHeight = rows * spriteSize.y + (rows - 1) * spacingY;

        // Calculate scale factor to fit screen
        float scaleX = 1f, scaleY = 1f;
        if (gridWidth > screenWidth)
            scaleX = screenWidth / gridWidth;
        if (gridHeight > screenHeight)
            scaleY = screenHeight / gridHeight;

        float finalScale = Mathf.Min(scaleX, scaleY, 1f); // never scale up, only down

        // Place cards row by row
        int cardIndex = 0;
        for (int r = 0; r < rows; r++)
        {
            int cardsInRow = Mathf.Min(cols, totalCards - r * cols);
            float rowWidth = cardsInRow * spriteSize.x * finalScale + (cardsInRow - 1) * spacingX * finalScale;
            float rowStartX = -rowWidth / 2f + (spriteSize.x * finalScale) / 2f;
            float y = (rows / 2f - r - 0.5f) * (spriteSize.y * finalScale + spacingY * finalScale);

            for (int c = 0; c < cardsInRow; c++)
            {
                if (cardIndex >= totalCards) break;

                int cardNumber = deck[cardIndex];
                GameObject newCard = Instantiate(cardPrefab, parent);
                newCard.name = $"Card_{cardNumber}_{cardIndex}";

                float x = rowStartX + c * (spriteSize.x * finalScale + spacingX * finalScale);
                newCard.transform.localPosition = new Vector3(x, y, 0f);

                newCard.transform.localScale = new Vector3(finalScale, finalScale, 1f);

                Card card = newCard.GetComponent<Card>();
                if (card != null)
                    card.Initialize(cardNumber);

                spawnedCards.Add(newCard);
                cardIndex++;
            }
        }
    }

}
