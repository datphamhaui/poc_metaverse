using Fusion;
using Micosmo.SensorToolkit.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBloomOrb : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [SerializeField] GameObject _explosionFX;
    [SerializeField] private GameObject _bloomOrbVisual;

    [Networked(OnChanged = nameof(OnDestroyedChanged))]
    public NetworkBool networkedDestroyed { get; set; }
    private bool _predictedDestroyed;
    private bool destroyed
    {
        get => Object.IsPredictedSpawn ? _predictedDestroyed : (bool)networkedDestroyed;
        set { if (Object.IsPredictedSpawn) _predictedDestroyed = value; else networkedDestroyed = value; }
    }

    public void Init(Vector3 forward)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void Spawned()
    {

    }

    public static void OnDestroyedChanged(Changed<NetworkBehaviour> changed)
    {
        ((NetworkBloomOrb)changed.Behaviour)?.OnDestroyedChanged();
    }

    private void OnDestroyedChanged()
    {
        if (destroyed)
        {
            if (_explosionFX != null)
            {
                transform.up = Vector3.up;
                _explosionFX.SetActive(true);
            }
            _bloomOrbVisual.gameObject.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else if (life.RemainingTime(Runner) < 1.0f)
        {
            if (destroyed) return;
            destroyed = true;
        }
    }
}
