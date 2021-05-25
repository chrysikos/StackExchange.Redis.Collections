using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange.Redis.Collections
{
    public class RedisDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDatabase database;
        private readonly string redisKey;

        private bool IsKeyNull(TKey key)
        {
            if (key == null)
                return true;
            else
                return false;
        }
        private void SetWith(TKey key, TValue value)
        {
            if (IsKeyNull(key))
                throw new ArgumentNullException(nameof(key));

            string serializedKey = KeySerializer.Serialize(key);
            string serializedValue = ValueSerializer.Serialize(value);
            this.database.HashSet(this.redisKey, serializedKey, serializedValue);
        }

        public ISerializer<TKey> KeySerializer { get; private set; }
        public IDeserialize<TKey> KeyDeserialize { get; private set; }
        public ISerializer<TValue> ValueSerializer { get; private set; }
        public IDeserialize<TValue> ValueDeserialize { get; private set; }


        public RedisDictionary(IDatabase database, string keyName)
        {
            this.redisKey = keyName;
            this.database = database;

            this.KeySerializer = new DefaultJsonConverter<TKey>();
            this.KeyDeserialize = new DefaultJsonConverter<TKey>();
            this.ValueSerializer = new DefaultJsonConverter<TValue>();
            this.ValueDeserialize = new DefaultJsonConverter<TValue>();
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException($"The specified key does not exists.");

                return value;
            }
            set
            {
                this.SetWith(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.database.HashKeys(this.redisKey)
                                    .Select(redisValue =>
                                    {
                                        return this.KeyDeserialize.Deserialize(redisValue);
                                    })
                                    .ToList();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return this.database.HashValues(this.redisKey)
                                    .Select(redisValue =>
                                    {
                                        return this.ValueDeserialize.Deserialize(redisValue);
                                    })
                                    .ToList();
            }
        }

        public int Count
        {
            get
            {
                long length = database.HashLength(this.redisKey);

                if (length > int.MaxValue)
                    throw new OverflowException("Length on Redis exceeded maximum value of integer.");

                return (int)length;
            }
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("The specified key already exists.");

            SetWith(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.database.KeyDelete(this.redisKey);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;

            if (!TryGetValue(item.Key, out value))
                return false;

            return EqualityComparer<TValue>.Default.Equals(item.Value, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (IsKeyNull(key))
                throw new ArgumentNullException($"{nameof(key)} can't be null.");

            var serializedKey = this.KeySerializer.Serialize(key);
            return database.HashExists(this.redisKey, serializedKey);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException($"{nameof(array)} parameter can't be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)} can't be less than zero.");

            if (arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)} can't be greater than collection's length.");

            int destinationArraySize = array.Length - arrayIndex;
            if (destinationArraySize < this.Count)
                throw new ArgumentOutOfRangeException("Destination array size is less than the collection's size.");


            foreach (var value in this)
            {
                array[arrayIndex++] = value;
            }
        }

        public bool Remove(TKey key)
        {
            if (IsKeyNull(key))
                throw new ArgumentNullException($"{nameof(key)} can't be null.");

            var serializedKey = this.KeySerializer.Serialize(key);
            return database.HashDelete(this.redisKey, serializedKey);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (IsKeyNull(key))
                throw new ArgumentNullException($"{nameof(key)} can't be null.");


            value = default(TValue);


            var serializedKey = this.KeySerializer.Serialize(key);
            var serializedValue = this.database.HashGet(this.redisKey, serializedKey);
            if (string.IsNullOrEmpty(serializedValue))
                return false;

            value = this.ValueDeserialize.Deserialize(serializedValue);
            return true;
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.database.HashScan(redisKey)
                              .Select(hashEntry =>
                                  {
                                      var deserializedKey = this.KeyDeserialize.Deserialize(hashEntry.Name);
                                      var deserializedValue = this.ValueDeserialize.Deserialize(hashEntry.Value);
                                      return new KeyValuePair<TKey, TValue>(deserializedKey, deserializedValue);
                                  })
                              .GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
