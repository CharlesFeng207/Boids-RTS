using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color colour;
    public GizmoType showSpawnRegion;

    private List<Boid> boids = new List<Boid>();

    [ContextMenu("RandomPos")]
    public void RandomPos()
    {
        foreach (var b in boids)
        {
            var randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = b.logicPos + randomCircle;
            b.logicPos = pos;
        }
    }

    // private List<Transform> 
    void Awake () {
        for (int i = 0; i < spawnCount; i++)
        {
            var randomCircle = Random.insideUnitCircle * spawnRadius;
            
            Boid boid = Instantiate (prefab);
            

            if (i == 0)
            {
                boid.gameObject.name = "Player";
                boid.isPlayer = true;
                boid.transform.localScale = Vector3.one * 2;
                boid.SetColour (Color.green);
                boid.logicPos = Vector2.zero;
            }
            else
            {
                boid.logicPos = randomCircle;
                boid.SetColour (colour);
            }

            boid.transform.position = boid.logicPos.ToWorldPos();

            boids.Add(boid);
        }
    }

    private void OnDrawGizmos () {
        if (showSpawnRegion == GizmoType.Always) {
            DrawGizmos ();
        }
    }

    void OnDrawGizmosSelected () {
        if (showSpawnRegion == GizmoType.SelectedOnly) {
            DrawGizmos ();
        }
    }

    void DrawGizmos () {

        Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere (transform.position, spawnRadius);
    }

}