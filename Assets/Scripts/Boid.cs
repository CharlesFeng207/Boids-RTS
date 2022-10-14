using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;

    // State
    public Vector3 targetForce = Vector3.zero;
    public Vector3 seperationForce = Vector3.zero;
    public Vector3 cohesionForce = Vector3.zero;
    
    Vector3 velocity;

    // To update:
    // Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 seperateHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    public int state;

    // Cached
    Material material;
    Transform target;

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
    }

    public void Initialize (BoidSettings settings, Transform target) {
        this.target = target;
        this.settings = settings;
    }

    public void SetColour (Color col) {
        if (material != null) {
            material.color = col;
        }
    }

    public void UpdateBoid () {
        velocity = Vector3.zero;
        targetForce = Vector3.zero;
        seperationForce = Vector3.zero;
        cohesionForce = Vector3.zero;
     
        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;
            
            Vector3 offsetToFlockmatesCentre = Vector3.zero;
            offsetToFlockmatesCentre = (centreOfFlockmates - transform.position);

            // var alignmentForce = SteerTowards (avgFlockHeading) * settings.alignWeight;
            cohesionForce = offsetToFlockmatesCentre * settings.cohesionWeight;
            seperationForce = seperateHeading * settings.seperateWeight;
            

            // acceleration += alignmentForce;
            // acceleration += cohesionForce;
        }

        // if (targetForce != Vector3.zero && seperationForce != Vector3.zero)
        // {
        //     if ( Vector3.Dot(targetForce, seperationForce) > 0)
        //     {
        //         velocity = seperationForce + targetForce;
        //     }
        // }
        // else
        // {
        //     velocity = seperationForce + targetForce;
        // }
        
        if (target != null) {
            Vector3 offsetToTarget = (target.position - transform.position);
            if (Vector3.Magnitude(offsetToTarget) > settings.targetMaxDistance)
                targetForce = offsetToTarget * settings.targetWeight;
        }


        // if (state == 0)
        // {
        //     if (Vector3.Magnitude(cohesionForce) > settings.cohesionMaxDistance)
        //     {
        //         state = 1;
        //     }
        //     velocity = targetForce + seperationForce;
        // }
        // else
        // {
        //     if (Vector3.Magnitude(cohesionForce) < settings.cohesionMinDistance)
        //     {
        //         state = 0;
        //     }
        //     
        //     velocity = targetForce + cohesionForce + seperationForce;    
        // }
        
        velocity = targetForce + cohesionForce + seperationForce;    
        
        if (Vector3.Magnitude(velocity) < settings.minSpeed)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = Vector3.ClampMagnitude(velocity, settings.maxSpeed);
        }

        // if (IsHeadingForCollision ()) {
        //     Vector3 collisionAvoidDir = ObstacleRays ();
        //     Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * settings.avoidCollisionWeight;
        //     velocity += collisionAvoidForce;
        // }

        // velocity += acceleration * Time.deltaTime;
        // float speed = velocity.magnitude;
        // Vector3 dir = velocity / speed;
        // speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        // velocity = dir * speed;
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
    }

    // bool IsHeadingForCollision () {
    //     RaycastHit hit;
    //     if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
    //         return true;
    //     } else { }
    //     return false;
    // }
    //
    // Vector3 ObstacleRays () {
    //     Vector3[] rayDirections = BoidHelper.directions;
    //
    //     for (int i = 0; i < rayDirections.Length; i++) {
    //         Vector3 dir = cachedTransform.TransformDirection (rayDirections[i]);
    //         Ray ray = new Ray (position, dir);
    //         if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
    //             return dir;
    //         }
    //     }
    //
    //     return Vector3.zero;
    // }

    Vector3 SteerTowards (Vector3 vector)
    {
        // return vector;
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (vector, settings.maxSteerForce);
    }

}