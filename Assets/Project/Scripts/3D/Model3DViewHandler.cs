using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Model3DViewHandler : MonoBehaviour
{
    //Note: I kept the 3D model viewer system hardcoaded for now, it can be expanded later.
    [SerializeField] List<GameObject> modelPrefabs;
    [SerializeField] Transform modelContainer;
    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject catalougeHUD;
    [SerializeField] Button exit3DViewButton;
    [SerializeField] Model3DInteraction model3DInteraction;
    public void Show3DModel(ProductData productData)
    {
        model3DInteraction.ResetModel(false);
        int productIndex = ProductsManager.Instance.productDatabase.availableCategories.IndexOf(productData.category);
        activeModel = Instantiate(modelPrefabs[productIndex]);
        activeModel.transform.SetParent(modelContainer);
        activeModel.transform.localPosition = Vector3.zero;
        activeModel.transform.localScale = new Vector3(1, 1, 1);
        activeModel.transform.localRotation = Quaternion.identity;

        gameObject.SetActive(true);
        catalougeHUD.SetActive(false);
        exit3DViewButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        model3DInteraction.ResetModel(false);
        Destroy(activeModel);
        gameObject.SetActive(false);
        catalougeHUD.SetActive(true);
        exit3DViewButton.gameObject.SetActive(false);
    }
}
