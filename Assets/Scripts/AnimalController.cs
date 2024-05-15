using System;
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

    private float timeSinceLastDestinationChange = 0f;
    public float destinationChangeInterval = 10f;

    public float hungerThreshold = 10f; 
    private float currentHunger = 0f;
    public float preyDetectionRange = 1f;



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
        // Actualizar el contador de hambre
        currentHunger += Time.deltaTime;
        currentAge += Time.deltaTime;

        // Verificar si el animal está hambriento
        if (currentHunger >= hungerThreshold)
        {
            // Buscar presas adecuadas si el animal es un cazador
            if (IsPredator())
            {
                // Implementar la lógica para buscar y perseguir presas
                GameObject prey = FindPrey();
                if (prey != null)
                {
                    // Perseguir a la presa
                    MoveTowardsDestination(prey.transform.position);
                    return;
                }
            }
        }

        // Si el animal no está cazando, continuar con su comportamiento normal
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

        // Movimiento
        MoveTowardsDestination(destinations[currentDestinationIndex].transform.position);

        CheckDestinationChange();

        // Actualizar el tiempo desde el último cambio de destino
        timeSinceLastDestinationChange += Time.deltaTime;
        if (timeSinceLastDestinationChange >= destinationChangeInterval)
        {
            SetRandomDestination();
            timeSinceLastDestinationChange = 0f;
        }

        // Envejecimiento y muerte
        IncreaseAge();
        if (IsOld(maxAge))
        {
            Die();
            return;
        }
    }

    void ChickenBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Si hay una presa, huir del depredador más cercano
            FleeFromPredators();
            return;
        }
    }

    void PenguinBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Si hay una presa, huir del depredador más cercano
            FleeFromPredators();
            return;
        }
    }
    void CatBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Si hay una presa, huir del depredador más cercano
            FleeFromPredators();
            return;
        }
    }

    void DogBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Si hay una presa, huir del depredador más cercano
            FleeFromPredators();
            return;
        }
    }

    void DeerBehaviour()
    {
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Si hay una presa, huir del depredador más cercano
            FleeFromPredators();
            return;
        }
    }

    void TigerBehaviour()
    {
        //eats everything
        GameObject prey = FindPrey();
        if (prey != null)
        {
            // Perseguir y comer a la presa
            MoveTowardsDestination(prey.transform.position);
            return;
        }
        //he doesn't run he's the big one babyyy
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
        // Incrementar el índice del destino actual
        currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Count;
    }

    private bool IsPredator()
    {
        return animalType == AnimalType.Cat || animalType == AnimalType.Dog || animalType == AnimalType.Tiger || animalType == AnimalType.Deer || animalType == AnimalType.Penguin;
    }

    private GameObject FindPrey()
    {
        string currentAnimalTag = animalType.ToString();

        List<string> preyTags = new List<string>(Enum.GetNames(typeof(AnimalType)));
        preyTags.Remove(currentAnimalTag);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (IsPrey(collider.tag))
            {
                if (Vector3.Distance(transform.position, collider.transform.position) <= preyDetectionRange)
                {
                    currentHunger = 0f;
                    Eat(collider.gameObject);
                    Debug.Log("noooomoriiistessss");
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
        if (ShouldFleeFromPredator())
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Cat") || collider.CompareTag("Dog") || collider.CompareTag("Tiger") || collider.CompareTag("Penguin") || collider.CompareTag("Deer"))
                {
                    Vector3 fleeDirection = transform.position - collider.transform.position;
                    fleeDirection.y = 0; 
                    fleeDirection.Normalize();
                    Vector3 fleePoint = transform.position + fleeDirection * fleeDistance;
                    MoveTowardsDestination(destinations[currentDestinationIndex].transform.position);
                    return;
                }
            }
        }
    }

    private bool ShouldFleeFromPredator()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Cat") || collider.CompareTag("Dog") || collider.CompareTag("Tiger") || collider.CompareTag("Penguin") || collider.CompareTag("Deer"))
            {
                return true;
            }
        }
        return false;
    }

    public void Eat(GameObject food)
    {
        switch (animalType)
        {
            case AnimalType.Chicken:
                if (food.CompareTag("Plant"))
                {
                    currentHunger = 0f;
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
                    currentHunger = 0f;
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
                    currentHunger = 0f;
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
                    currentHunger = 0f;
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
                break;

            case AnimalType.Tiger:
                if (food.CompareTag("Chicken") || food.CompareTag("Penguin") || food.CompareTag("Cat") || food.CompareTag("Dog") || food.CompareTag("Deer"))
                {
                    currentHunger = 0f;
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                break;

            case AnimalType.Deer:
                if (food.CompareTag("Chicken") || food.CompareTag("Penguin") || food.CompareTag("Cat") || food.CompareTag("Dog"))
                {
                    currentHunger = 0f;
                    Destroy(food);
                    StartCoroutine(HealOverTime(20f));
                }
                else
                {
                    Flee();
                }
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

    //repasar
    public void Reproduce(GameObject partner)
    {
        if (!CanReproduce()) return;

        AnimalController partnerAnimal = partner.GetComponent<AnimalController>();
        if (partnerAnimal == null || partnerAnimal.animalType != animalType) return;

        // Restricción: Solo permitir la reproducción entre animales del mismo tipo
        if (partnerAnimal.animalType != animalType) return;

        // Restricción: Calcular la similitud genética y permitir la reproducción si es mayor que un umbral
        float geneticSimilarityThreshold = 0.5f; // Puedes ajustar este valor
        float geneticSimilarity = CalculateGeneticSimilarity(partnerAnimal.genes);
        if (geneticSimilarity < geneticSimilarityThreshold) return;

        // Restricción basada en la cadena alimenticia (si es necesario)

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

    //repasar
    private float CalculateGeneticSimilarity(Genes partnerGenes)
    {
        // Calcular la similitud genética entre los genes del animal y los genes del compañero
        float colorSimilarity = Vector3.Distance(new Vector3(genes.color.r, genes.color.g, genes.color.b), new Vector3(partnerGenes.color.r, partnerGenes.color.g, partnerGenes.color.b));
        float speedDifference = Mathf.Abs(genes.speed - partnerGenes.speed);
        // Calcula la similitud basada en los demás atributos genéticos y devuelve un valor promedio
        return (colorSimilarity + speedDifference) / 2f;
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

    void SetRandomDestination()
    {
        // Seleccionar aleatoriamente un índice de destino inicial
        currentDestinationIndex = UnityEngine.Random.Range(0, destinations.Count);
    }

    public void Die()
    {
        // Eliminar el animal de la lista de animales en el GameController
        gameController.RemoveAnimal(gameObject);

        // Destruir el GameObject asociado al animal
        Destroy(gameObject);
    }
}
