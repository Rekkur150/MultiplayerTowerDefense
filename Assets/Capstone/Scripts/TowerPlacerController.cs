using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(TowerPlacer))]
public class TowerPlacerController : NetworkBehaviour
{
    public TowerPlacer towerPlacer;
    public TextMeshProUGUI TowerErrorText;
    public UISliderFloatIndicator BuildingProgressSlider;

    private IEnumerator WaitForPlacingTowerCoroutine;
    private IEnumerator PlaceTowerError;

    private void Awake()
    {
        if (TryGetComponent(out TowerPlacer towerPlacer))
            this.towerPlacer = towerPlacer;

        if (TowerErrorText == null)
            Debug.LogError("This doesn't have text mesh pro!", this);

        TowerErrorText.text = "";

        if (TryGetComponent(out UISliderFloatIndicator BuildingProgressSlider))
            this.BuildingProgressSlider = BuildingProgressSlider;

        towerPlacer.OnStartBuildingATower += new TowerPlacer.MyTowerEventHandler(StartedBuilding);
        towerPlacer.OnCancelingBuildingATower += new TowerPlacer.MyTowerEventHandler(FinishedBuilding);
        towerPlacer.OnFinishedBuildingATower += new TowerPlacer.MyTowerEventHandler(FinishedBuilding);
    }

    private void Update()
    {
        if (hasAuthority)
        {
            SelectTower();

            if (Input.GetButtonDown("Fire1"))
            {
                towerPlacer.ClientEnableDisabledTower();

                string ErrorReturn = towerPlacer.ErrorPlacement();

                if (ErrorReturn != default(string))
                    PutError(ErrorReturn);
            }



            if (Input.GetButtonDown("Cancel"))
                towerPlacer.ClientCancelSpawning();
        }
    }

    private void SelectTower()
    {

        if (Input.GetButtonDown("Tower1"))
        {

            towerPlacer.ClientCancelSpawning();

            if (towerPlacer.GetPrefabTowerIndex() == 0)
                return;

            SummonTower(0);

        }
        else if (Input.GetButtonDown("Tower2"))
        {
            towerPlacer.ClientCancelSpawning();

            if (towerPlacer.GetPrefabTowerIndex() == 1)
                return;

            SummonTower(1);
        }
        else if (Input.GetButtonDown("Tower3"))
        {
            towerPlacer.ClientCancelSpawning();

            if (towerPlacer.GetPrefabTowerIndex() == 2)
                return;

            SummonTower(2);
        }
        else if (Input.GetButtonDown("Tower4"))
        {
            towerPlacer.ClientCancelSpawning();

            if (towerPlacer.GetPrefabTowerIndex() == 3)
                return;

            SummonTower(3);
        }
        else if (Input.GetButtonDown("Tower5"))
        {
            towerPlacer.ClientCancelSpawning();

            if (towerPlacer.GetPrefabTowerIndex() == 4)
                return;

            SummonTower(4);
        }
    }


    private void SummonTower(int tower)
    {
        if (WaitForPlacingTowerCoroutine != null)
            StopCoroutine(WaitForPlacingTowerCoroutine);

        WaitForPlacingTowerCoroutine = WaitToSummonTower(tower);

        StartCoroutine(WaitForPlacingTowerCoroutine);
    }

    private IEnumerator WaitToSummonTower(int tower)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!towerPlacer.HasPrefabTower())
                break;
        }

        towerPlacer.ClientSpawnDisabledTower(tower);
    }

    private void PutError(string error)
    {
        if (PlaceTowerError != null)
            StopCoroutine(PlaceTowerError);

        PlaceTowerError = PutErrorWait(error);

        StartCoroutine(PlaceTowerError);
    }

    private IEnumerator PutErrorWait(string error)
    {
        TowerErrorText.text = error;
        yield return new WaitForSeconds(3);
        TowerErrorText.text = "";
    }

    private void StartedBuilding(TowerInterface tower)
    {
        BuildingProgressSlider.gameObject.SetActive(true);
        BuildingProgressSlider.SetValueOverTime(1f, tower.tower.BuildTime);
    }

    private void FinishedBuilding(TowerInterface tower)
    {
        BuildingProgressSlider.StopValueOverTime();
        BuildingProgressSlider.SetValue(0f);
        BuildingProgressSlider.gameObject.SetActive(false);
    }
}
