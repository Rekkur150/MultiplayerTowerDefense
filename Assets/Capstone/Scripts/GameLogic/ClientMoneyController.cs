using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ClientMoneyController : NetworkBehaviour
{
    [SyncVar(hook = nameof(MoneyUpdate))]
    public float Money = 100f;
    public float DefaultMoney = 500f;
    public float MaxValue = 1000f;

    static public ClientMoneyController singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    [ServerCallback]
    private void Start()
    {
        SetMoney(DefaultMoney);
    }

    public override void OnStartClient()
    {
        MoneyUpdate(Money, Money);
    }


    [ServerCallback]
    public void SetMoney(float money)
    {
        Money = money;
    }

    [ServerCallback]
    public void RemoveMoney(float money)
    {
        Money = Money - money;
    }

    public void MoneyUpdate(float oldMoney, float newMoney)
    {
        if (UpdatedMoney != null)
            UpdatedMoney(newMoney);
    }

    public delegate void MoneyUpdateHandler(float money);
    public event MoneyUpdateHandler UpdatedMoney;
}
