using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProductsManager : MonoBehaviour
{
    public static ProductsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Data")]
    public ProductDatabase productDatabase;
    public Dictionary<string, Texture2D> textureCache = new(); //Catch all dowloaded images.
    [SerializeField] private string jsonFileName = "products.json";

    [Header("Script references")]
    [SerializeField] private ProductVirtualGrid productCatalogueGrid;
    [SerializeField] private FilterBoxController filterBoxController;
    [SerializeField] private ProductDetails productDetailsPanel;
    [SerializeField] public Model3DViewHandler model3DViewHandler;

    //[Header("Model 3D view")]

    [Header("Others")]
    [SerializeField] Transform noProductsFoundTextObj;

    //[Header("Debug variables")]
    //[SerializeField] public List<Texture2D> imagesDebug = new List<Texture2D>();

    private void Start()
    {
        StartCoroutine(LoadProducts());
    }

    #region LoadProducts
    private IEnumerator LoadProducts()
    {
        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            jsonFileName
        );

        using UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load product data: {request.error}");
            yield break;
        }

        string json = request.downloadHandler.text;

        Debug.Log("JSON Data:"+ json);

        ProductWrapper wrapper = JsonUtility.FromJson<ProductWrapper>(json);

        if (wrapper == null || wrapper.products == null)
        {
            Debug.LogError("Failed to parse product data.");
            yield break;
        }

        ProductData[] products = wrapper.products;

        Debug.Log($"Loaded {products.Length} products.");

        productDatabase = new ProductDatabase(products);

        #region Debug Area
        DisplayProductsInCatalogue(new List <ProductData> (products));
        filterBoxController.UpdateFilterBoxWithFilterData(productDatabase.availableCategories);
       /* foreach (ProductData product in products)
        {
            LoadImage(ResolveUrl(product.thumbnailUrl), (image) => { });
        }*/
        #endregion
    }

    [Serializable]
    private class ProductWrapper
    {
        public ProductData[] products;
    }
    #endregion

    #region TextureCache
    public void LoadImage(string url, Action<Texture2D> onComplete)
    {
        // 1. Check in-memory cache first
        if (textureCache.TryGetValue(url, out Texture2D cachedTexture))
        {
            onComplete?.Invoke(cachedTexture);
            return;
        }
        // 2. Not cached: fetch at runtime via UnityWebRequest
        StartCoroutine(DownloadTextureRoutine(url, onComplete));
    }
    private IEnumerator DownloadTextureRoutine(string url, Action<Texture2D> onComplete)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Store in cache for future cards
            textureCache[url] = texture;
            
            onComplete?.Invoke(texture);

            //imagesDebug.Add(texture); //Storing for debug purpose.
        }
        else
        {
            Debug.LogWarning($"Failed to download image from {url}: {request.error}");
            onComplete?.Invoke(null);
        }
    }

    public string ResolveUrl(string rawUrl)
    {
        if (rawUrl.StartsWith("http://") || rawUrl.StartsWith("https://"))
        {
            return rawUrl;
        }
        string localPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Thumbnails", rawUrl);
#if UNITY_ANDROID && !UNITY_EDITOR
    return localPath;
#else
        // Replace Windows backslashes with forward slashes for UnityWebRequest URI format
        return "file:///" + localPath.Replace("\\", "/");
#endif
    }
    #endregion

    #region OnProductCardClick
    public void OnProductCardClick(ProductData productData)
    {
        productDetailsPanel.BindAndShow(productData, (ele) => { model3DViewHandler.Show3DModel(ele); });
    }
    #endregion

    #region ProductCatalogue
    public void DisplayProductsInCatalogue(List<ProductData> products)
    {
        noProductsFoundTextObj.gameObject.SetActive(!(products.Count > 0));

        productCatalogueGrid.SetProducts(products);
        //Note: if there is no products(products.count == 0) show Empty text.
    }
    #endregion
}
