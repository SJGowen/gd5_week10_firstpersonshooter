using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaycastShoot : MonoBehaviour
{
    [SerializeField] public int gunDamage = 1;
    [SerializeField] public float fireRate = 0.3f;
    [SerializeField] public float fireRange = 50f;
    [SerializeField] public float hitForce = 10f;
    
    float spawnAreaRange = 8f;
    float cubeScale = 1f;
    float nextFire;

    Camera fpsCam;

    AudioSource gunAudio;
    LineRenderer laserLine;
    [SerializeField] float shotDuration = 0.2f;
    [SerializeField] Transform gunEnd;

    public LayerMask layerMask;
    public GameObject cubePrefab;
    public GameObject evaPrefab;
    int EvasToSpawn = 0;

    public Slider laserTempSlider;
    public Image laserTempFillImage;

    void Start()
    {
        fpsCam = Camera.main;
        gunAudio = GetComponent<AudioSource>();
        laserLine = GetComponent<LineRenderer>();
        StartCoroutine(SpawnSingleCube());
        StartCoroutine(CoolDownLaserSlider());
        StartCoroutine(SpawnEvaRagdoll());
    }

    void Update()
    {
        if (laserTempSlider.value < 1.0f && Input.GetButtonDown("Fire1") && Time.time > nextFire) // Single Shot
        // if (Input.GetButton("Fire1") && Time.time > nextFire) // Continuous Fire
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(ShootingEffects());

            // Raycast shooting
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new (0.5f, 0.5f, 0));

            laserLine.SetPosition(0, gunEnd.position);

            Debug.DrawRay(rayOrigin, fpsCam.transform.forward * fireRange, Color.red);

            // RaycastHit hit;
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out var hit, fireRange, layerMask))
            {
                HandleLaserHit(hit);
            }
            else
            {
                laserLine.SetPosition(1, fpsCam.transform.forward * 1000_000);
            }

            UpdateLaserTempBar(0.1f);
        }
    }

    private void HandleLaserHit(RaycastHit hit)
    {
        laserLine.SetPosition(1, hit.point);

        Shootable target = hit.transform.GetComponent<Shootable>();
        if (target != null)
        {
            //Debug.Log($"Hit: {hit.transform.name} with damage {gunDamage}.");
            target.Damage(gunDamage);
        }

        Enemy ragdollEnemy = hit.transform.GetComponent<Enemy>();

        if (ragdollEnemy != null)
        {
            //Debug.Log($"Hit: {hit.transform.name}, triggering ragdoll.");
            ragdollEnemy.TriggerRagdoll();
            Destroy(hit.transform.gameObject, 5f);
        }

        if (hit.rigidbody != null)
        {
            //Debug.Log($"Hit: {hit.transform.name}, applying hitForce: {hitForce}.");
            hit.rigidbody.AddForce(-hit.normal * hitForce, ForceMode.Impulse);
        }
    }

    IEnumerator ShootingEffects()
    {
        gunAudio.Play();
        laserLine.enabled = true;
        yield return new WaitForSeconds(shotDuration);
        laserLine.enabled = false;
    }

    IEnumerator SpawnSingleCube()
    {
        while (true)
        {
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
            if (cubes.Length < 1)
            {
                Vector3 randomPosition = new(
                    Random.Range(-spawnAreaRange, spawnAreaRange),
                    12, // Give some height so that cubes can be ontop of buildings & trees
                    Random.Range(-spawnAreaRange, spawnAreaRange)
                );

                // Create a cube that is of specified scale at a random position
                Instantiate(cubePrefab, randomPosition, cubePrefab.transform.rotation).transform.localScale = Vector3.one * cubeScale;
                // Increase the spawn area
                spawnAreaRange += 1;
                // Make this cubeScale 90% of former cubeScale
                cubeScale *= 0.9f;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CoolDownLaserSlider()
    {
        while (true)
        {
            if (laserTempSlider.value > 0.1f)
            {
                UpdateLaserTempBar(-0.1f);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SpawnEvaRagdoll()
    {
        while (true)
        {
            GameObject[] evas = GameObject.FindGameObjectsWithTag("Eva");
            if (evas.Length < 1)
            {
                EvasToSpawn++;
                
                for (var i = 0; i < EvasToSpawn; i++)
                {
                    Vector3 randomPosition = new(
                        Random.Range(-spawnAreaRange, spawnAreaRange),
                        12, // Give some height so that Eva can be ontop of buildings & trees
                        Random.Range(-spawnAreaRange, spawnAreaRange)
                    );
                    // Create a Eva Ragdoll at a random position
                    Instantiate(evaPrefab, randomPosition, evaPrefab.transform.rotation);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateLaserTempBar(float amountToAdd)
    {
        laserTempSlider.value += amountToAdd;
        float valuePercent = laserTempSlider.value / laserTempSlider.maxValue;
        laserTempFillImage.color = Color.Lerp(Color.green, Color.red, valuePercent);
    }
}
