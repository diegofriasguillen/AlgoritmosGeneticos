using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public enum AnimalType { Chicken, Penguin, Cat, Dog, Deer, Tiger };

    public AnimalType animalType;
    public float maxHealth = 100f;
    public float maxAge = 100f;
    public float reproductionCooldown = 10f;

    private float currentHealth;
    public float currentAge;
    private bool canReproduce = true;
    private GameController gameController;

    private Vector3 homePosition;

    public float reproductionDistance = 1f;

    public float movementSpeed = 5f;
    public float fleeDistance = 10f;
    public float detectionRadius = 10f;

    public Genes genes;

    public List<GameObject> destinations;
    private int currentDestinationIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
        currentAge = 0f;
        gameController = FindObjectOfType<GameController>();

        homePosition = transform.position;

        genes = new Genes();

        SetNextDestination();
    }

    void Update()
    {
        switch (animalType)
        {
            case AnimalType.Chicken:
                ChickenBehaviour();
                break;
            case AnimalType.Penguin:
                PenguinBehaviour();
                break;
            case AnimalType.Cat:
                CatBehaviour();
                break;
            case AnimalType.Dog:
                DogBehaviour();
                break;
            case AnimalType.Deer:
                DeerBehaviour();
                break;
            case AnimalType.Tiger:
                TigerBehaviour();
                break;
        }

        MoveTowardsDestination();
    }

    void ChickenBehaviour()
    {
        FindFood("Plant");
    }

    void PenguinBehaviour()
    {
        FindFood("Chicken");
    }
    void CatBehaviour()
    {
        FindFood("Chicken");
        FindFood("Penguin");
    }

    void DogBehaviour()
    {
        FindFood("Cat");
        FindFood("Penguin");
        FindFood("Chicken");
    }

    void DeerBehaviour()
    {
        FindFood("Cat");
        FindFood("Chicken");
        FindFood("Dog");
        FindFood("Penguin");
    }

    void TigerBehaviour()
    {
        FindFood("Cat");
        FindFood("Chicken");
        FindFood("Dog");
        FindFood("Deer");
        FindFood("Penguin");
    }

    void MoveTowardsDestination()
    {
        if (destinations.Count == 0) return; 

        Vector3 currentDestination = destinations[currentDestinationIndex].transform.position;

        Vector3 moveDirection = (currentDestination - transform.position).normalized;

        transform.position += moveDirection * movementSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, currentDestination) < 0.1f)
        {
            SetNextDestination();
        }
    }

    void SetNextDestination()
    {
        // Incrementar el índice del destino actual
        currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Count;
    }
    void FindFood(string foodTag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(foodTag))
            {
                MoveTowardsDestination();
                return;
            }
        }
        MoveTowardsDestination();
    }

    void FleeFromPredators(string predatorTag)
    {
        // Huir de los depredadores con la etiqueta especificada
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(predatorTag))
            {
                // Si se detecta un depredador, huir de él
                Vector3 fleeDirection = transform.position - collider.transform.position;
                fleeDirection.y = 0; // Mantener la misma altura
                fleeDirection.Normalize();
                Vector3 fleePoint = transform.position + fleeDirection * fleeDistance;
                MoveTowardsDestination();
                return;
            }
        }
    }

    public void Eat(GameObject food)
    {
        switch (animalType)
        {
            case AnimalType.Chicken:
                if (food.CompareTag("Plant"))
                {
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Penguin:
                if (food.CompareTag("Chicken"))
                {
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Cat:
                if (food.CompareTag("Chicken") || food.CompareTag("Penguin"))
                {
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Dog:
                if (food.CompareTag("Chicken") || food.CompareTag("Penguin") || food.CompareTag("Cat"))
                {
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Deer:
                if (!food.CompareTag("Tiger"))
                {
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Tiger:
                break;
        }
    }

    private IEnumerator HealOverTime(float healAmount)
    {
        while (currentHealth < maxHealth)
        {
            yield return new WaitForSeconds(1.5f);
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    public void Flee()
    {
        transform.position = homePosition;
    }

    public bool CanReproduce()
    {
        return canReproduce;
    }

    public void Reproduce(GameObject partner)
    {
        if (!CanReproduce()) return;

        AnimalController partnerAnimal = partner.GetComponent<AnimalController>();
        if (partnerAnimal == null || partnerAnimal.animalType != animalType) return;

        if (!partnerAnimal.CanReproduce()) return;

        float distance = Vector3.Distance(transform.position, partner.transform.position);

        if (distance <= reproductionDistance)
        {
            Genes childGenes = new Genes(genes, partnerAnimal.genes);

            gameController.SpawnOffspring(animalType, transform.position, childGenes);
            canReproduce = false;
            partnerAnimal.canReproduce = false;

            StartCoroutine(ResetReproductionCooldown());
        }
    }

    private IEnumerator ResetReproductionCooldown()
    {
        yield return new WaitForSeconds(reproductionCooldown);
        canReproduce = true;
    }

    public void IncreaseAge()
    {
        currentAge += Time.deltaTime;
    }

    public bool IsOld(float maxAge = 20)
    {
        return currentAge >= maxAge;
    }

    public void SetGenes(Genes newGenes)
    {
        genes = newGenes;
    }
}
