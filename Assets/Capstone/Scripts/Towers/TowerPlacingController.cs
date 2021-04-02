using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]
[RequireComponent(typeof(MaterialChanger))]
[RequireComponent(typeof(ColliderChanger))]
public class TowerPlacingController : MonoBehaviour
{
    public Tower tower;
    public MaterialChanger materialChanger;
    public ColliderChanger colliderChanger;

    public Material PlaceableMaterial;
    public Material NotPlaceableMaterial;

    public GameObject attackRangePreview;

    // Start is called before the first frame update
    void Awake()
    {

        if (TryGetComponent(out Tower tower))
            this.tower = tower;

        if (TryGetComponent(out MaterialChanger materialChanger))
            this.materialChanger = materialChanger;

        if (TryGetComponent(out ColliderChanger colliderChanger))
            this.colliderChanger = colliderChanger;

        if (PlaceableMaterial == null)
            Debug.LogError("No placeable material!", this);

        if (NotPlaceableMaterial == null)
            Debug.LogError("No not placeable material!", this);

        if (attackRangePreview == null)
            Debug.LogError("No gameobject to reference range", this);

        enabled = false;
    }

    private void OnEnable()
    {
        Vector3 scaleWidth = attackRangePreview.transform.localScale / (attackRangePreview.GetComponent<MeshRenderer>().bounds.size.magnitude / 2);
        attackRangePreview.transform.localScale = scaleWidth * (tower.AreaFinder.AreaFinderCollider.bounds.size.magnitude / 2);
        attackRangePreview.SetActive(true);

        colliderChanger.ChangeCollider(false);
    }

    private void OnDisable()
    {
        attackRangePreview.SetActive(false);
        materialChanger.RevertMaterial();
        colliderChanger.RevertCollider();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TowerPlacer.CheckTowerPlacement(gameObject))
            materialChanger.ChangeMaterial(PlaceableMaterial);
        else materialChanger.ChangeMaterial(NotPlaceableMaterial);
    }
}
