namespace StackExchange.Redis.Collections
{
    public interface IDeserialize<T>
    {
        T Deserialize(string value);
    }
}
