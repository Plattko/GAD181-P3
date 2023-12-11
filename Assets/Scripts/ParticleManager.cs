using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private Transform p1Transform;
    [SerializeField] private Transform p2Transform;
    
    [SerializeField] private ParticleSystem p1SwapParticles;
    [SerializeField] private ParticleSystem p2SwapParticles;

    [SerializeField] private Transform swapTrailTransform;
    private ParticleSystem swapTrailParticles;

    // Start is called before the first frame update
    void Start()
    {
        swapTrailParticles = swapTrailTransform.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayJumpParticles(ParticleSystem particles, Transform transform)
    {
        particles.Stop();
        particles.transform.position = transform.position;
        particles.Play();
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

    public void PlaySwapTrailParticles()
    {
        // Set the particle system's position to the midpoint between the players
        swapTrailTransform.position = p1Transform.position + (p2Transform.position - p1Transform.position) / 2;

        // Scale the particle shape
        var shape = swapTrailParticles.shape;
        shape.scale = new Vector2((p2Transform.position - p1Transform.position).magnitude - 2.5f, shape.scale.y);

        // Set the number of particles in the burst
        var emission = swapTrailParticles.emission;
        var burst = new ParticleSystem.Burst(0, (ushort)Mathf.Clamp(Mathf.RoundToInt(shape.scale.x * 1.5f), 25, int.MaxValue));
        emission.SetBurst(0, burst);

        // Rotate the particle system
        Vector2 direction = p1Transform.position - swapTrailTransform.position;
        swapTrailTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        swapTrailParticles.Stop();
        swapTrailParticles.Play();
    }
}
