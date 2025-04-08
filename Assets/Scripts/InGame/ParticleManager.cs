using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public List<ParticleSystem> particles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EmitParticle(int idx)
    {
        //var emit = new ParticleSystem.EmitParams();
        particles[idx].Emit(10);
    }
}
