using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public Transform[] spawnPoints;
    public float reproductionInterval = 10f;
    public float maxAnimalAge = 100f;

    private List<GameObject> animals = new List<GameObject>();

    public Transform[] reproductionPoints;

    void Start()
    {
        // how many animals of each type are
        Dictionary<AnimalController.AnimalType, int> animalCounts = new Dictionary<AnimalController.AnimalType, int>();

        // counting animals
        foreach (var type in (AnimalController.AnimalType[])Enum.GetValues(typeof(AnimalController.AnimalType)))
        {
            animalCounts.Add(type, 0);
        }

        // Spawn initial animals
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < animalPrefabs.Length; i++)
            {
                GameObject animalPrefab = animalPrefabs[i];
                AnimalController.AnimalType animalType = (AnimalController.AnimalType)i;

                // Verificar si ya se han creado dos animales de este tipo
                if (animalCounts[animalType] < 2)
                {
                    // Incrementar el contador de este tipo de animal
                    animalCounts[animalType]++;

                    Vector3 position = spawnPoint.position;

                    // Usar los puntos de reproducción específicos para cada tipo de animal
                    if (i < reproductionPoints.Length)
                    {
                        position = reproductionPoints[i].position;

                        // Aplicar una desviación aleatoria a la posición de aparición
                        position += Random.insideUnitSphere * 2f; // Puedes ajustar el valor para controlar la distancia de desviación
                        position.y = spawnPoint.position.y; // Mantener la misma altura
                    }

                    // Instanciar un animal en el punto de spawn
                    GameObject newAnimal = Instantiate(animalPrefab, position, Quaternion.identity);
                    animals.Add(newAnimal);
                }
            }
        }

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

                    // if its old enough die
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

    public void SpawnOffspring(AnimalController.AnimalType type, Vector3 position, Genes genes)
    {
        GameObject animalPrefab = animalPrefabs[(int)type];

        GameObject offspring = Instantiate(animalPrefab, position, Quaternion.identity);

        AnimalController animalController = offspring.GetComponent<AnimalController>();

        animalController.SetGenes(genes);

        animals.Add(offspring);
    }
}
