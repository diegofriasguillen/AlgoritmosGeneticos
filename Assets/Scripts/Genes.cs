using UnityEngine;

public class Genes
{
    public Color color;
    public float speed;
    public float strength;
    public float fear;
    public float intelligence;

    public Genes()
    {
        GenerateRandomGenes();
    }

    public Genes(Genes parent1Genes, Genes parent2Genes)
    {
        InheritGenes(parent1Genes, parent2Genes);
    }

    private void GenerateRandomGenes()
    {
        color = new Color(Random.value, Random.value, Random.value);
        speed = Random.Range(0f, 10f);
        strength = Random.Range(0f, 50f);
        fear = Random.Range(0f, 15f);
        intelligence = Random.Range(0f, 20f);
    }

    private void InheritGenes(Genes parent1Genes, Genes parent2Genes)
    {
        color = MutateColor((parent1Genes.color + parent2Genes.color) / 2f);
        speed = MutateValue((parent1Genes.speed + parent2Genes.speed) / 2f);
        strength = MutateValue((parent1Genes.strength + parent2Genes.strength) / 2f);
        fear = MutateValue((parent1Genes.fear + parent2Genes.fear) / 2f);
        intelligence = MutateValue((parent1Genes.intelligence + parent2Genes.intelligence) / 2f);
    }

    private Color MutateColor(Color baseColor)
    {
        float mutationRate = 0.1f;
        return new Color(
            Mathf.Clamp01(baseColor.r + Random.Range(-mutationRate, mutationRate)),
            Mathf.Clamp01(baseColor.g + Random.Range(-mutationRate, mutationRate)),
            Mathf.Clamp01(baseColor.b + Random.Range(-mutationRate, mutationRate))
        );
    }

    //allarevalues, entonces es la misma operación para everything
    private float MutateValue(float baseValue)
    {
        float mutationRate = 0.1f;
        float mutatedValue = baseValue + Random.Range(-mutationRate * baseValue, mutationRate * baseValue);
        return Mathf.Clamp(mutatedValue, 0f, Mathf.Infinity);
    }
}
