using System;
using System.Collections;
using System.Collections.Generic;

namespace StackExchange.Redis.Collections
{
    public class RedisList<T> : IList<T>
    {
        readonly IDatabase database;
        readonly string redisKey;

        public RedisList(IDatabase database, string keyName)
        {
            if (database == null)
                throw new ArgumentNullException($"{nameof(database)} parameter can't be null.");

            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentNullException($"{nameof(keyName)} parameter can't be null.");

            this.Serializer = new DefaultJsonConverter<T>();
            this.Deserialize = new DefaultJsonConverter<T>();
            this.redisKey = keyName;
            this.database = database;
        }

        public ISerializer<T> Serializer { get; private set; }

        public IDeserialize<T> Deserialize { get; private set; }

        public int Count
        {
            get
            {
                long length = database.ListLength(redisKey);

                if (length > int.MaxValue)
                    throw new OverflowException("List length on Redis exceeded maximum value of integer.");

                return (int)length;
            }

        }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                var redisValue = GetValueByIndex(index);
                var value = this.Deserialize.Deserialize(redisValue);
                return value;
            }
            set
            {
                Insert(index, value);
            }
        }

        private string GetValueByIndex(int index)
        {
            var redisValue = this.database.ListGetByIndex(redisKey, index);
            if (!redisValue.HasValue)
                throw new IndexOutOfRangeException("Index is out of range");

            return redisValue;
        }

        public int IndexOf(T item)
        {
            int index = 0;

            foreach (var value in this)
            {
                if (EqualityComparer<T>.Default.Equals(value, item))
                    return index;

                index++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            var transaction = database.CreateTransaction();
            transaction.AddCondition(Condition.ListIndexExists(redisKey, index));

            //Find value at index
            var oldValue = GetValueByIndex(index);

            //Set index with keyFlag
            var keyFlag = Guid.NewGuid().ToString();
            transaction.ListSetByIndexAsync(redisKey, index, keyFlag);

            //Insert the new value before the keyFlag
            var value = this.Serializer.Serialize(item);
            transaction.ListInsertBeforeAsync(redisKey, keyFlag, value);

            //Restore keyFlag with its old value
            transaction.ListInsertAfterAsync(redisKey, keyFlag, oldValue);

            //Remove keyFlag from list
            transaction.ListRemoveAsync(redisKey, keyFlag);

            bool committed = transaction.Execute();
        }

        public void RemoveAt(int index)
        {
            var deleteFlag = Guid.NewGuid().ToString();
            try
            {
                this.database.ListSetByIndex(redisKey, index, deleteFlag);
            }
            catch (RedisServerException ex)
            {
                if (ex.Message.Equals("ERR index out of range"))
                    throw new IndexOutOfRangeException("Index is out of range");
            }
            this.database.ListRemove(redisKey, deleteFlag, flags: CommandFlags.FireAndForget);
        }

        public void Add(T item)
        {
            var value = this.Serializer.Serialize(item);
            this.database.ListRightPush(redisKey, value);
        }

        public void Clear()
        {
            this.database.KeyDelete(redisKey);
        }

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException($"{nameof(array)} parameter can't be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)}");

            if (arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)}");

            int destinationArraySize = array.Length - arrayIndex;
            if (destinationArraySize < this.Count)
                throw new ArgumentOutOfRangeException("Destination array size is less than the collection's size.");


            foreach (var value in this)
            {
                array[arrayIndex++] = value;
            }
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}