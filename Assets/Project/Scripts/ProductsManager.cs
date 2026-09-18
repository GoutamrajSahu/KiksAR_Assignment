using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductsManager : MonoBehaviour
{
    public static ProductsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public ProductDatabase productDatabase;
}
