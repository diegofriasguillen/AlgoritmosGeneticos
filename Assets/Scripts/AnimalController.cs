using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public enum AnimalType { Chicken, Penguin, Cat, Dog, Deer, Tiger };
    public AnimalType animalType;
    //health-age
    public float maxHealth = 100f;
    public float maxAge = 100f;
    private float currentHealth;
    public float currentAge;
    //reproductionbaby
    public float reproductionCooldown = 10f;
    private bool canReproduce = true;
    public float reproductionDistance = 1f;
    //referencesotherscripts
    private GameController gameController;
    public Genes genes;
    //movement
    public float movementSpeed = 5f;
    //hunt-eat-runbitc
    public float fleeDistance = 10f;
    public float detectionRadius = 10f;
    public float hungerThreshold = 10f;
    private float currentHunger = 0f;
    public float preyDetectionRange = 1f;
    //wheretogo
    public List<GameObject> destinations;
    private int currentDestinationIndex = 0;
    private float timeSinceLastDestinationChange = 0f;
    public float destinationChangeInterval = 10f;
    public Vector3 homePosition;

    void Start()
    {
        currentHealth = maxHealth;
        currentAge = 0f;
        gameController = FindObjectOfType<GameController>();
        homePosition = transform.position;
        genes = new Genes();
        SetRandomDestination();
        SetNextDestination();
    }

    void Update()
    {
        currentHunger += Time.deltaTime;
        currentAge += Time.deltaTime;

        if (currentHunger >= hungerThreshold && IsPredator())
        {
            GameObject prey = FindPrey();
            if (prey != null)
            {
                MoveTowardsDestination(prey.transform.position);
                return;
            }
        }

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

        MoveTowardsDestination(destinations[currentDestinationIndex].transform.position);
        CheckDestinationChange();

        if (IsOld())
        {
            Die();
            return;
        }
    }

    void ChickenBehaviour()
    {
        FleeFromPredators();
    }

    void PenguinBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            MoveTowardsDestination(prey.transform.position);
        }
        else
        {
            FleeFromPredators();
        }
    }

    void CatBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            MoveTowardsDestination(prey.transform.position);
        }
        else
        {
            FleeFromPredators();
        }
    }

    void DogBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            MoveTowardsDestination(prey.transform.position);
        }
        else
        {
            FleeFromPredators();
        }
    }

    void DeerBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            MoveTowardsDestination(prey.transform.position);
        }
        else
        {
            FleeFromPredators();
        }
    }

    void TigerBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            MoveTowardsDestination(prey.transform.position);
            return;
        }
    }

    void MoveTowardsDestination(Vector3 targetPosition)
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
    }

    void CheckDestinationChange()
    {
        timeSinceLastDestinationChange += Time.deltaTime;
        if (timeSinceLastDestinationChange >= destinationChangeInterval)
        {
            SetRandomDestination();
            timeSinceLastDestinationChange = 0f;
        }
    }

    void SetNextDestination()
    {
        currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Count;
    }

    private bool IsPredator()
    {
        return animalType == AnimalType.Cat || animalType == AnimalType.Penguin || animalType == AnimalType.Dog ||animalType == AnimalType.Deer|| animalType == AnimalType.Tiger;
    }

    private GameObject FindPrey()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (IsPrey(collider.tag))
            {
                if (Vector3.Distance(transform.position, collider.transform.position) <= preyDetectionRange)
                {
                    currentHunger = 0f;
                    Destroy(collider.gameObject);
                    return collider.gameObject;
                }
            }
        }
        return null;
    }

    bool IsPrey(string tag)
    {
        switch (animalType)
        {
            case AnimalType.Chicken:
                return tag == "Plant";
            case AnimalType.Penguin:
                return tag == "Chicken";
            case AnimalType.Cat:
                return tag == "Chicken" || tag == "Penguin";
            case AnimalType.Dog:
                return tag == "Chicken" || tag == "Penguin" || tag == "Cat";
            case AnimalType.Deer:
                return tag == "Chicken" || tag == "Penguin" || tag == "Cat" || tag == "Dog";
            case AnimalType.Tiger:
                return tag == "Chicken" || tag == "Penguin" || tag == "Cat" || tag == "Dog" || tag == "Deer";
            default:
                return false;
        }
    }

    void FleeFromPredators()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            AnimalController otherAnimal = collider.GetComponent<AnimalController>();
            if (otherAnimal != null && otherAnimal.IsPredator() && otherAnimal.animalType != animalType)
            {
                Vector3 fleeDirection = transform.position - otherAnimal.transform.position;
                fleeDirection.y = 0;
                fleeDirection.Normalize();
                Vector3 fleePoint = transform.position + fleeDirection * fleeDistance;
                MoveTowardsDestination(fleePoint);
                return;
            }
        }
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

        float distance = Vector3.Distance(transform.position, partner.transform.position);
        if (distance <= reproductionDistance)
        {
            StartCoroutine(MoveToHomeAndReproduce(partnerAnimal));
        }
    }

    private IEnumerator MoveToHomeAndReproduce(AnimalController partnerAnimal)
    {
        Vector3 partnerHomePosition = partnerAnimal.homePosition;
        while (Vector3.Distance(transform.position, homePosition) > 0.1f)
        {
            MoveTowardsDestination(homePosition);
            yield return null;
        }

        while (Vector3.Distance(partnerAnimal.transform.position, partnerHomePosition) > 0.1f)
        {
            partnerAnimal.MoveTowardsDestination(partnerHomePosition);
            yield return null;
        }

        Genes childGenes = new Genes(genes, partnerAnimal.genes);
        gameController.SpawnOffspring(animalType, homePosition, childGenes);

        canReproduce = false;
        partnerAnimal.canReproduce = false;
        StartCoroutine(ResetReproductionCooldown());
        partnerAnimal.StartCoroutine(partnerAnimal.ResetReproductionCooldown());
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

    public bool IsOld()
    {
        return currentAge >= maxAge;
    }

    void SetRandomDestination()
    {
        currentDestinationIndex = Random.Range(0, destinations.Count);
    }

    public void SetGenes(Genes newGenes)
    {
        genes = newGenes;
    }

    public void Die()
    {
        gameController.RemoveAnimal(gameObject);
        Destroy(gameObject);
    }
}
