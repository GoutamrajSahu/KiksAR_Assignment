using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FilterBoxController : MonoBehaviour
{
    public bool isFilterBoxOn;
    public Image bg;
    [SerializeField] Image iconImage;
    [SerializeField] List<Sprite> iconSprits;
    [SerializeField] GameObject filterCategoryPrefab;
    [SerializeField] Transform categoriesContainer;
    [SerializeField] List<FilterCategory> allFilterCategories;


    public void UpdateFilterBoxWithFilterData(List<string> availableCategories)
    {
        for (int i = 0; i < availableCategories.Count; i++)
        {
            GameObject categoryObj = Instantiate(filterCategoryPrefab);
            FilterCategory filterCategory = categoryObj.GetComponent<FilterCategory>();
            filterCategory.UpdateThisWithData(availableCategories[i]);
            categoryObj.transform.parent = categoriesContainer;
            categoryObj.transform.localScale = new Vector3(1, 1, 1);
            allFilterCategories.Add(filterCategory);
        }
    }

    #region Filter Functionality
    public void Filter()
    {
        //Create filter query
        List<ProductData> filteredProducts = new List<ProductData>();
        foreach (FilterCategory filterCategory in allFilterCategories)
        {
            if (filterCategory.categoryToggle.isOn)
            {
                //Find all products with this category.
                List<ProductData> tmp = new List<ProductData>(ProductsManager.Instance.productDatabase.products).FindAll(ele => ele.category == filterCategory.categoryName);
                List<ProductData> tmp2 = new List<ProductData>();
                //Filter the product according to selected subcategories.{"Male", "Female", "Kids-Boy", "Kids-Girl"}
                bool isSubcategorySelected = false;
                if (filterCategory.subcategoryToggle_Male.isOn)
                {
                    tmp2.AddRange(tmp.FindAll(ele => ele.subcategory == ProductsManager.Instance.productDatabase.subcategory[0]));
                    isSubcategorySelected = true;
                }
                if (filterCategory.subcategoryToggle_Female.isOn)
                {
                    tmp2.AddRange(tmp.FindAll(ele => ele.subcategory == ProductsManager.Instance.productDatabase.subcategory[1]));
                    isSubcategorySelected = true;
                }
                if (filterCategory.subcategoryToggle_KidsBoy.isOn)
                {
                    tmp2.AddRange(tmp.FindAll(ele => ele.subcategory == ProductsManager.Instance.productDatabase.subcategory[2]));
                    isSubcategorySelected = true;
                }
                if (filterCategory.subcategoryToggle_KidsGirl.isOn)
                {
                    tmp2.AddRange(tmp.FindAll(ele => ele.subcategory == ProductsManager.Instance.productDatabase.subcategory[3]));
                    isSubcategorySelected = true;
                }

                filteredProducts.AddRange(isSubcategorySelected ? tmp2 : tmp);//Add all from this category if any subcategory not selected.
            }
        }
        ProductsManager.Instance.DisplayProductsInCatalogue(filteredProducts);

        ToggleFilterPopup();
    }

    public void ResetFilter()
    {
        //Turn off all toggels
        foreach (FilterCategory filterCategory in allFilterCategories)
        {
            filterCategory.categoryToggle.isOn = false;
            filterCategory.subcategoryToggle_Male.isOn = false;
            filterCategory.subcategoryToggle_Female.isOn = false;
            filterCategory.subcategoryToggle_KidsBoy.isOn = false;
            filterCategory.subcategoryToggle_KidsGirl.isOn = false;
        }

        //Showing all products on reset.
        ProductsManager.Instance.DisplayProductsInCatalogue(new List<ProductData>(ProductsManager.Instance.productDatabase.products));

        ToggleFilterPopup();
    }
    #endregion

    #region PopUp visibility
    public void ToggleFilterPopup()
    {
        ShowFilterPopup(!isFilterBoxOn);
    }

    public void ShowFilterPopup(bool boolVal)
    {
        if (isFilterBoxOn != boolVal)
        {
            isFilterBoxOn = boolVal;

            if (boolVal)
            {
                GetComponent<RectTransform>().DOAnchorPosY(-243, 0.2f).OnComplete(() => { bg.gameObject.SetActive(true); });
            }
            else
            {
                GetComponent<RectTransform>().DOAnchorPosY(1569, 0.2f).OnComplete(() => { bg.gameObject.SetActive(false); });
            }
        }

        iconImage.sprite = boolVal ? iconSprits[0] : iconSprits[1];
    }
    #endregion
}
