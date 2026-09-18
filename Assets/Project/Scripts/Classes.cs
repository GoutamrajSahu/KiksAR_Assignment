using System;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

[Serializable]
public class ProductDatabase
{
    public ProductData[] products;
    public List<string> availableCategories { get; private set; } = new(); //Storing all the possible categories available in the give product data.
    public List<string> subcategory = new() {"Male", "Female", "KidsBoy", "KidsGirl"}; //Keeping subcategory fixed as its same for all categories.
    public ProductDatabase(ProductData[] products)
    {
        this.products = products;

        // Extract unique categories (no duplicates, case-insensitive)
        HashSet<string> uniqueCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < this.products.Length; i++)
        {
            if (!string.IsNullOrEmpty(this.products[i].category))
            {
                uniqueCategories.Add(this.products[i].category.Trim());
            }
        }
        availableCategories = new List<string>(uniqueCategories);
    }
}

[Serializable]
public class ProductData
{
    public string productId;
    public string name;
    public string category;
    public string subcategory;
    public string description;
    public string thumbnailUrl;
    public string modelCategory;
}