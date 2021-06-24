namespace OSCalendar.Domain.Convertors
{
    public interface IFormatConverter
    {
        public string Serialize<TFrom>(TFrom from);
        public TFrom Deserialize<TFrom>(string serializedText);
    }
}