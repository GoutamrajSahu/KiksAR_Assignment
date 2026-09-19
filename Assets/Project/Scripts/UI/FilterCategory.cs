using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FilterCategory : MonoBehaviour
{
    [SerializeField] Transform subcategoriesObj;

    [SerializeField] TextMeshProUGUI filterCategoryLabel;
    [SerializeField] public Toggle categoryToggle;
    [SerializeField] public Toggle subcategoryToggle_Male;
    [SerializeField] public Toggle subcategoryToggle_Female;
    [SerializeField] public Toggle subcategoryToggle_KidsBoy;
    [SerializeField] public Toggle subcategoryToggle_KidsGirl;

    [SerializeField] public string categoryName;

    private void OnEnable()
    {
        categoryToggle.onValueChanged.AddListener(OnCategoryToggleChanged);
    }

    private void OnDisable()
    {
        categoryToggle.onValueChanged.RemoveListener(OnCategoryToggleChanged);
    }

    private void OnCategoryToggleChanged(bool isOn)
    {
        subcategoriesObj.gameObject.SetActive(isOn);
        RectTransform rectT = transform.GetComponent<RectTransform>();
        if (isOn)
        {
            rectT.DOSizeDelta(new Vector2(rectT.sizeDelta.x, 203),0.1f);
        }
        else
        {
            rectT.DOSizeDelta(new Vector2(rectT.sizeDelta.x, 132), 0.1f);

            //If the category is false set subcategories to false as well.
            subcategoryToggle_Male.isOn = false;
            subcategoryToggle_Female.isOn = false;
            subcategoryToggle_KidsBoy.isOn = false;
            subcategoryToggle_KidsGirl.isOn = false;
        }
    }

    public void UpdateThisWithData(string categoryName)
    {
        filterCategoryLabel.text = categoryName;
        this.categoryName = categoryName;
    }
}
