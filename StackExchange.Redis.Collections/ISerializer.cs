namespace StackExchange.Redis.Collections
{
    public interface ISerializer<T>
    {
        string Serialize(T value);
    }
}
