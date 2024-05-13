using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public Transform[] spawnPoints;
    public float reproductionInterval = 10f;
    public float maxAnimalAge = 100f;

    private List<GameObject> animals = new List<GameObject>();

    void Start()
    {
        // Spawn initial animals
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
            GameObject newAnimal = Instantiate(animalPrefab, spawnPoint.position, Quaternion.identity);
            animals.Add(newAnimal);
        }

        // Start reproduction coroutine
        StartCoroutine(ReproductionRoutine());
    }

    IEnumerator ReproductionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(reproductionInterval);

            foreach (GameObject animal in animals)
            {
                AnimalController animalController = animal.GetComponent<AnimalController>();
                if (animalController != null)
                {
                    if (animalController.CanReproduce())
                    {
                        GameObject partner = FindPartner(animal);
                        if (partner != null)
                        {
                            animalController.Reproduce(partner);
                        }
                    }

                    // Aging and death
                    animalController.IncreaseAge();
                    if (animalController.IsOld(maxAnimalAge))
                    {
                        animals.Remove(animal);
                        Destroy(animal);
                    }
                }
            }
        }
    }

    GameObject FindPartner(GameObject currentAnimal)
    {
        // Implement partner finding logic here
        // For simplicity, let's just return a random animal for now
        if (animals.Count > 1)
        {
            List<GameObject> potentialPartners = new List<GameObject>(animals);
            potentialPartners.Remove(currentAnimal);
            return potentialPartners[Random.Range(0, potentialPartners.Count)];
        }
        else
        {
            return null;
        }
    }

    public void SpawnOffspring(AnimalController.AnimalType type, Vector3 position)
    {
        // Lógica para spawnear una cría en la posición dada
    }
}
