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

    public ProductDatabase productDatabase;
    [SerializeField] private string jsonFileName = "products.json";

    private void Start()
    {
        StartCoroutine(LoadProducts());
    }

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
    }

    [Serializable]
    private class ProductWrapper
    {
        public ProductData[] products;
    }
}
