using BlueNotation.Data;
using BlueNotation.Game;
using System.Xml.Serialization;

namespace BlueNotation.Services;

public class DataService
{
    private readonly LocalStorageService _storageService;

    public Statistics Statistics { get; set; } = new();
    public PresetsData PresetsData { get; set; } = new();

    public DataService(LocalStorageService localStorageService)
    {
        _storageService = localStorageService;
    }

    public async Task SaveData()
    {
        await _storageService.SaveData(SerializeData());
    }

    public async Task LoadData()
    {
        var xml = await _storageService.LoadData();

        if (xml is not null)
        {
            try
            {
                DeserializeData(xml);
                return;
            }
            catch (Exception e) when (e is InvalidOperationException || e is ArgumentException)
            {
                Console.WriteLine("Local storage was corrupt (data).");
            }
        }
        else
        {
            Console.WriteLine("Found no local storage (data).");
        }

        Statistics.UnloadData();
    }

    public string SerializeData()
    {
        Statistics.LoadData();

        var serializer = new XmlSerializer(Statistics.GetType());

        using StringWriter textWriter = new();
        serializer.Serialize(textWriter, Statistics);

        return textWriter.ToString();
    }

    public void DeserializeData(string xml)
    {
        using var reader = new StringReader(xml);
        var deserializer = new XmlSerializer(Statistics.GetType());

        if (deserializer.Deserialize(reader) is Statistics stats)
        {
            Statistics = stats;

            Statistics.UnloadData();
            return;
        }

        throw new ArgumentException("Could not be deserialized.", nameof(xml));
    }

    public async Task SavePresets()
    {
        await _storageService.SavePresets(SerializePresets());
    }

    public async Task LoadPresets()
    {

        var xml = await _storageService.LoadPresets();

        if (xml is not null)
        {
            try
            {
                DeserializePresets(xml);
                return;
            }
            catch (Exception e) when (e is InvalidOperationException || e is ArgumentException)
            {
                Console.WriteLine("Local storage was corrupt (presets).");
            }
        }
        else
        {
            Console.WriteLine("Found no local storage (presets).");
        }
        
    }

    public string SerializePresets()
    {
        var serializer = new XmlSerializer(PresetsData.GetType());

        using StringWriter textWriter = new();
        serializer.Serialize(textWriter, PresetsData);

        return textWriter.ToString();
    }

    public void DeserializePresets(string xml)
    {
        using var reader = new StringReader(xml);
        var deserializer = new XmlSerializer(PresetsData.GetType());

        if (deserializer.Deserialize(reader) is PresetsData presetData)
        {
            PresetsData = presetData;
            return;
        }

        throw new ArgumentException("Could not be deserialized.", nameof(xml));
    }
}
