using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingTile : MonoBehaviour
{

    public List<Sprite> variants;

    public GameObject wheatPrefab;
    public GameObject potatoPrefab;
    public GameObject flowerPrefab;
    public List<Vector2> offsets = new List<Vector2>();

    public int daysInDyingState = 0;

    public enum TileHealthState
    {
        Very_Dry = 0,
        Dry = 1,
        Healthy = 2,
        Wet = 3,
        Drowning = 4
    }


    public TileHealthState tileState;
    public Plant plant;

    private ConsoleManager console;

    private void Start()
    {
        console = GameObject.FindObjectOfType<ConsoleManager>();
    }

    public void NextDay(WeatherEventSystem.WeatherType weather, WeatherEventSystem.WeatherEvent weatherEvent)
    {




        // TODO: Add lightning strike

        if (weather == WeatherEventSystem.WeatherType.Normal || weather == WeatherEventSystem.WeatherType.Drought)
        {
            if (TileHealthState.Very_Dry == tileState)
            {
                daysInDyingState++;
            }
            else
            {
                tileState = tileState.Previous();
            }
        }
        else if (weather == WeatherEventSystem.WeatherType.Rain)
        {
            if ((int)tileState < 3)
            {
                tileState = tileState.Next();
            }
        }

        else if ((int)tileState < 3)
        {
            tileState = TileHealthState.Wet;
        }
        else
        {
            tileState = TileHealthState.Drowning;
        }

        if (weather == WeatherEventSystem.WeatherType.Drought && (int)tileState > 1)
        {
            tileState = TileHealthState.Dry;
        }

        if (tileState == TileHealthState.Healthy)
        {
            if (plant != null)
            {
                plant.Grow();
            }
            daysInDyingState = 0;
        }


        else if (tileState == TileHealthState.Very_Dry || tileState == TileHealthState.Drowning)
        {
            daysInDyingState++;
        }
        else if (daysInDyingState > 0)
        {
            daysInDyingState--;
        }

        if (daysInDyingState > 2)
        {
            Die();
        }

        UpdateGraphic();
    }

    public void Water()
    {
        if (tileState == TileHealthState.Drowning)
        {
            console.ThrowError("unable to water tile " + this.name + ". It's already drowning.");
            return;
            // TODO: Kill the plant when watering whilst drowning? 
        }

        tileState = tileState.Next();


        UpdateGraphic();



    }

    public void Die()
    {
        if (plant != null)
        {
            //TODO: Add dying message/animation
            console.commandLogHistory.Add($"oh no! a {plant.plantType} plant on {this.name} has died!");
            console.UpdateConsoleLog();
            Destroy(plant.gameObject);
            plant = null;
        }
    }

    public void GrowDebug()
    {
        if (plant != null)
        {
            plant.Grow();
        }
    }


    public void PlantSeed(Plant.PlantType type)
    {
        daysInDyingState = 0;
        switch (type)
        {
            case Plant.PlantType.wheat:
                plant = Instantiate(wheatPrefab, new Vector3(transform.position.x + offsets[0].x, transform.position.y + offsets[0].y, transform.position.z), Quaternion.identity, transform).GetComponent<Plant>();
                break;
            case Plant.PlantType.potato:
                plant = Instantiate(potatoPrefab, new Vector3(transform.position.x + offsets[1].x, transform.position.y + offsets[1].y, transform.position.z), Quaternion.identity, transform).GetComponent<Plant>();
                break;
            case Plant.PlantType.flower:
                print(plant);
                plant = Instantiate(flowerPrefab, new Vector3(transform.position.x + offsets[2].x, transform.position.y + offsets[2].y, transform.position.z), Quaternion.identity, transform).GetComponent<Plant>();
                break;
        }

    }

    public void Harvest()
    {
        if (CanHarvest())
        {
            Destroy(plant.gameObject);
            plant = null;
        }
    }

    public bool CanHarvest()
    {
        if (plant != null)
            return plant.isInFinalStage ? true : false;
        return false;
    }


    public bool CanPlant()
    {
        return plant == null;
    }

    public TileHealthState GetTileHealth()
    {
        return tileState;
    }

    public void UpdateGraphic()
    {
        if ((int)tileState < 2)
        {
            GetComponent<SpriteRenderer>().sprite = variants[0];
        }
        else if ((int)tileState == 2)
        {
            GetComponent<SpriteRenderer>().sprite = variants[1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = variants[2];

        }
    }

}




public static class Extensions
{

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
}