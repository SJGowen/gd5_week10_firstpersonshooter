using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaycastShoot : MonoBehaviour
{
    [SerializeField] public int gunDamage = 1;
    [SerializeField] public float fireRate = 0.5f;
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

    public Slider laserTempSlider;
    Color minColor = Color.green;
    Color maxColor = Color.red;

    void Start()
    {
        fpsCam = Camera.main;
        gunAudio = GetComponent<AudioSource>();
        laserLine = GetComponent<LineRenderer>();
        StartCoroutine(SpawnSingleCube());
        StartCoroutine(CoolDownLaserSlider());
    }

    void Update()
    {
        Debug.Log($"Laser Temp: {laserTempSlider.value}");
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
                laserLine.SetPosition(1, hit.point);

                ShootableBox target = hit.transform.GetComponent<ShootableBox>();
                if (target != null)
                {
                    //Debug.Log($"Hit: {hit.transform.name} with damage {gunDamage}.");
                    target.Damage(gunDamage);
                }

                if (hit.rigidbody != null)
                {
                    //Debug.Log($"Hit: {hit.transform.name}, applying hitForce: {hitForce}.");
                    hit.rigidbody.AddForce(-hit.normal * hitForce, ForceMode.Impulse);
                }
            }
            else
            {
                laserLine.SetPosition(1, fpsCam.transform.forward * 1000_000);
            }

            laserTempSlider.value += 0.1f; // Increase the slider value as shot was fired
            //laserTempSlider.image.color = Color.Lerp(minColor, maxColor, laserTempSlider.value);
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
                laserTempSlider.value -= 0.1f; // Decrease the slider value over time
                //laserTempSlider.image.color = Color.Lerp(minColor, maxColor, laserTempSlider.value);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
