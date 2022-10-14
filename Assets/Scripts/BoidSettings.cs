using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float minSpeed;
    public float maxSpeed;
    public float steerSpeed;
    public float trackSpeed;
    public float perceptionRadius;
    public float seperateRadius;

    public float cohesionWeight;
    public float seperateWeight;
    public float targetWeight;
    public float avoidCollisionWeight;
    
    public LayerMask obstacleMask;
    public float boundsRadius;
    public float collisionAvoidDst;

    public float updateInterval;
    public float targetMaxDistance;

    public float timeScale;
}