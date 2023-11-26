using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private Transform p1Transform;
    [SerializeField] private Transform p2Transform;
    
    [SerializeField] private ParticleSystem p1SwapParticles;
    [SerializeField] private ParticleSystem p2SwapParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayDoubleJumpParticles(List<ParticleSystem> particles, Transform transform)
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
            particle.transform.position = transform.position;
            particle.Play();
        }
    }

    public void PlaySwapParticles()
    {
        p1SwapParticles.Stop();
        p2SwapParticles.Stop();

        p1SwapParticles.Play();
        p2SwapParticles.Play();
    }
}
