using System.Collections.Generic;
using UnityEngine;

public class ProductCardPool : MonoBehaviour
{
    public static ProductCardPool Instance;

    public ProductCard cardPrefab;
    public int initialPoolSize = 16;
    public GameObject poolStorage;

    public Queue<ProductCard> availableCards = new Queue<ProductCard>();
    public List<ProductCard> activeCards = new List<ProductCard>();

    void Awake()
    {
        if (poolStorage == null)
        {
            poolStorage = gameObject;
        }

        Instance = this;

        PrewarmPool();
    }

    void PrewarmPool()
    {
        if (cardPrefab == null) return;

        for (int i = 0; i < initialPoolSize; i++)
        {
            ProductCard card = Instantiate(cardPrefab, poolStorage.transform);
            card.gameObject.SetActive(false);
            availableCards.Enqueue(card);
        }
    }

    public ProductCard Get(Transform targetParent = null)
    {
        ProductCard card;

        if (availableCards.Count > 0)
        {
            card = availableCards.Dequeue();
        }
        else
        {
            card = Instantiate(cardPrefab, poolStorage.transform);
        }

        if (targetParent != null)
        {
            card.transform.SetParent(targetParent, false);
        }

        card.gameObject.SetActive(true);
        activeCards.Add(card);
        return card;
    }

    public void Return(ProductCard card)
    {
        if (card == null) return;

        card.Clear();
        card.gameObject.SetActive(false);

        if (poolStorage != null)
        {
            card.transform.SetParent(poolStorage.transform, false);
        }

        activeCards.Remove(card);
        availableCards.Enqueue(card);
    }

    public void ReturnAll()
    {
        for (int i = activeCards.Count - 1; i >= 0; i--)
        {
            ProductCard card = activeCards[i];
            card.Clear();
            card.gameObject.SetActive(false);

            if (poolStorage != null)
            {
                card.transform.SetParent(poolStorage.transform, false);
            }

            availableCards.Enqueue(card);
        }

        activeCards.Clear();
    }
}
