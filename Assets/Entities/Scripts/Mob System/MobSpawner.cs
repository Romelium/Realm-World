using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [Tooltip("The object that be follow for mobs to be spawned around it.\n\ndefault is main camera if there one.")]
    public Transform target;
    [Tooltip("The object that contain the spawned mobs.\n\ndefault is the object containing this script.")]
    public Transform parent;
    public Mob[] Mobs;
    [Tooltip("Play with it!")]
    public int radius = 10;
    [Tooltip("Play with it!")]
    public int thinkness = 100;
    [Tooltip("Play with it!")]
    public Vector3 offset = new Vector3(0, 10, 0);
    [Tooltip("Play with it! In Play mode with mobs!")]
    public float groundDistance = 10;
    [Tooltip("Play with it! In Play mode with mobs!")]
    public float spawnTime = 1;
    [Header("Gizmos")]
    public uint quality = 10;

    private float spawnTimeDelta = 0f;
    struct MobRay
    {
        public Ray ray;
        public enum HitStatus
        {
            cant,
            didNotMeetRequirements,
            success
        }
        public HitStatus hitStatus;
    }
    private MobRay[] mobRays;

    void Awake()
    {
        mobRays = new MobRay[Mobs.Length];
        if (target == null)
            target = Camera.main.transform;
        if (parent == null)
            parent = transform;
    }
    void Update()
    {
        if (spawnTimeDelta > spawnTime)
        {
            for (int i = 0; i < Mobs.Length; i++)
            {
                var random2D = Random.insideUnitCircle;
                var random3D = new Vector3(random2D.x, 0, random2D.y);

                mobRays[i].ray = new Ray(random3D.normalized * radius + random3D * thinkness + offset + target.position, Vector3.down);
                mobRays[i].hitStatus = MobRay.HitStatus.cant;

                var hit = new RaycastHit();
                if (Physics.Raycast(mobRays[i].ray, out hit))
                {
                    mobRays[i].hitStatus = MobRay.HitStatus.didNotMeetRequirements;
                    if (
                        Mobs[i].chance >= Random.Range(0f, 1f) &&
                        Mobs[i].heightCutOff < hit.point.y &&
                        Mobs[i].allowedSteepness < hit.normal.y
                    )
                    {
                        mobRays[i].hitStatus = MobRay.HitStatus.success;
                        Instantiate(Mobs[i].mob, hit.point, Mobs[i].mob.transform.rotation, parent);
                    }
                }
            }
            spawnTimeDelta = 0f;
        }
        spawnTimeDelta += Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        var prev = Vector3.left; // Start point

        for (int i = 1; i < quality + 1; i++)
        {
            var angle = (1f / quality) * i; // [0,1] range
            angle = angle * Mathf.PI * 2 - Mathf.PI; // Convert to Radians: [-PI,PI] range

            var now = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)); // Radians to direction

            var worldOffset = offset + target.position;
            // Inner line
            Gizmos.DrawLine(prev * radius + worldOffset, now * radius + worldOffset);
            // Outer line
            Gizmos.DrawLine(prev * (radius + thinkness) + worldOffset, now * (radius + thinkness) + worldOffset);

            prev = now;
        }
        if (Application.isPlaying)
        {
            for (int i = 0; i < mobRays.Length; i++)
            {
                switch (mobRays[i].hitStatus)
                {
                    case MobRay.HitStatus.success:
                        Gizmos.color = Color.green;
                        break;
                    case MobRay.HitStatus.didNotMeetRequirements:
                        Gizmos.color = Color.yellow;
                        break;
                    case MobRay.HitStatus.cant:
                        Gizmos.color = Color.red;
                        break;
                }
                Gizmos.DrawRay(mobRays[i].ray.origin, mobRays[i].ray.direction * groundDistance);
            }
        }
    }
    void OnValidate()
    {
        Awake();
    }
}
