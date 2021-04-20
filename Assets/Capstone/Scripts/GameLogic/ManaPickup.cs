using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
[RequireComponent(typeof(NetworkTransform))]
public class ManaPickup : NetworkBehaviour
{

    [Serializable]
    public struct ColorLinker
    {
        public float minValue;
        public float maxValue;
        public float sizeMultiplier;
        public Color color;
    }


    public Rigidbody Rigidbody;
    public List<ColorLinker> ColorLinkers = new List<ColorLinker>();
    [SyncVar(hook=nameof(ChangedValue))]
    public float Value;

    [ServerCallback]
    void Awake()
    {
        if (Rigidbody == null)
            Debug.LogError("No Rigidbody!", this);

        Vector3 explosion = transform.position;
        explosion.x = explosion.x + UnityEngine.Random.Range(-2f, 2f);
        explosion.z = explosion.z + UnityEngine.Random.Range(-2f, 2f);

        Rigidbody.AddExplosionForce(200f, explosion, 10f, 10f);

    }

    [ServerCallback]
    public void SetValue(float value)
    {
        Value = value;
    }

    private void ChangedValue(float oldValue, float newValue)
    {
        UpdateManaShape();
    }

    private void UpdateManaShape()
    {
        foreach (ColorLinker colorlink in ColorLinkers)
        {
            if (Value >= colorlink.minValue && Value <= colorlink.maxValue)
            {
                transform.localScale = transform.localScale * colorlink.sizeMultiplier;
                MeshRenderer meshrend = GetComponentInChildren<MeshRenderer>();
                meshrend.material.color = colorlink.color;
                break;
            }
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerInterface player))
            return;

        if (ClientMoneyController.singleton.Money + Value > ClientMoneyController.singleton.MaxValue)
            return;

        ClientMoneyController.singleton.RemoveMoney(-Value);
        ManaDropper.singleton.DestroyMana(gameObject);
    }

}
