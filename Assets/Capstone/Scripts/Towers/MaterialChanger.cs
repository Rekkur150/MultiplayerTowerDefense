using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public struct MaterialLinker
    {
        public Renderer meshRenderer;
        public Material material;
    }

    private List<MaterialLinker> MaterialLinkers;
    public List<MeshRenderer> IgnoredMeshes = new List<MeshRenderer>();

    protected void Awake()
    {
        GetDefaultMaterial();
    }

    private void GetDefaultMaterial()
    {
        if (MaterialLinkers != null)
            return;

        List<Renderer> Renderers = new List<Renderer>();

        MeshRenderer[] MeshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] SkinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (MeshRenderer mr in MeshRenderers)
        {
            Renderers.Add(mr);
        }

        foreach (SkinnedMeshRenderer smr in SkinnedMeshRenderers)
        {
            Renderers.Add(smr);
        }

        List<MaterialLinker> materialLinkers = new List<MaterialLinker>();

        foreach (Renderer meshrenderer in Renderers)
        {
            MaterialLinker materialLinker = new MaterialLinker();
            materialLinker.meshRenderer = meshrenderer;
            materialLinker.material = meshrenderer.material;
            materialLinkers.Add(materialLinker);
        }

        MaterialLinkers = materialLinkers;

    }

    public void ChangeMaterial(Material tochange)
    {
        if (MaterialLinkers == null)
            GetDefaultMaterial();

        foreach (MaterialLinker materialLinker in MaterialLinkers)
        {
            materialLinker.meshRenderer.material = tochange;
        }
    }


    public void RevertMaterial()
    {
        if (MaterialLinkers == null)
            return;

        foreach (MaterialLinker materialLinker in MaterialLinkers)
        {
            materialLinker.meshRenderer.material = materialLinker.material;
        }
    }

    public void ChangeOpaquicity(float opaquicity, float Time)
    {
        StartCoroutine(MakeOpapueHelper(opaquicity, Time));
    }

    private IEnumerator MakeOpapueHelper(float opaquicity, float Time)
    {
        if (MaterialLinkers.Count == 0)
            yield break;

        float DefaultTransparency = MaterialLinkers[0].meshRenderer.material.color.a;
        float Rate = (opaquicity - DefaultTransparency) / Time;


        for (float i = 0.01f; i< Time; i = i + 0.1f)
        {
            foreach (MaterialLinker materialLinker in MaterialLinkers)
            {
                Color newColor = new Color();
                newColor = materialLinker.meshRenderer.material.color;
                newColor.a = DefaultTransparency + Rate * i;

                materialLinker.meshRenderer.material.color = newColor;

            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
