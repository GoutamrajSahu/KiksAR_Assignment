using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;


public class FilterBoxController : MonoBehaviour
{
    public bool isFilterBoxOn;
    public Image bg;

    [SerializeField] GameObject filterCategoryPrefab;
    [SerializeField] Transform categoriesContainer;
    [SerializeField] List<FilterCategory> allFilterCategories;

    public void UpdateFilterBoxWithFilterData(List<string> availableCategories)
    {
        for (int i = 0; i < availableCategories.Count; i++)
        {
            GameObject categoryObj = Instantiate(filterCategoryPrefab);
            categoryObj.GetComponent<FilterCategory>().UpdateThisWithData(availableCategories[i]);
            categoryObj.transform.parent = categoriesContainer;
        }
    }

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
    }
    #endregion
}
