using Fusion;
using System;
using  UnityEngine;

public sealed class NetworkCulling : NetworkBehaviour
{
    public Action<bool> updated;
    public bool isCulled => _isCulled;

    private float _radius = 2f;
    private int _tickRate;
    private bool _isCulled;

    public override sealed void Spawned()
    {
        _tickRate = Runner.Config.Simulation.TickRate;
        _isCulled = false;
    }

    public override sealed void Despawned(NetworkRunner runner, bool hasState)
    {
        _isCulled = false;

    }
    public override void FixedUpdateNetwork()
    {
        if (Runner == null || Runner.IsForward == false) return;

        var target = Object.InputAuthority;

        if (target.IsValid)
        {
            Runner.AddPlayerAreaOfInterest(target, transform.position, _radius);
        }

        int simulationTick = Runner.Simulation.Tick;
        bool isCulled = false;

        if (Object.IsProxy == true && Object.LastReceiveTick > 0)
        {
            int lastReceiveTickThreshold = simulationTick - _tickRate * 2;

            SimulationSnapshot serverState = Runner.Simulation.LatestServerState;

            if (serverState != null)
            {
                lastReceiveTickThreshold = serverState.Tick - _tickRate * 2;
            }

            if (Object.LastReceiveTick < lastReceiveTickThreshold - 1.90f)
            {
                isCulled = true;
            }
        }

        if (_isCulled != isCulled)
        {
            _isCulled = isCulled;

            if (updated != null)
            {
                try
                {
                    updated(isCulled);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}
