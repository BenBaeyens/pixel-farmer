using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public TextMeshProUGUI consoleLog;
    public TMP_InputField consoleInput;

    public List<string> inputHistory = new List<string>(); // List of inputs
    public List<string> commandLogHistory = new List<string>(); // List of correct commands
    public int maxConsoleLines = 27;

    [HideInInspector] public WeatherEventSystem weatherSystem;
    [HideInInspector] public GodSystem godSystem;
    [HideInInspector] public DebugWindow debug;

    public bool isGameOver;


    public string startMessage = "Hello, welcome to the game! You can type commands in the chat box below. Start with 'help' for a list of commands!";

    [Header("Sounds")]

    public AudioClip Type_Sound;
    public AudioClip Success_Command_Sound;
    public AudioClip Error_Command_Sound;

    public AudioClip Rain_Ambient_Sound;
    public AudioClip Storm_Ambient_Sound;
    public AudioClip Drought_Ambient_Sound;
    public AudioClip Ambient_Birds_Sound;
    public AudioClip Gods_Enter_Sound;
    public AudioClip Harvest_Sound;
    public AudioClip Plant_Sound;
    public AudioClip Water_Sound;
    public AudioClip Inventory_Sound;
    public AudioClip Wait_Sound;

    [Header("Colors")]

    public Color errorMessageColor;
    public Color plantMessageColor;
    public Color locationMessageColor;
    public Color quantityMessageColor;
    public Color healthyMessageColor;
    public Color dryMessageColor;
    public Color drowningMessageColor;
    public Color noPlantMessageColor;

    public Color wheatColor;
    public Color potatoColor;
    public Color flowerColor;

    public int unlockStage = 1;
    public int maxCharactersPerLine = 66;

    public List<Plant.PlantType> seedInventory = new List<Plant.PlantType>();
    public List<Plant.PlantType> plantInventory = new List<Plant.PlantType>();


    private int currentHistory = 0;

    public int currentDay = 0;

    public List<FarmingTile> tiles = new List<FarmingTile>();
    public Transform tileHolder;

    private void Awake()
    {
        weatherSystem = GameObject.FindObjectOfType<WeatherEventSystem>();
        godSystem = GameObject.FindObjectOfType<GodSystem>();
        debug = GameObject.FindObjectOfType<DebugWindow>();
    }

    private void Start()
    {

        currentDay = 0;
        // Start the game off with 4 wheat
        AddToInventory(4, Plant.PlantType.wheat);
        AddToInventory(2, Plant.PlantType.potato);
        AddToInventory(2, Plant.PlantType.flower);
        foreach (Transform tile in tileHolder)
        {
            if (tile.name != "Indicators")
            {
                tiles.Add(tile.GetComponent<FarmingTile>());
            }
        }

        commandLogHistory.Add(startMessage);
        UpdateConsoleLog();
    }

    private void Update()
    {
        if (isGameOver)
            return;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (inputHistory.Count > 0 && currentHistory < inputHistory.Count)
            {
                currentHistory += 1;
                consoleInput.text = inputHistory[inputHistory.Count - currentHistory];
                consoleInput.caretPosition = consoleInput.text.Length; // Set caret at end of text
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (inputHistory.Count > 0 && currentHistory > 1)
            {
                currentHistory -= 1;
                consoleInput.text = inputHistory[inputHistory.Count - currentHistory];
                consoleInput.caretPosition = consoleInput.text.Length; // Set caret at end of text

            }
            else
            {
                currentHistory = 0;
                consoleInput.text = "";
            }
        }
    }

    public void OnCompleteInput()
    {
        // Only execute if input is not null or equal to the previous input
        if (consoleInput.text != "")
        {
            // Get the command and clear the input field
            string command = consoleInput.text;
            consoleInput.text = "";
            ParseCommand(command);
            if (inputHistory.Count == 0 || inputHistory[inputHistory.Count - 1] != command)
                inputHistory.Add(command);
            consoleInput.Select();
            consoleInput.ActivateInputField();
            currentHistory = 0;
            AudioManager.Instance.Play(Success_Command_Sound, 0.5f);
        }

    }

    public void UpdateConsoleInputToLowerCase()
    {
        consoleInput.text = consoleInput.text.ToLower();
        AudioManager.Instance.Play(Type_Sound, 0.24f);
    }

    private void ParseCommand(string command)
    {
        if (isGameOver)
        {
            return;
        }
        // Get all the keywords in the command 
        string[] keywords = command.Split(' ');

        if (keywords.Length == 2)
            if (keywords[1] == "")
                // Only allow the first keyword (command) to stay
                keywords = keywords.Where(e => e == keywords[0]).ToArray();


        // Find out which command was used
        switch (keywords[0].ToLower())
        {
            case "help":
                // Display the commands and their possible attributes
                // TODO: add help page
                HelpCommand();
                break;
            case "wait":
                // Execute the wait function and its paremeters
                WaitCommand();
                break;
            case "water":
                // Execute the water function and its paremeters
                WaterCommand(keywords);
                break;
            case "plant":
                // Execute the plant function and its paremeters
                PlantCommand(keywords);
                break;
            case "inv":
            case "inventory":
                // Execute the inventory function
                InventoryCommand();
                break;
            case "sell":
                // Execute the sell function
                // TODO: sell function
                break;
            case "sacrifice":
                // Execute the sacrifice 
                SacrificeCommand(keywords);
                break;
            case "inspect":
                // Execute the inspect function
                InspectCommand(keywords);
                break;
            case "grow":
                if (debug.isEnabled)
                    GrowCommandDev();
                else
                    ThrowError("command not found. try 'help' for a list of commands.");
                break;
            case "harvest":
                HarvestCommand(keywords);
                break;
            case "clear":
                ClearCommand();
                break;
            case "togglecheats":
                debug.ToggleWindow();
                break;
            default:
                // Add an error log to the console -- alternatively, add a list of help

                ThrowError("command not found. try 'help' for a list of commands.");
                break;
        }

        UpdateConsoleLog();

    }


    #region commands

    private void HelpCommand()
    {
        DefaultHelpInstructions();
    }
    private void SacrificeCommand(string[] args)
    {
        if (args.Length < 2)
        {
            ThrowError("no quantity selected");
            return;
        }

        if (args.Length < 3)
        {
            ThrowError("no plant selected");
            return;
        }

        if (int.TryParse(args[1], out int amount))
        {
            switch (args[2])
            {
                case "wheat":
                    SacrificeCheckAndExecute(Plant.PlantType.wheat, amount);
                    break;
                case "potato":
                    SacrificeCheckAndExecute(Plant.PlantType.potato, amount);
                    break;
                case "flower":
                    SacrificeCheckAndExecute(Plant.PlantType.flower, amount);
                    break;
                default:
                    ThrowError("incorrect seed type given.");
                    break;
            }
        }
        else if (int.TryParse(args[2], out amount))
        {
            switch (args[1])
            {
                case "wheat":
                    SacrificeCheckAndExecute(Plant.PlantType.wheat, amount);
                    break;
                case "potato":
                    SacrificeCheckAndExecute(Plant.PlantType.potato, amount);
                    break;
                case "flower":
                    SacrificeCheckAndExecute(Plant.PlantType.flower, amount);
                    break;
                default:
                    ThrowError("incorrect seed type given.");
                    break;
            }
        }
        else
        {
            ThrowError("incorrect amount given.");
            SacrificeCommandHelp(true);
            return;
        }
    }
    private void SacrificeCheckAndExecute(Plant.PlantType type, int amount)
    {
        switch (type)
        {
            case Plant.PlantType.wheat:
                if (debug.wheatCrops >= amount)
                {
                    godSystem.Sacrifice(type, amount);
                }
                break;
            case Plant.PlantType.potato:
                if (debug.potatoCrops >= amount)
                {
                    godSystem.Sacrifice(type, amount);
                }
                break;
            case Plant.PlantType.flower:
                if (debug.flowerCrops >= amount)
                {
                    godSystem.Sacrifice(type, amount);
                }
                break;
        }
    }

    private void WaterCommand(string[] args)
    {
        if (args.Length < 2)
        {
            ThrowError("can't water: no location given");
            WaterCommandHelp(true);
            return;
        }


        if (args[1] == "all")
        {
            AudioManager.Instance.Play(Water_Sound);
            commandLogHistory.Add($"watered {CC.Color(locationMessageColor)}all tiles.");
            foreach (FarmingTile tile in tiles)
            {
                tile.Water();
            }
            return;
        }

        bool validTile = true;
        List<FarmingTile> validTiles = new List<FarmingTile>();
        Dictionary<string, FarmingTile> tileNames = new Dictionary<string, FarmingTile>();
        foreach (FarmingTile tile in tiles)
        {
            tileNames.Add(tile.name, tile);
        }
        for (int i = 1; i < args.Length; i++)
        {
            if (!tileNames.Keys.Contains(args[i]))
            {
                validTile = false;
            }
            else
            {
                validTiles.Add(tileNames[args[i]]);
            }
        }
        if (!validTile)
        {
            ThrowError("given tile does not exist");
            WaterCommandHelp(true);
            return;
        }

        AudioManager.Instance.Play(Water_Sound);

        // If valid tiles are given, water plants
        // TODO: Change error message from FarmingTile to how it was done in the plant function (skipping certain tiles)
        commandLogHistory.Add($"watered on tiles{CC.Color(locationMessageColor)} {string.Join(" ", args.Where(e => e != args[0]))}");
        foreach (FarmingTile tile in validTiles)
        {
            tile.Water();
        }




    }

    private void ClearCommand()
    {
        commandLogHistory.Clear();
        UpdateConsoleLog();
    }

    private void HarvestCommand(string[] args)
    {

        if (args.Length < 2)
        {
            ThrowError("can't harvest: no location given");
            HarvestCommandHelp(true);
            return;
        }

        if (args[1] == "all")
        {
            AudioManager.Instance.Play(Harvest_Sound, 0.2f);

            commandLogHistory.Add($"harvested {CC.Color(locationMessageColor)}all tiles.");
            foreach (FarmingTile tile in tiles)
            {
                if (tile.CanHarvest())
                {
                    // TODO: check if plant was healthy and able to be added to inventory
                    // TODO: Randomise the number of seeds you get back
                    plantInventory.Add(tile.plant.plantType);
                    seedInventory.Add(tile.plant.plantType);
                    tile.Harvest();

                }
            }
            return;
        }

        bool validTile = true;
        List<FarmingTile> validTiles = new List<FarmingTile>();
        Dictionary<string, FarmingTile> tileNames = new Dictionary<string, FarmingTile>();
        foreach (FarmingTile tile in tiles)
        {
            tileNames.Add(tile.name, tile);
        }
        for (int i = 1; i < args.Length; i++)
        {
            if (!tileNames.Keys.Contains(args[i]))
            {
                validTile = false;
            }
            else
            {
                validTiles.Add(tileNames[args[i]]);
            }
        }
        if (!validTile)
        {
            ThrowError("given tile does not exist");
            HarvestCommandHelp(true);
            return;
        }

        // If valid tiles are given, Harvest plants
        // TODO: Change error message from FarmingTile to how it was done in the plant function (skipping certain tiles)
        commandLogHistory.Add($"harvested on tiles{CC.Color(locationMessageColor)} {string.Join(" ", args.Where(e => e != args[0]))}");
        AudioManager.Instance.Play(Harvest_Sound, 0.2f);

        foreach (FarmingTile tile in validTiles)
        {
            if (tile.CanHarvest())
            {
                // TODO: check if plant was healthy and able to be added to inventory
                plantInventory.Add(tile.plant.plantType);
                seedInventory.Add(tile.plant.plantType);
                tile.Harvest();

            }
        }


    }

    private void GrowCommandDev()
    {
        commandLogHistory.Add($"<color=yellow>DEV TOOL<color=white>: grew {CC.Color(locationMessageColor)}all available plants");
        foreach (FarmingTile tile in tiles)
        {
            tile.GrowDebug();
        }
    }

    private void InspectCommand(string[] args)
    {
        if (args.Length > 1)
        {
            foreach (FarmingTile tile in tiles)
            {
                if (tile.name == args[1])
                {
                    if (tile.plant == null)
                    {
                        commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{args[1]} <color=white>has {CC.Color(noPlantMessageColor)}no plant <color=white>on it");
                        return;
                    }
                    else
                    {
                        switch (tile.tileState)
                        {
                            case FarmingTile.TileHealthState.Dry:
                                commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(dryMessageColor)}dry {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                                break;
                            case FarmingTile.TileHealthState.Healthy:
                                commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(healthyMessageColor)}healthy {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                                break;
                            case FarmingTile.TileHealthState.Wet:
                                commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(drowningMessageColor)}wet {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                                break;
                            case FarmingTile.TileHealthState.Very_Dry:
                                commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(dryMessageColor)}very dry {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it. It is dying.");
                                break;
                            case FarmingTile.TileHealthState.Drowning:
                                commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(drowningMessageColor)}drowning {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it. It is dying.");
                                break;
                        }
                        return;
                    }
                }


            }
            ThrowError("incorrect tile.");
        }
        else
        {
            foreach (FarmingTile tile in tiles)
            {
                if (tile.plant == null)
                {
                    commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has {CC.Color(noPlantMessageColor)}no plant <color=white>on it");
                }
                else
                {
                    switch (tile.tileState)
                    {
                        case FarmingTile.TileHealthState.Dry:
                            commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(dryMessageColor)}dry {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                            break;
                        case FarmingTile.TileHealthState.Healthy:
                            commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(healthyMessageColor)}healthy {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                            break;
                        case FarmingTile.TileHealthState.Wet:
                            commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(drowningMessageColor)}wet {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it.");
                            break;
                        case FarmingTile.TileHealthState.Very_Dry:
                            commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(dryMessageColor)}very dry {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it. It is dying.");
                            break;
                        case FarmingTile.TileHealthState.Drowning:
                            commandLogHistory.Add($"tile {CC.Color(locationMessageColor)}{tile.name} <color=white>has a {CC.Color(drowningMessageColor)}drowning {tile.plant.GetGrowthStage()} {tile.plant.plantType} plant on it. It is dying.");
                            break;
                    }
                }
            }
            UpdateConsoleLog();


        }
    }

    private void PlantCommand(string[] args)
    {
        // Check if there are arguments
        if (args.Length <= 2)
        {
            ThrowError("could not plant: missing locations and/or type");
            PlantCommandHelp(true);
            return;
        }

        if (args[1] != "wheat" && args[1] != "potato" && args[1] != "flower")
        {
            ThrowError("given seed type invalid");
            PlantCommandHelp(true);

            return;
        }

        if ((args[1] == "potato" && unlockStage < 2) || (args[1] == "flower" && unlockStage < 3))
        {
            ThrowError("given seed type invalid");
            PlantCommandHelp(true);

            return;
        }

        // Check for missing seeds
        if ((args[1] == "wheat" && !seedInventory.Contains(Plant.PlantType.wheat)) || (args[1] == "potato" && !seedInventory.Contains(Plant.PlantType.potato)) || (args[1] == "flower" && !seedInventory.Contains(Plant.PlantType.flower)))
        {
            ThrowError("no seeds of this type available");
            return;
        }

        if (args[2] == "all")
        {
            int plantableTiles = 0;
            int seedShortages = 0;
            List<string> plantedTiles = new List<string>();
            List<string> skippedTiles = new List<string>();

            foreach (FarmingTile tile in tiles)
            {
                if (tile.CanPlant())
                {
                    plantableTiles++;
                    if (args[1] == "wheat")
                    {
                        if (seedInventory.Contains(Plant.PlantType.wheat))
                        {
                            tile.PlantSeed(Plant.PlantType.wheat);
                            seedInventory.Remove(Plant.PlantType.wheat);
                            plantedTiles.Add(tile.name);
                        }
                        else
                        {
                            seedShortages += 1;
                            skippedTiles.Add(tile.name);
                        }
                    }
                    else if (args[1] == "potato")
                    {
                        if (seedInventory.Contains(Plant.PlantType.potato))
                        {
                            tile.PlantSeed(Plant.PlantType.potato);
                            seedInventory.Remove(Plant.PlantType.potato);
                            plantedTiles.Add(tile.name);
                        }
                        else
                        {
                            seedShortages += 1;
                            skippedTiles.Add(tile.name);
                        }
                    }
                    else
                    {
                        if (seedInventory.Contains(Plant.PlantType.flower))
                        {
                            tile.PlantSeed(Plant.PlantType.flower);
                            seedInventory.Remove(Plant.PlantType.flower);
                            plantedTiles.Add(tile.name);
                        }
                        else
                        {
                            seedShortages += 1;
                            skippedTiles.Add(tile.name);
                        }
                    }
                }
            }

            if (plantableTiles == 0)
            {
                ThrowError("no tiles available to plant.");
            }

            else if (seedShortages == 0)
            {
                AudioManager.Instance.Play(Plant_Sound);
                commandLogHistory.Add($"planted {CC.Color(plantMessageColor)}{args[1]} <color=white>on{CC.Color(locationMessageColor)} all available squares");
            }
            else
            {
                AudioManager.Instance.Play(Plant_Sound);
                commandLogHistory.Add($"planted {CC.Color(plantMessageColor)}{args[1]} <color=white>on{CC.Color(locationMessageColor)} {string.Join(" ", plantedTiles)}<color=white>. skipped {CC.Color(errorMessageColor)}{string.Join(" ", skippedTiles)} <color=white>- no seeds");
            }
        }
        else
        {
            List<string> validTiles = new List<string>();
            int plantableTiles = 0;
            int seedShortages = 0;
            List<string> plantedTiles = new List<string>();
            List<string> skippedTiles = new List<string>();
            bool validTile = true;
            Dictionary<string, FarmingTile> tileNames = new Dictionary<string, FarmingTile>();
            foreach (FarmingTile tile in tiles)
            {
                tileNames.Add(tile.name, tile);
            }
            for (int i = 2; i < args.Length; i++)
            {
                if (!tileNames.Keys.Contains(args[i]))
                {
                    validTile = false;
                }
                else if (!validTiles.Contains(args[i]) && tileNames[args[i]].CanPlant())
                {
                    validTiles.Add(args[i]);
                }
            }
            if (!validTile)
            {
                ThrowError("given tile does not exist");
                PlantCommandHelp(true);

            }
            else
            {
                // PLANT ON TILES
                if (validTiles.Count == 0)
                {
                    ThrowError("there is already a plant at this location");
                }
                else
                {
                    foreach (FarmingTile tile in tiles)
                    {
                        foreach (string validtile in validTiles)
                        {
                            if (tile.name == validtile && tile.CanPlant())
                            {
                                plantableTiles++;
                                if (args[1] == "wheat")
                                {
                                    if (seedInventory.Contains(Plant.PlantType.wheat))
                                    {
                                        tile.PlantSeed(Plant.PlantType.wheat);
                                        seedInventory.Remove(Plant.PlantType.wheat);
                                        plantedTiles.Add(tile.name);
                                    }
                                    else
                                    {
                                        seedShortages += 1;
                                        skippedTiles.Add(tile.name);
                                    }
                                }
                                else if (args[1] == "potato")
                                {
                                    if (seedInventory.Contains(Plant.PlantType.potato))
                                    {
                                        tile.PlantSeed(Plant.PlantType.potato);
                                        seedInventory.Remove(Plant.PlantType.potato);
                                        plantedTiles.Add(tile.name);
                                    }
                                    else
                                    {
                                        seedShortages += 1;
                                        skippedTiles.Add(tile.name);
                                    }
                                }
                                else
                                {
                                    if (seedInventory.Contains(Plant.PlantType.flower))
                                    {
                                        tile.PlantSeed(Plant.PlantType.flower);
                                        seedInventory.Remove(Plant.PlantType.flower);
                                        plantedTiles.Add(tile.name);
                                    }
                                    else
                                    {
                                        seedShortages += 1;
                                        skippedTiles.Add(tile.name);
                                    }
                                }

                            }
                        }
                    }
                    if (plantableTiles == 0)
                    {
                        ThrowError("no tiles available to plant.");
                    }
                    else if (seedShortages == 0)

                    {
                        AudioManager.Instance.Play(Plant_Sound);
                        commandLogHistory.Add($"planted {CC.Color(plantMessageColor)}{args[1]} <color=white>on{CC.Color(locationMessageColor)} {string.Join(" ", plantedTiles)}");
                    }
                    else
                    {
                        AudioManager.Instance.Play(Plant_Sound);
                        commandLogHistory.Add($"planted {CC.Color(plantMessageColor)}{args[1]} <color=white>on{CC.Color(locationMessageColor)} {string.Join(" ", plantedTiles)}<color=white>. skipped {CC.Color(errorMessageColor)}{string.Join(" ", skippedTiles)} <color=white>- no seeds");
                    }
                }
            }
        }
    }

    private void WaitCommand()
    {
        AudioManager.Instance.Play(Wait_Sound);

        // WAIT ONE DAY
        currentDay++;

        godSystem.NextDay();
        weatherSystem.NextDay();

        if (godSystem.CheckForGameOver() && !isGameOver)
        {
            ClearCommand();
            commandLogHistory.Add("Oh no! The gods are angry and have destroyed your land. Try to please them next time!");
            ThrowError("GAME OVER!");
            UpdateConsoleLog();
            consoleInput.interactable = false;
            isGameOver = true;
            return;
        }

        string logMessage = "waiting until next day.. ";

        if (godSystem.happiness == GodSystem.Happiness.Sad)
        {
            AudioManager.Instance.Play(Storm_Ambient_Sound);
            logMessage += "the Gods are sad and have thrown heavy rain upon your lands.";
            weatherSystem.weatherType = WeatherEventSystem.WeatherType.Storm;
        }
        else if (godSystem.happiness == GodSystem.Happiness.Angry)
        {
            AudioManager.Instance.Play(Drought_Ambient_Sound);
            logMessage += "the Gods are angry and drain your land of water.";
            weatherSystem.weatherType = WeatherEventSystem.WeatherType.Drought;
        }
        else
        {
            switch (weatherSystem.weatherType)
            {
                case WeatherEventSystem.WeatherType.Normal:
                    logMessage += "the sun is shining..";
                    AudioManager.Instance.Play(Ambient_Birds_Sound);
                    break;
                case WeatherEventSystem.WeatherType.Rain:
                    AudioManager.Instance.Play(Rain_Ambient_Sound);
                    logMessage += "a light drizzle falls upon the farmland..";
                    break;
                case WeatherEventSystem.WeatherType.Storm:
                    AudioManager.Instance.Play(Storm_Ambient_Sound);
                    logMessage += "heavy storms arrived..";
                    break;
                case WeatherEventSystem.WeatherType.Drought:
                    AudioManager.Instance.Play(Drought_Ambient_Sound);
                    logMessage += "a scorching sun brings a heavy drought..";
                    break;
            }
        }

        // TODO: Multiple different messages for the sun, rain and storm console logs
        // TODO: Add thunderstorm warning and lightning message

        if (logMessage.Length > maxCharactersPerLine)
        {
            commandLogHistory.Add($"{logMessage.Substring(0, maxCharactersPerLine)}");
            commandLogHistory.Add($"{logMessage.Substring(maxCharactersPerLine).Trim()}");
        }
        else
        {
            commandLogHistory.Add($"{logMessage}");
        }
        UpdateConsoleLog();
        foreach (FarmingTile tile in tiles)
        {
            tile.NextDay(weatherSystem.weatherType, weatherSystem.weatherEvent);
        }


    }

    public void InventoryCommand()
    {
        AudioManager.Instance.Play(Inventory_Sound);
        int wheatSeed = 0, potatoSeed = 0, flowerSeed = 0;
        int wheat = 0, potato = 0, flower = 0;

        foreach (Plant.PlantType plant in seedInventory)
        {
            switch (plant)
            {
                case Plant.PlantType.wheat:
                    wheatSeed++;
                    break;
                case Plant.PlantType.potato:
                    potatoSeed++;
                    break;
                case Plant.PlantType.flower:
                    flowerSeed++;
                    break;
            }
        }
        foreach (Plant.PlantType plant in plantInventory)
        {
            switch (plant)
            {
                case Plant.PlantType.wheat:
                    wheat++;
                    break;
                case Plant.PlantType.potato:
                    potato++;
                    break;
                case Plant.PlantType.flower:
                    flower++;
                    break;
            }
        }

        commandLogHistory.Add("Inventory:");
        if (unlockStage >= 1)
        {
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{wheatSeed} <color=white>x {CC.Color(wheatColor)}wheat seeds");
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{wheat} <color=white>x {CC.Color(wheatColor)}wheat");
        }
        if (unlockStage >= 2)
        {
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{potatoSeed} <color=white>x {CC.Color(potatoColor)}potato seeds");
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{potato} <color=white>x {CC.Color(potatoColor)}potato");
        }
        if (unlockStage >= 3)
        {
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{flowerSeed} <color=white>x {CC.Color(flowerColor)}flower seeds");
            commandLogHistory.Add($"  {CC.Color(quantityMessageColor)}{flower} <color=white>x {CC.Color(flowerColor)}flower");

        }
        UpdateConsoleLog();

    }

    #endregion


    public void UpdateConsoleLog()
    {
        if (commandLogHistory.Count > maxConsoleLines)
        {
            for (int i = 0; i < commandLogHistory.Count - maxConsoleLines; i++)
            {
                commandLogHistory.RemoveAt(0);
            }
        }

        consoleLog.text = "";
        for (int i = 0; i < commandLogHistory.Count; i++)
        {
            consoleLog.text += "<color=#499FDB>>  </color><color=white>" + commandLogHistory[i] + "\n";
        }
    }

    public void GodsMessage(string message)
    {
        AudioManager.Instance.Play(Gods_Enter_Sound);

        if (message.Length > maxCharactersPerLine)
        {
            commandLogHistory.Add($"{CC.Color(errorMessageColor)}{message.Substring(0, maxCharactersPerLine)}");
            commandLogHistory.Add($"{CC.Color(errorMessageColor)}{message.Substring(maxCharactersPerLine).Trim()}");
        }
        else
        {
            commandLogHistory.Add($"{CC.Color(errorMessageColor)}{message}");
        }
        UpdateConsoleLog();
    }

    public void ThrowError(string message)
    {
        commandLogHistory.Add($"{CC.Color(errorMessageColor)}{message}");
        UpdateConsoleLog();
        AudioManager.Instance.Play(Error_Command_Sound, 0.2f);

    }

    public void AddToInventory(int count, Plant.PlantType type)
    {
        for (int i = 0; i < count; i++)
        {
            seedInventory.Add(type);
        }
    }


    #region helpCommands

    private void DefaultHelpInstructions()
    {
        commandLogHistory.Add($"Color hierarchy: ");
        commandLogHistory.Add($"  - {CC.Color(plantMessageColor)}[plants]");
        commandLogHistory.Add($"  - {CC.Color(locationMessageColor)}[locations]");
        commandLogHistory.Add($"  - {CC.Color(quantityMessageColor)}[quantities]");
        commandLogHistory.Add($"  - {CC.Color(errorMessageColor)}[ALL option]");
        commandLogHistory.Add($"  - [optional brackets]");

        commandLogHistory.Add($"List of currently available commands:");
        PlantCommandHelp(false);
        WaterCommandHelp(false);
        HarvestCommandHelp(false);
        InspectCommandHelp(false);
        commandLogHistory.Add($"  - WAIT");
        commandLogHistory.Add($"  - INVENTORY/INV");
        commandLogHistory.Add($"  - CLEAR (clears log)");
    }

    private void PlantCommandHelp(bool isError)
    {
        if (isError)
        {
            ThrowError($"Correct usage: PLANT {CC.Color(plantMessageColor)}[seed] {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...<color=white>]");
        }
        else
        {
            commandLogHistory.Add($"  - PLANT {CC.Color(plantMessageColor)}[seed] {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...<color=white>]");
        }
    }

    private void WaterCommandHelp(bool isError)
    {
        if (isError)
        {
            ThrowError($"Correct usage: WATER {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...<color=white>]");
        }
        else
        {
            commandLogHistory.Add($"  - WATER {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...<color=white>]");
        }
    }

    private void InspectCommandHelp(bool isError)
    {
        if (isError)
        {
            ThrowError($"Correct usage: INSPECT {CC.Color(locationMessageColor)}[location<color=white>]");
        }
        else
        {
            commandLogHistory.Add($"  - INSPECT {CC.Color(locationMessageColor)}[location<color=white>]");
        }
    }

    private void HarvestCommandHelp(bool isError)
    {
        if (isError)
        {
            ThrowError($"Correct usage: HARVEST {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...]");
        }
        else
        {
            commandLogHistory.Add($"  - HARVEST {CC.Color(locationMessageColor)}[location]<color=white>/{CC.Color(errorMessageColor)}[ALL] <color=white>[{CC.Color(locationMessageColor)}[location] ...]");
        }
    }

    private void SacrificeCommandHelp(bool isError)
    {
        if (isError)
        {

        }
        else
        {
            commandLogHistory.Add($"  - SACRIFICE {CC.Color(plantMessageColor)}[plant] {CC.Color(quantityMessageColor)}[quantity]");
        }
    }


    #endregion

}
#region Color Conversion

public class CC
{
    public static string Color(Color color)
    {
        string temp = "<color=#";
        temp += UnityEngine.ColorUtility.ToHtmlStringRGB(color);
        temp += ">";
        return temp;
    }
}
#endregion




