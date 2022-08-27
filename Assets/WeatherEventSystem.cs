using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEventSystem : MonoBehaviour
{
    public enum WeatherType
    {
        Normal,
        Rain,
        Storm,
        Drought
    }

    public enum WeatherEvent
    {
        None,
        Flood,
        Lightning,
        Drought
    }


    public WeatherType weatherType;
    public WeatherEvent weatherEvent;

    public void NextDay()
    {
        // TODO: Add rain/storm/sun effects
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                weatherType = WeatherType.Normal;
                break;
            case 1:
                weatherType = WeatherType.Rain;
                break;
            case 2:
                weatherType = WeatherType.Storm;
                break;
            case 3:
                weatherType = WeatherType.Drought;
                break;
            default:
                weatherType = WeatherType.Normal;
                break;
        }

        GenerateNewEvent();
    }

    public void GenerateNewEvent()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                weatherEvent = WeatherEvent.None;
                break;
            case 1:
                weatherEvent = WeatherEvent.Flood;
                break;
            case 2:
                if (weatherType == WeatherType.Storm)
                {
                    weatherEvent = WeatherEvent.Lightning;
                }
                else
                {
                    weatherEvent = WeatherEvent.None;
                }
                break;
        }
    }
}
