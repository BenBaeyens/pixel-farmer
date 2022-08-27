using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class DebugWindow : MonoBehaviour
{

    private ConsoleManager manager;

    public TextMeshProUGUI WheatSeeds;
    public TextMeshProUGUI WheatCrops;

    public TextMeshProUGUI PotatoSeeds;
    public TextMeshProUGUI PotatoCrops;

    public TextMeshProUGUI FlowerSeeds;
    public TextMeshProUGUI FlowerCrops;

    public Button AddWheat;
    public Button RemoveWheat;

    public Button AddPotato;
    public Button RemovePotato;

    public Button AddFlower;
    public Button RemoveFlower;

    public Button ReloadButton;
    public GameObject debugText;

    public bool isEnabled = true;

    public int wheatSeeds = 0, wheatCrops = 0, potatoSeeds = 0, potatoCrops = 0, flowerSeeds = 0, flowerCrops = 0;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<ConsoleManager>();


        AddWheat.onClick.AddListener(delegate { Buttons(1, Plant.PlantType.wheat); });
        RemoveWheat.onClick.AddListener(delegate { Buttons(-1, Plant.PlantType.wheat); });


        AddPotato.onClick.AddListener(delegate { Buttons(1, Plant.PlantType.potato); });
        RemovePotato.onClick.AddListener(delegate { Buttons(-1, Plant.PlantType.potato); });

        AddFlower.onClick.AddListener(delegate { Buttons(1, Plant.PlantType.flower); });
        RemoveFlower.onClick.AddListener(delegate { Buttons(-1, Plant.PlantType.flower); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        wheatSeeds = 0; wheatCrops = 0; potatoSeeds = 0; potatoCrops = 0; flowerSeeds = 0; flowerCrops = 0;
        foreach (Plant.PlantType plant in manager.seedInventory)
        {
            if (plant == Plant.PlantType.wheat)
                wheatSeeds++;
            else if (plant == Plant.PlantType.potato)
                potatoSeeds++;
            else
                flowerSeeds++;
        }
        foreach (Plant.PlantType plant in manager.plantInventory)
        {
            if (plant == Plant.PlantType.wheat)
                wheatCrops++;
            else if (plant == Plant.PlantType.potato)
                potatoCrops++;
            else
                flowerCrops++;
        }

        WheatSeeds.text = "Wheat seeds: " + wheatSeeds;
        WheatCrops.text = "Wheat crops: " + wheatCrops;
        PotatoSeeds.text = "Potato seeds: " + potatoSeeds;
        PotatoCrops.text = "Potato crops: " + potatoCrops;
        FlowerSeeds.text = "Flower seeds: " + flowerSeeds;
        FlowerCrops.text = "Flower crops: " + flowerCrops;

    }

    public void Buttons(int count, Plant.PlantType type)
    {
        if (count < 0 && manager.seedInventory.Contains(type))
        {
            manager.seedInventory.Remove(type);
        }
        else if (count > 0)
        {
            manager.seedInventory.Add(type);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleWindow()
    {
        isEnabled = !isEnabled;
        WheatSeeds.gameObject.SetActive(!WheatSeeds.gameObject.activeSelf);
        PotatoSeeds.gameObject.SetActive(!PotatoSeeds.gameObject.activeSelf);
        FlowerSeeds.gameObject.SetActive(!FlowerSeeds.gameObject.activeSelf);
        FlowerCrops.gameObject.SetActive(!FlowerCrops.gameObject.activeSelf);
        WheatCrops.gameObject.SetActive(!WheatCrops.gameObject.activeSelf);
        PotatoCrops.gameObject.SetActive(!PotatoCrops.gameObject.activeSelf);
        AddFlower.gameObject.SetActive(!AddFlower.gameObject.activeSelf);
        AddWheat.gameObject.SetActive(!AddWheat.gameObject.activeSelf);
        AddPotato.gameObject.SetActive(!AddPotato.gameObject.activeSelf);
        RemoveFlower.gameObject.SetActive(!RemoveFlower.gameObject.activeSelf);
        RemovePotato.gameObject.SetActive(!RemovePotato.gameObject.activeSelf);
        RemoveWheat.gameObject.SetActive(!RemoveWheat.gameObject.activeSelf);
        ReloadButton.gameObject.SetActive(!ReloadButton.gameObject.activeSelf);
        debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);

    }

}
