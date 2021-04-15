using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MaterialChanger))]
[RequireComponent(typeof(Tower))]
public class TowerBuildingController : MonoBehaviour
{

    public MaterialChanger materialChanger;
    public Tower tower;
    public UISliderFloatIndicator progressionSlider;

    public List<GameObject> EnableWhileBuilding = new List<GameObject>();
    public Material BuildingMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        materialChanger = GetComponent<MaterialChanger>();

        if (TryGetComponent(out Tower tower))
            this.tower = tower;

        if (progressionSlider == null)
            Debug.LogError("No progession slider found!", this);

        if (BuildingMaterial == null)
            Debug.LogError("No Material Set", this);

        enabled = false;
    }

    private void OnEnable()
    {
        SetObjectsState(true);

        StartBuilding();
    }

    private void OnDisable()
    {
        SetObjectsState(false);
        materialChanger.RevertMaterial();
    }

    private void StartBuilding()
    {
        materialChanger.ChangeMaterial(BuildingMaterial);
        materialChanger.ChangeOpaquicity(1, tower.BuildTime);
        progressionSlider.SetMaxValue(tower.MaxHealth);
        progressionSlider.SetValue(0);
        progressionSlider.SetValueOverTime(tower.MaxHealth, tower.BuildTime);
    }

    private void SetObjectsState(bool isEnabled)
    {
        foreach (GameObject gameobj in EnableWhileBuilding)
        {
            gameobj.SetActive(isEnabled);
        }
    }
}
