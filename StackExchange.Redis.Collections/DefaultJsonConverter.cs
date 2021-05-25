
namespace StackExchange.Redis.Collections
{
    public class DefaultJsonConverter<T> : ISerializer<T>, IDeserialize<T>
    {
        public T Deserialize(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize(T value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
}
