using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductVirtualGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    [Header("Grid")]
    [SerializeField] private float cardWidth = 200f;
    [SerializeField] private float cardHeight = 200f;
    [SerializeField] private float horizontalSpacing = 20f;
    [SerializeField] private float verticalSpacing = 20f;
    [SerializeField] private float horizontalPadding = 20f;
    [SerializeField] private float topPadding = 20f;
    [SerializeField] private float bottomPadding = 20f;

    [Header("Virtualization")]
    [SerializeField] private int extraRows = 1;

    private List<ProductData> products;

    private readonly Dictionary<int, ProductCard> activeCards = new();
    private readonly HashSet<int> loadingIndices = new();

    private int columns;
    private int totalRows;
    private int previousFirstRow = -1;

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScroll);
    }

    public void SetProducts(List<ProductData> products)
    {
        this.products = products;

        Rebuild();
    }

    private void Rebuild()
    {
        ReleaseAllCards();

        if (products == null || products.Count == 0)
        {
            content.sizeDelta = new Vector2(scrollRect.viewport.rect.width,0f);
            return;
        }

        columns = CalculateColumns();

        totalRows = Mathf.CeilToInt((float)products.Count / columns);

        UpdateContentSize();

        previousFirstRow = -1;

        UpdateVisibleCards(true);
    }

    private int CalculateColumns()
    {
        float availableWidth = scrollRect.viewport.rect.width - horizontalPadding * 2f;

        int columnCount = Mathf.FloorToInt((availableWidth + horizontalSpacing)/(cardWidth + horizontalSpacing));

        return Mathf.Max(1, columnCount);
    }

    private void UpdateContentSize()
    {
        float width = scrollRect.viewport.rect.width;

        float height = topPadding + totalRows * cardHeight + Mathf.Max(0, totalRows - 1) * verticalSpacing + bottomPadding;

        content.sizeDelta = new Vector2(width,height);
    }

    private void OnScroll(Vector2 _)
    {
        UpdateVisibleCards(false);
    }

    private void UpdateVisibleCards(bool force)
    {
        if (products == null || products.Count == 0)
            return;

        float scrollY = Mathf.Max(0f,content.anchoredPosition.y);

        float rowHeight = cardHeight + verticalSpacing;

        int firstVisibleRow = Mathf.FloorToInt(scrollY / rowHeight);

        firstVisibleRow = Mathf.Max(0,firstVisibleRow - extraRows);

        if (!force && firstVisibleRow == previousFirstRow)
        {
            return;
        }

        previousFirstRow = firstVisibleRow;

        int visibleRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / rowHeight);

        int lastVisibleRow = firstVisibleRow + visibleRowCount + extraRows;

        lastVisibleRow = Mathf.Min(lastVisibleRow, totalRows - 1);

        int firstIndex = firstVisibleRow * columns;

        int lastIndex = Mathf.Min(products.Count - 1, ((lastVisibleRow + 1) * columns) - 1);

        UpdateCards(firstIndex, lastIndex);
    }

    private void UpdateCards(int firstIndex, int lastIndex)
    {
        List<int> cardsToRelease = new();

        foreach (KeyValuePair<int, ProductCard> pair in activeCards)
        {
            if (pair.Key < firstIndex || pair.Key > lastIndex)
            {
                cardsToRelease.Add(pair.Key);
            }
        }

        foreach (int index in cardsToRelease)
        {
            ReleaseCard(index);
        }

        for (int index = firstIndex; index <= lastIndex; index++)
        {
            if (!activeCards.ContainsKey(index))
            {
                CreateCard(index);
            }
        }
    }

    private void CreateCard(int index)
    {
        ProductCard card = ProductCardPool.Instance.Get();

        card.transform.SetParent(content, false);

        RectTransform rect = card.transform as RectTransform;

        rect.anchorMin = new Vector2(0f, 1f);

        rect.anchorMax = new Vector2(0f, 1f);

        rect.pivot = new Vector2(0f, 1f);

        int row = index / columns;
        int column = index % columns;

        rect.anchoredPosition = new Vector2(GetXPosition(column), GetYPosition(row));

        rect.sizeDelta = new Vector2(cardWidth, cardHeight);

        ProductData product = products[index];

        card.Bind(product, productData => { ProductsManager.Instance.OnProductCardClick(productData); });

        activeCards.Add(index, card);
    }

    private float GetXPosition(int column)
    {
        return horizontalPadding + column * (cardWidth + horizontalSpacing);
    }

    private float GetYPosition(int row)
    {
        return -topPadding - row * (cardHeight + verticalSpacing);
    }

    private void ReleaseCard(int index)
    {
        if (!activeCards.TryGetValue(index, out ProductCard card))
        {
            return;
        }

        activeCards.Remove(index);
        loadingIndices.Remove(index);

        ProductCardPool.Instance.Return(card);
    }

    private void ReleaseAllCards()
    {
        List<int> indices = new(activeCards.Keys);

        foreach (int index in indices)
        {
            ReleaseCard(index);
        }

        loadingIndices.Clear();
    }
}