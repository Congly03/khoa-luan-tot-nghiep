using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    [Header("ZombieSpawn Var")]
    public GameObject zombiePrefab;
    public GameObject dangerZone1;
    public Transform zombieSpawnPosition;
    private float repeatCycle = 1f;

    [Header("Sounds")]
    public AudioClip DangerZoneSound;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            InvokeRepeating("EnemySpawner", 1f, repeatCycle);
            audioSource.PlayOneShot(DangerZoneSound);
            StartCoroutine(dangerZoneTimer());
            Destroy(gameObject, 10f);
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
    private int zombieCount = 0;
    public int maxZombies = 2;

    void EnemySpawner()
    {
        if (zombieCount >= maxZombies)
        {
            CancelInvoke("EnemySpawner");
            return;
        }

        Instantiate(zombiePrefab, zombieSpawnPosition.position, zombieSpawnPosition.rotation);
        zombieCount++;
    }

    IEnumerator dangerZoneTimer()
    {
        dangerZone1.SetActive(true);
        yield return new WaitForSeconds(5f);
        dangerZone1.SetActive(false);
    }
}
