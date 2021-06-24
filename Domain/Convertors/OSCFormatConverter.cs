using System.Text.Json;

namespace OSCalendar.Domain.Convertors
{
    public class OSCFormatConverter: IFormatConverter
    {
        public string Serialize<TFrom>(TFrom from) => JsonSerializer.Serialize(from);

        public TFrom Deserialize<TFrom>(string serializedText) => JsonSerializer.Deserialize<TFrom>(serializedText);
    }
}