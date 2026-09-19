using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductCard : MonoBehaviour
{
    [Header("UI References")]

    [Tooltip("Alternative thumbnail display if UI.Image is used")]
    [SerializeField] private Image thumbnailPictureImage;

    [SerializeField] private TMP_Text productName;
    [SerializeField] private TMP_Text categoryLabel;
    [SerializeField] private TMP_Text subcategoryLabel;
    [SerializeField] private TMP_Text description;

    [Header("Interactive")]
    [SerializeField] private Button cardButton;

    // The data currently bound to this card
    public ProductData CurrentData { get; private set; }

    private Action<ProductData> onCardClicked;

    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(HandleCardClick);
        }
    }

    private void OnDestroy()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(HandleCardClick);
        }
    }

    // Binds product data to the UI elements and triggers lazy thumbnail loading.
    public void Bind(ProductData data, Action<ProductData> onClickedCallback = null)
    {
        CurrentData = data;
        onCardClicked = onClickedCallback;

        if (data == null)
        {
            Clear();
            return;
        }

        // Set text labels
        if (productName != null) productName.text = data.name;
        if (categoryLabel != null) categoryLabel.text = data.category;
        if (subcategoryLabel != null) subcategoryLabel.text = data.subcategory;
        if (description != null) description.text = data.description;

        // Reset image display while loading
        ResetThumbnailDisplay();

        // Lazy-load thumbnail image via ProductsManager
        if (!string.IsNullOrEmpty(data.thumbnailUrl) && ProductsManager.Instance != null)
        {
            string resolvedUrl = ProductsManager.Instance.ResolveUrl(data.thumbnailUrl);

            ProductsManager.Instance.LoadImage(resolvedUrl, (texture) =>
            {
                // Verify the card hasn't been recycled for a different product in the meantime
                if (CurrentData != data || texture == null) return;

                ApplyTexture(texture);
            });
        }
    }

    // Clears the card UI to an empty state.
    public void Clear()
    {
        CurrentData = null;
        onCardClicked = null;

        if (productName != null) productName.text = string.Empty;
        if (categoryLabel != null) categoryLabel.text = string.Empty;
        if (subcategoryLabel != null) subcategoryLabel.text = string.Empty;
        if (description != null) description.text = string.Empty;

        ResetThumbnailDisplay();
    }

    private void ResetThumbnailDisplay()
    {
        if (thumbnailPictureImage != null)
        {
            thumbnailPictureImage.sprite = null;
            thumbnailPictureImage.enabled = false;
        }
    }

    private void ApplyTexture(Texture2D texture)
    {
        if (thumbnailPictureImage != null)
        {
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            thumbnailPictureImage.sprite = sprite;
            thumbnailPictureImage.enabled = true;
        }
    }

    private void HandleCardClick()
    {
        if (CurrentData != null)
        {
            onCardClicked?.Invoke(CurrentData);
        }
    }
}
