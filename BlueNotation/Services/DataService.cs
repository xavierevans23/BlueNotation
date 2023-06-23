using BlueNotation.Data;
using System;
using System.Xml.Serialization;

namespace BlueNotation.Services;

public class DataService
{
    public Statistics Statistics { get; private set; } = new();

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
}
