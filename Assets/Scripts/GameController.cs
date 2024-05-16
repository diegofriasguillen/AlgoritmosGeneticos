using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public Transform[] spawnPoints;

    public float reproductionInterval = 15f;
    public float maxAnimalAge = 100f;

    private List<GameObject> animals = new List<GameObject>();
    public Transform[] reproductionPoints;

    void Start()
    {
        Dictionary<AnimalController.AnimalType, int> animalCounts = new Dictionary<AnimalController.AnimalType, int>();
        foreach (var type in (AnimalController.AnimalType[])System.Enum.GetValues(typeof(AnimalController.AnimalType)))
        {
            animalCounts.Add(type, 0);
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < animalPrefabs.Length; i++)
            {
                GameObject animalPrefab = animalPrefabs[i];
                AnimalController.AnimalType animalType = (AnimalController.AnimalType)i;

                if (animalCounts[animalType] < 4)
                {
                    animalCounts[animalType]++;
                    Vector3 position = spawnPoint.position;
                    if (i < reproductionPoints.Length)
                    {
                        position = reproductionPoints[i].position;
                        position += Random.insideUnitSphere * 2f;
                        position.y = spawnPoint.position.y;
                    }
                    GameObject newAnimal = Instantiate(animalPrefab, position, Quaternion.identity);
                    animals.Add(newAnimal);
                }
            }
        }

        StartCoroutine(ReproductionRoutine());
    }

    //peoplecalifornication
    IEnumerator ReproductionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(reproductionInterval);

            List<GameObject> animalsToRemove = new List<GameObject>();
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
                }
            }

            foreach (GameObject animalToRemove in animalsToRemove)
            {
                animals.Remove(animalToRemove);
                Destroy(animalToRemove);
            }
        }
    }

    GameObject FindPartner(GameObject currentAnimal)
    {
        if (animals.Count > 1)
        {
            List<GameObject> potentialPartners = new List<GameObject>();
            AnimalController currentAnimalController = currentAnimal.GetComponent<AnimalController>();

            foreach (GameObject animal in animals)
            {
                AnimalController animalController = animal.GetComponent<AnimalController>();
                if (animalController != null && animalController.animalType == currentAnimalController.animalType && animalController != currentAnimal)
                {
                    potentialPartners.Add(animal);
                }
            }

            if (potentialPartners.Count > 0)
            {
                return potentialPartners[Random.Range(0, potentialPartners.Count)];
            }
        }
        return null;
    }


    public void SpawnOffspring(AnimalController.AnimalType type, Vector3 position, Genes genes)
    {
        GameObject animalPrefab = animalPrefabs[(int)type];
        GameObject offspring = Instantiate(animalPrefab, position, Quaternion.identity);
        AnimalController animalController = offspring.GetComponent<AnimalController>();
        animalController.SetGenes(genes);
        animals.Add(offspring);
    }
}
