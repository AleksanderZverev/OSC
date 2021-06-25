using Newtonsoft.Json;

namespace OSCalendar.Domain.Convertors
{
    public class OSCFormatConverter: IFormatConverter
    {
        public string Serialize<TFrom>(TFrom from) => JsonConvert.SerializeObject(from);

        public TFrom Deserialize<TFrom>(string serializedText)
            => JsonConvert.DeserializeObject<TFrom>(serializedText);
    }
}