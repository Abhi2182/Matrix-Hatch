using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Card Data")]
    public int cardID; // number for matching (1,2,3,...)
    public TMP_Text idText; // Text component on the front
    public Sprite backSprite; // back image for hidden state
    public Sprite frontSprite; // front image for shown state

    [Header("Components")]
    [SerializeField]private SpriteRenderer spriteRenderer;
    [SerializeField]private BoxCollider2D boxCollder;

    [Header("Flip Animation Settings")]
    public float flipSpeed = 400f; // rotation speed in degrees per second
    private Quaternion faceRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion backRotation = Quaternion.Euler(0, 180, 0);

    [HideInInspector] public bool isFaceUp = false;
    [HideInInspector] public bool isMatched = false;
    [HideInInspector] public bool isFlipping = false;

    private void Awake()
    {
        boxCollder.enabled = false;
    }
    public void Initialize(int number )
    {
        cardID = number;
        if (idText != null)
        {
            idText.text = cardID.ToString();
            idText.enabled = true;
        }

        // reset rotation and state
        transform.rotation = faceRotation;
        spriteRenderer.sprite = frontSprite;
        isFaceUp = true;
        isMatched = false;

        // Start coroutine for delayed back display
        StartCoroutine(ShowBackWithDelay(GameManager.Instance.showCardBackDelay));
    }
    private IEnumerator ShowBackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBack();
    }
    public void ShowBack()
    {
        isFaceUp = false;
        isMatched = false;
        isFlipping = false;
        spriteRenderer.sprite = backSprite;
        boxCollder.enabled = true;
        if (idText != null) idText.enabled = false;
        transform.rotation = backRotation;
    }

    public void TryFlip()
    {
        if (isMatched || isFlipping) return;
        StartCoroutine(FlipCoroutine());
    }
    
    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;

        float startY = isFaceUp ? 0f : 180f;
        float targetY = isFaceUp ? 180f : 0f;
        float duration = Mathf.Abs(targetY - startY) / flipSpeed;
        float elapsed = 0f;

        // halfway (90°) = change visible side
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentY = Mathf.Lerp(startY, targetY, elapsed / duration);
            transform.rotation = Quaternion.Euler(0, currentY, 0);

            // When crossing the halfway point, swap side
            if (!isFaceUp && currentY <= 90f)
            {
                // show face
                spriteRenderer.sprite = frontSprite; // clear back
                if (idText != null) idText.enabled = true;
            }
            else if (isFaceUp && currentY >= 90f)
            {
                // show back
                spriteRenderer.sprite = backSprite;
                if (idText != null) idText.enabled = false;
            }

            yield return null;
        }

        transform.rotation = isFaceUp ? backRotation : faceRotation;
        isFaceUp = !isFaceUp;
        isFlipping = false;

        if (isFaceUp)
            MatchChecker.Instance?.EnqueueCard(this);
        
    }

    public void FlipBackImmediate()
    {
        if (isMatched) return;
        StopAllCoroutines();
        StartCoroutine(FlipCoroutine());
    }

    public IEnumerator OnMatchedAnimCoroutine()
    {
        isMatched = true;
        float duration = 0.2f;
        Vector3 start = transform.localScale;
        Vector3 end = start * 1.1f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.CanPlayerFlip(this))
        {
            AudioManager.Instance?.PlayFlip(); // play card flip sound
            TryFlip();
        }
    }
}
