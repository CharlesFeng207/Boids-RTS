using System;
using UnityEngine;

public class Boid : MonoBehaviour {

    BoidSettings settings;

    // To update:
    [HideInInspector]
    public Vector2 logicPos;
    [HideInInspector]
    public Vector2 velocity;
    
    [HideInInspector]
    public Vector2 seperateHeading;
    [HideInInspector]
    public Vector2 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;
    
    [HideInInspector]
    public Vector3 preFramePos;

    Material material;
    Transform target;
    
    [HideInInspector]
    public Vector3 collisionAvoidDir;

    public bool isPlayer;
    
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
        velocity = Vector2.zero;
        collisionAvoidDir = Vector2.zero;
        var targetForce = Vector2.zero;
        var seperationForce = Vector2.zero;
        var cohesionForce = Vector2.zero;
        
     
        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;
            
            Vector2 offsetToFlockmatesCentre = Vector2.zero;
            offsetToFlockmatesCentre = (centreOfFlockmates - logicPos);

            cohesionForce = offsetToFlockmatesCentre * settings.cohesionWeight;
            seperationForce = seperateHeading * settings.seperateWeight;
        }

        if (target != null) {
            var targetPos = new Vector2(target.position.x,  target.position.z);
            var offsetToTarget = (targetPos - logicPos);
            if (offsetToTarget.magnitude > settings.targetMaxDistance)
                targetForce = offsetToTarget * settings.targetWeight;
        }
        
        velocity = targetForce + cohesionForce + seperationForce;
        
        if (velocity.magnitude < settings.minSpeed)
        {
            velocity = Vector2.zero;
        }
        else
        {
            // if (IsHeadingForCollision ()) {
            //     collisionAvoidDir = ObstacleRays ();
            //     
            //     var collisionAvoidForce = collisionAvoidDir * settings.avoidCollisionWeight;
            //     velocity += new Vector2(collisionAvoidForce.x, collisionAvoidForce.z);
            // }
            
            velocity = Vector2.ClampMagnitude(velocity, settings.maxSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        var pos = new Vector3(logicPos.x, 0, logicPos.y);
        // if (collisionAvoidDir != Vector3.zero)
        // {
        //     Gizmos.DrawRay(pos, collisionAvoidDir.normalized);
        // }
        
        Gizmos.DrawWireSphere(pos, settings.boundsRadius);
    }
    
    

    private void Update()
    {
        var child = transform.GetChild(0);
        if (isPlayer)
        {
            logicPos = new Vector2(transform.position.x, transform.position.z);
            
            var dir = transform.position - preFramePos;
            if (dir != Vector3.zero)
            {
                child.forward = dir.normalized * (settings.steerSpeed * 3f) + child.forward;
            }  
        }
        else
        {
            logicPos += (velocity * Time.deltaTime);
        
            var renderPos = new Vector3(logicPos.x, 0, logicPos.y);
            var prePos = transform.position;
            transform.position = Vector3.Lerp(transform.position, renderPos, settings.trackSpeed);

            var dir = transform.position - prePos;
            if (dir != Vector3.zero)
            {
                child.forward = dir.normalized * settings.steerSpeed + child.forward;
            }  
        }

        preFramePos = transform.position;
    }

    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.Raycast (logicPos, velocity, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }
    
    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions;
    
        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = transform.TransformDirection (rayDirections[i]);
            if (!Physics.Raycast(logicPos, dir, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }
    
        return Vector3.zero;
    }

}