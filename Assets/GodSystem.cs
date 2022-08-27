using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSystem : MonoBehaviour
{
    private ConsoleManager console;

    public enum Happiness
    {
        Angry,
        Sad,
        Happy,
    }

    public int unsatisfiedCount;

    public Plant.PlantType requiredType;
    public int requiredAmount;
    public Happiness happiness;
    public int daysBeforeGettingAngry = 4;

    private int daysWithoutPlant;
    private int daysWithoutChallenge;
    bool initialMessage = false;
    bool currentlyHasChallenge = false;

    private void Start()
    {
        console = GameObject.FindObjectOfType<ConsoleManager>();
    }

    public void NextDay()
    {
        if (!currentlyHasChallenge)
        {
            daysWithoutChallenge++;
        }
        else
        {
            daysWithoutPlant++;
        }
        if (console.currentDay > 10)
        {
            if (!initialMessage)
            {
                console.GodsMessage("WE ARE THE GODS. PROVIDE US WITH THE PLANTS WE NEED OR SUFFER THE CONSEQUENCES");
                GenerateNewRequirements();
                initialMessage = true;
            }
            else if (daysWithoutChallenge > 5)
            {
                console.GodsMessage("WE REQUIRE ANOTHER SACRIFICE.");
                GenerateNewRequirements();
            }

            if (daysWithoutPlant >= daysBeforeGettingAngry)
            {
                AnnoyGods();
                ResetGods();
                unsatisfiedCount++;
                // TODO: Game over? lives?
            }
        }
        else
        {
            return;
        }

    }

    public bool CheckForGameOver()
    {
        if (unsatisfiedCount > 2)
        {
            console.GodsMessage("WE ARE THE GODS. YOU HAVE DISSAPOINTED US FOR THE LAST TIME.");
            return true;
        }
        return false;
    }

    private void ResetGods()
    {
        daysWithoutPlant = 0;

        currentlyHasChallenge = false;
        daysWithoutChallenge = 0;
    }

    private void GenerateNewRequirements()
    {
        // TODO: Check for unlock level
        // TODO: Increase difficulty as time progresses

        currentlyHasChallenge = true;
        daysWithoutPlant = 0;
        daysWithoutChallenge = 0;
        int randomPlant = Random.Range(0, 3);
        requiredAmount = 1 + Random.Range(0, 3);
        Debug.Log(randomPlant);

        if (randomPlant == 0)
        {
            requiredType = Plant.PlantType.wheat;
        }
        if (randomPlant == 1)
        {
            requiredType = Plant.PlantType.potato;
        }
        if (randomPlant == 2)
        {
            requiredType = Plant.PlantType.flower;
        }
        console.GodsMessage($"WE REQUIRE {requiredAmount} {requiredType.ToString().ToUpper()} PLANTS. GIVE THEM TO US AS FAST AS POSSIBLE.");
    }

    private void AnnoyGods()
    {
        Debug.Log("Annoyed gods");
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            happiness = Happiness.Sad;
            console.GodsMessage("WE ARE SAD. YOU DISSAPOINTED US. DO NOT DO SO AGAIN.");
            unsatisfiedCount++;
        }
        if (random == 1)
        {
            happiness = Happiness.Angry;
            console.GodsMessage("YOU DISSAPOINTED US. WE ARE ANGRY. DO NOT DO SO AGAIN.");
            unsatisfiedCount++;

        }
        else
        {
            happiness = Happiness.Happy;
            console.GodsMessage("YOU DISSAPOINTED US. WE WILL HAVE MERCY THIS TIME. DO NOT DISSAPOINT US AGAIN.");
        }
    }


    public void Sacrifice(Plant.PlantType sacrificedPlant, int amount)
    {
        // checks if the correct sacrifice has been given
        if (sacrificedPlant == requiredType && amount == requiredAmount)
        {
            daysWithoutChallenge = 0;
            currentlyHasChallenge = false;
        }
        else
        {
            console.GodsMessage("THIS IS NOT WHAT WE REQUIRE. DO BETTER.");
            daysWithoutPlant = 0;
        }
    }
}
