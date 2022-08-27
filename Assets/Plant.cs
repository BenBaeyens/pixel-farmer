using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plant : MonoBehaviour
{

    public enum PlantType
    {
        wheat = 0,
        potato = 1,
        flower = 2
    }

    public enum PlantGrowthState
    {
        seeded,
        rooted,
        sprouted,
        mature,
        blossoming
    }

    public Sprite seedGraphic;
    public Sprite rootGraphic;
    public Sprite sproutGraphic;
    public Sprite matureGraphic;
    public Sprite blossomGraphic;


    public PlantGrowthState plantGrowthState;
    public PlantType plantType;

    public bool isInFinalStage;

    private SpriteRenderer spriteRenderer;



    private void Start()
    {
        isInFinalStage = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 6;
        plantGrowthState = PlantGrowthState.seeded;
        UpdateGraphic();
    }

    public void Grow()
    {
        switch (plantGrowthState)
        {
            case PlantGrowthState.seeded:
                // Only go to sprout if the plant type is wheat (add more plants here)
                if (plantType == PlantType.wheat)
                    plantGrowthState = PlantGrowthState.sprouted;
                else
                    plantGrowthState = PlantGrowthState.rooted;
                break;
            case PlantGrowthState.rooted:
                plantGrowthState = PlantGrowthState.sprouted;
                break;
            case PlantGrowthState.sprouted:
                plantGrowthState = PlantGrowthState.mature;
                isInFinalStage = plantType == PlantType.flower ? false : true;
                break;
            case PlantGrowthState.mature:
                if (plantType == PlantType.flower)
                {
                    plantGrowthState = PlantGrowthState.blossoming;
                    isInFinalStage = true;
                }
                break;
        }

        UpdateGraphic();
    }

    public void UpdateGraphic()
    {
        switch (plantGrowthState)
        {
            case PlantGrowthState.seeded:
                spriteRenderer.sprite = seedGraphic;
                break;
            case PlantGrowthState.rooted:
                spriteRenderer.sprite = rootGraphic;
                break;
            case PlantGrowthState.sprouted:
                spriteRenderer.sprite = sproutGraphic;
                break;
            case PlantGrowthState.mature:
                spriteRenderer.sprite = matureGraphic;
                break;
            case PlantGrowthState.blossoming:
                spriteRenderer.sprite = blossomGraphic;
                break;

        }
    }

    public PlantGrowthState GetGrowthStage()
    {
        return plantGrowthState;
    }

}
