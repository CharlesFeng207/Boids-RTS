using System;
using UnityEngine;


public class Boid : MonoBehaviour
{
    BoidSettings settings;

    // To update:
    [HideInInspector] public Vector2 logicPos;
    [HideInInspector] public Vector2 velocity;

    [HideInInspector] public Vector2 seperateHeading;
    [HideInInspector] public Vector2 centreOfFlockmates;
    [HideInInspector] public int numPerceivedFlockmates;

    [HideInInspector] public Vector3 preFramePos;
    [HideInInspector] public int collisonDir;

    Material material;
    Transform target;


    public bool isPlayer;

    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.target = target;
        this.settings = settings;
    }

    public void SetColour(Color col)
    {
        if (material != null)
        {
            material.color = col;
        }
    }

    public void UpdateBoid()
    {
        velocity = Vector2.zero;
        var targetForce = Vector2.zero;
        var seperationForce = Vector2.zero;
        var cohesionForce = Vector2.zero;


        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector2 offsetToFlockmatesCentre = Vector2.zero;
            offsetToFlockmatesCentre = (centreOfFlockmates - logicPos);

            cohesionForce = offsetToFlockmatesCentre * settings.cohesionWeight;
            seperationForce = seperateHeading * settings.seperateWeight;
        }

        if (target != null)
        {
            var targetPos = target.position.ToLogicPos();
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
            velocity = HandleCollision(velocity, 2);
            velocity = Vector2.ClampMagnitude(velocity, settings.maxSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        var pos = logicPos.ToWorldPos();
        Gizmos.DrawRay(pos, velocity.ToWorldPos().normalized * settings.collisionAvoidDst);
        Gizmos.DrawWireSphere(pos, settings.boundsRadius);
    }

    private void Update()
    {
        var child = transform.GetChild(0);
        if (isPlayer)
        {
            logicPos = transform.position.ToLogicPos();

            var dir = transform.position - preFramePos;
            if (dir != Vector3.zero)
            {
                child.forward = dir.normalized * (settings.steerSpeed * 3f) + child.forward;
            }
        }
        else
        {
            logicPos += (velocity * Time.deltaTime);

            var prePos = transform.position;
            transform.position = Vector3.Lerp(transform.position, logicPos.ToWorldPos(), settings.trackSpeed);

            var dir = transform.position - prePos;
            if (dir != Vector3.zero)
            {
                child.forward = dir.normalized * settings.steerSpeed + child.forward;
            }
        }

        preFramePos = transform.position;
    }

    Vector2 HandleCollision(Vector2 dir, int loopCount)
    {
        if (loopCount == 0) return dir;
        RaycastHit hit;
        if (Physics.Raycast(logicPos.ToWorldPos(), dir.ToWorldPos(), out hit, settings.collisionAvoidDst,
            settings.obstacleMask))
        {
            var dir1 = Quaternion.Euler(0f, 90f, 0f) * hit.normal;
            var dir2 = Quaternion.Euler(0f, -90f, 0f) * hit.normal;

            if (collisonDir == 0)
            {
                var targetPos = target.position.ToLogicPos();
                var offsetToTarget = (targetPos - logicPos);
                var dotDir1 = Vector2.Dot(offsetToTarget, dir1.ToLogicPos());
                collisonDir = dotDir1 > 0f ? 1 : 2;
            }

            var newDir = collisonDir == 1 ? dir1 : dir2;
            return HandleCollision(newDir.ToLogicPos() * dir.magnitude, loopCount - 1);
        }

        if(loopCount == 2)
            collisonDir = 0;
        return dir;
    }

    // Vector3 ObstacleRays () {
    //     Vector3[] rayDirections = BoidHelper.directions;
    //
    //     for (int i = 0; i < rayDirections.Length; i++) {
    //         Vector3 dir = transform.TransformDirection (rayDirections[i]);
    //         if (!Physics.Raycast(logicPos.ToWorldPos(), dir, settings.collisionAvoidDst, settings.obstacleMask)) {
    //             return dir;
    //         }
    //     }
    //
    //     return Vector3.zero;
    // }
}