using UnityEngine;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    public Transform target;
    Boid[] boids;

    private float t;
    void Start () {
        boids = FindObjectsOfType<Boid> ();
        foreach (Boid b in boids) {
            b.Initialize (settings, target);
        }

    }

    void Update ()
    {
        Time.timeScale = settings.timeScale;
        t += Time.deltaTime;
        
        if (t > settings.updateInterval)
        {
            t = 0;

            if (boids != null)
            {
                int numBoids = boids.Length;
                var boidData = new BoidData[numBoids];

                for (int i = 0; i < boids.Length; i++)
                {
                    boidData[i].position = boids[i].logicPos;
                    boidData[i].isPlayer = boids[i].isPlayer ? 1 : 0;
                }

                var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
                boidBuffer.SetData(boidData);

                compute.SetBuffer(0, "boids", boidBuffer);
                compute.SetInt("numBoids", boids.Length);
                compute.SetFloat("viewRadius", settings.perceptionRadius);
                compute.SetFloat("seperateRadius", settings.seperateRadius);

                int threadGroups = Mathf.CeilToInt(numBoids / (float) threadGroupSize);
                compute.Dispatch(0, threadGroups, 1, 1);

                boidBuffer.GetData(boidData);

                for (int i = 0; i < boids.Length; i++)
                {
                    boids[i].centreOfFlockmates = boidData[i].flockCentre;
                    boids[i].seperateHeading = boidData[i].seperateHeading;
                    boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                    if(!boids[i].isPlayer)
                        boids[i].UpdateBoid();
                }

                boidBuffer.Release();
            }
        }
    }

    public struct BoidData {
        public Vector2 position;
        public Vector2 flockCentre;
        public Vector2 seperateHeading;
        public int numFlockmates;
        public int isPlayer;

        public static int Size {
            get {
                return sizeof (float) * 2 * 3 + sizeof (int) * 2;
            }
        }
    }
}