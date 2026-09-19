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
    [SerializeField] Toggle categoryToggle;
    [SerializeField] Toggle subcategoryToggle_Male;
    [SerializeField] Toggle subcategoryToggle_Female;
    [SerializeField] Toggle subcategoryToggle_KidsBoy;
    [SerializeField] Toggle subcategoryToggle_KidsGirl;

    [SerializeField] string categoryName;

    private void OnEnable()
    {
        categoryToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDisable()
    {
        categoryToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
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
        }
    }

    public void UpdateThisWithData(string categoryName)
    {
        filterCategoryLabel.text = categoryName;
    }
}
