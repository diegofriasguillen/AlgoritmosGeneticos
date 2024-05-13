using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAnimals : MonoBehaviour
{
    //health
    public float animalsHealth;
    public int maxhealth;
    public int currentHealth;

    //drink water
    private int maxHidratation = 100;
    public int currentHydrate;
    private bool canHydrate = true;
    public GameObject waterPoint1;
    public GameObject waterPoint2;
    public GameObject waterPoint3;
    public GameObject waterPoint4;
    private float hydrateTime = 5f;
    private bool isHydrating = false;

    //eating
    private int maxEat;
    public int currentEat;
    private bool canEat = true;
    private float eatTime = 3f;
    private bool isEating = false;

    //chillpoints
    public Transform place1;
    public Transform place2;
    public Transform place3;
    public Transform place4;
    public Transform place5;
    public Transform place6;

    private System.Collections.IEnumerator DrinkWater()
    {
        isHydrating = true;

        while (currentHydrate < maxHidratation)
        {
            yield return new WaitForSeconds(hydrateTime);
            currentHydrate += 10;
            currentHydrate = Mathf.Clamp(currentHydrate, 0, maxHidratation);
        }

        isHydrating = false;
    }

    private void Flee()
    {

    }

    private void MakingLove()
    {

    }

    private void JustHanging()
    {

    }

    private void Dying()
    {

    }
}
