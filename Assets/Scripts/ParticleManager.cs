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

    public void PlayDoubleJumpParticles(ParticleSystem particles, Transform transform)
    {
        particles.Stop();
        particles.transform.position = transform.position;
        particles.Play();
    }

    public void PlaySwapParticles()
    {
        p1SwapParticles.Stop();
        p2SwapParticles.Stop();

        //p1SwapParticles.transform.position = p1Transform.position;
        //p2SwapParticles.transform.position = p2Transform.position;

        p1SwapParticles.Play();
        p2SwapParticles.Play();
    }
}
