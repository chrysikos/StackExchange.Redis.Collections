using NUnit.Framework;
using StackExchange.Redis;
using StackExchange.Redis.Collections;
using System;
using System.Collections.Generic;


namespace Tests
{
    public class RedisDictionaryTests
    {
        IDatabase database = null;
        List<RedisKey> testKeyNames;

        [SetUp]
        public void Setup()
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.100.32:6379,password=1234");
            database = redis.GetDatabase();
            Assert.AreNotEqual(database, null);

            testKeyNames = new List<RedisKey>()
            {
                "test:1AF19746-9C84-4D80-912C-2108147BB82F", //00
                "test:1AF19746-9C84-4D80-912C-2108147BB82F", //01
                "test:5E3B3791-F053-4ACD-B0D8-AA7BB00153BA", //02
                "test:852F8ED8-55FC-4588-B69C-606A023F1A26", //03
                "test:91544364-5DBA-4D89-B6AE-ED80E848A951", //04
                "test:9D7A94A8-A7F1-43D7-964D-BF5EAD0CE86D", //05
                "test:24404112-083D-4420-B58C-1A6976571F51", //06
                "test:707FEF9A-4B89-4061-BF4F-690162FBFC2D", //07
                "test:BA2EDA80-27E6-4E69-A159-9DD737D48BAF", //08
                "test:ADA4983A-0ED6-41FE-A9D0-F62A1131E481", //09
                "test:C0DBE19E-67AA-439B-B0DB-0697DF173BBD", //10
                "test:E48075FB-9A9C-4FE1-871B-4AAA086B1F2D", //11
                "test:304B66AE-D487-4545-B7D4-14555BD76D25", //12
                "test:136B4A9D-90C6-42CC-9EAA-C4DCBAD5F7B8", //13
                "test:03B76AA5-2526-4A48-8F9E-20428B77BB26", //14
                "test:EB8AD855-78F8-4E97-9F00-0A12D4859A7E", //15
                "test:16B29352-E740-4C97-B396-1D5F4762434B", //16
                "test:AD40C87D-F3C3-40D5-BB2B-C35FD3653954", //17
                "test:6F092E01-29F7-4A1C-81EB-8622721233D4", //18
                "test:586B5636-BFF3-496C-B4C8-9A81EE374BD6", //19
                "test:9F0DDF76-57D7-472D-BB13-E5EEA93AD576", //20
                "test:D50A54C2-7653-4C2E-91CE-CE281033CA6E", //21
                "test:6B670FBA-ECF6-4017-8535-D0CD8BD8F7FC", //22
                "test:FC6776FA-932D-4655-A016-6C1A5BBEBACA", //23
                "test:19F7D669-7B33-43DF-8C97-7FEBA038857F", //24
                "test:9CC8768E-3FBE-4316-BC86-B21240276B66", //25

            };

            database.KeyDelete(testKeyNames.ToArray());
        }
        [TearDown]
        public void TearDown()
        {
            database.KeyDelete(testKeyNames.ToArray());
        }
        [Test]
        public void Indexer_Should_read_and_write_the_element_with_specified_key()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[0])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };


            var expectedValue = "Chrysikos";
            var actualValue = dictionary["1a-32"];
            Assert.AreEqual(expectedValue, actualValue);



            expectedValue = "Chry";
            dictionary["1a-32"] = expectedValue;
            actualValue = dictionary["1a-32"];
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Initializer_Should_adds_elements_with_the_provided_key_and_value_to_the_collection_through_the_add_method()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[1])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };


            var expectedValue = "Chrysikos";
            var actualValue = dictionary["1a-32"];
            Assert.AreEqual(expectedValue, actualValue);

            expectedValue = "Kourgia";
            actualValue = dictionary["4c-96"];
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Add_Should_adds_an_element_with_the_provided_key_and_value_to_the_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[2]);
            dictionary.Add("5f-81", "chrysikos");
            dictionary.Add(new KeyValuePair<string, string>("6f-92", "kourgia"));

            var expectedValue = "chrysikos";
            var actualValue = dictionary["5f-81"];
            Assert.AreEqual(expectedValue, actualValue);

            expectedValue = "kourgia";
            actualValue = dictionary["6f-92"];
            Assert.AreEqual(expectedValue, actualValue);

        }
        [Test]
        public void Add_Should_raise_an_ArgumentNullException_When_provided_key_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[3]);

            try
            {
                dictionary.Add(null, "ok");
                Assert.Fail("Add failed to raise an ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void Add_Should_raise_an_ArgumentException_When_an_element_with_the_same_key_already_exists_in_the_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[4])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            try
            {
                dictionary.Add("3c-96", "New Value");
                Assert.Fail("Add failed to raise an ArgumentException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentException>(ex);
            }
        }
        [Test]
        public void TryGetValue_Should_get_the_value_associated_with_the_specified_key()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[5])
              {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            var expectedValue = "Chrysikos";
            string actualValue = "";

            dictionary.TryGetValue("1a-32", out actualValue);

            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void TryGetValue_Should_raise_an_ArgumentNullException_When_the_provided_key_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[6])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            try
            {
                string actualValue = "";
                dictionary.TryGetValue(null, out actualValue);
                Assert.Fail("Add failed to raise an ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void TryGetValue_Should_return_true_When_element_with_the_specified_key_exists()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[7])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            var expectedValue = true;
            string result = "";
            var actualValue = dictionary.TryGetValue("1a-32", out result);

            Assert.AreEqual("Chrysikos", result);
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void TryGetValue_Should_return_false_When_element_with_the_specified_key_does_not_exists()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[8])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            var expectedValue = false;
            string result = null;
            var actualValue = dictionary.TryGetValue("5f-335", out result);

            Assert.AreEqual(null, result);
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Indexer_Should_raise_an_KeyNotFoundException_When_provided_key_does_not_exists()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[9])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            try
            {
                var value = dictionary["aa-10"];
                Assert.Fail("Add failed to raise an KeyNotFoundException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<KeyNotFoundException>(ex);
            }
        }
        [Test]
        public void Indexer_Should_raise_an_ArgumentNullException_When_the_provided_key_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[10])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("4c-96", "Kourgia" )
            };

            try
            {

                var value = dictionary[null];
                Assert.Fail("Add failed to raise an ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void Keys_Should_get_a_ICollection_containing_the_keys_of_the_Dictionary()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[11])
            {
                 new KeyValuePair<string, string>("1a-32","Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            ICollection<string> expectedKeys = new List<string>()
            {
                "1a-32",
                "6c-96",
                "8c-96",
                "3c-96"
            };

            ICollection<string> actualKeys = dictionary.Keys;

            int expectedCount = expectedKeys.Count;
            int actualCount = actualKeys.Count;
            Assert.AreEqual(expectedCount, actualCount);

            foreach (var expectedKey in expectedKeys)
            {
                var expectedValue = true;
                var actualValue = actualKeys.Contains(expectedKey);

                Assert.AreEqual(expectedValue, actualValue, $"The expectedKey={expectedKey} doesn't found.");
            }
        }
        [Test]
        public void Values_Should_get_a_ICollection_containing_the_values_of_the_Dictionary()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[12])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            ICollection<string> expectedValues = new List<string>()
            {
                "Chrysikos",
                "Kourgia",
                "Efstathios",
                "Vassiliki"
            };

            ICollection<string> actualValues = dictionary.Values;

            int expectedCount = expectedValues.Count;
            int actualCount = actualValues.Count;
            Assert.AreEqual(expectedCount, actualCount);



            foreach (var expectedValuesValue in expectedValues)
            {
                var expectedValue = true;
                var actualValue = actualValues.Contains(expectedValuesValue);

                Assert.AreEqual(expectedValue, actualValue, $"The expectedKey={expectedValuesValue} doesn't found.");
            }
        }
        [Test]
        public void ContainsKey_Should_determines_whether_the_collection_contains_an_element_with_the_specified_key()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[13])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            var expectedValue = true;
            var actualValue = dictionary.ContainsKey("8c-96");

            Assert.AreEqual(expectedValue, actualValue);


            expectedValue = false;
            actualValue = dictionary.ContainsKey("355-33");
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void ContainsKey_Should_raise_an_ArgumentNullException_When_provided_key_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[14])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            try
            {
                var actualValue = dictionary.ContainsKey(null);
                Assert.Fail("ContainsKey failed to raise an ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void Count_Should_gets_the_number_of_elements_contained_in_the_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[15])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            var expectedCount = 4;
            var actualCount = dictionary.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void Clear_Should_remove_all_items_from_the_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[16])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            var expectedCount = 4;
            var actualCount = dictionary.Count;
            Assert.AreEqual(expectedCount, actualCount);


            dictionary.Clear();

            expectedCount = 0;
            actualCount = dictionary.Count;
            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void Contains_Should_determines_whether_the_collection_contains_a_specific_value()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[17])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            var expectedValue = true;
            var actualValue = dictionary.Contains(new KeyValuePair<string, string>("8c-96", "Efstathios"));
            Assert.AreEqual(expectedValue, actualValue);

            expectedValue = false;
            actualValue = dictionary.Contains(new KeyValuePair<string, string>("c-6", "Value P"));
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Remove_Should_removes_the_element_with_the_specified_key_from_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[18])
            {
                 new KeyValuePair<string, string>("1a-32", "Chrysikos"),
                 new KeyValuePair<string, string>("6c-96", "Kourgia" ),
                 new KeyValuePair<string, string>("8c-96", "Efstathios"),
                 new KeyValuePair<string, string>("3c-96", "Vassiliki")
            };

            var expectedValue = true;
            var actualValue = dictionary.Remove("6c-96");
            Assert.AreEqual(expectedValue, actualValue);


            var expectedCount = 3;
            var actualCount = dictionary.Count;
            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void Remove_Should_raise_an_ArgumentNullException_When_provided_key_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[19]);

            try
            {
                dictionary.Remove(null);
                Assert.Fail("Remove failed to raise an ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void GetEnumerator_Should_returns_an_enumerator_that_iterates_through_the_collection()
        {
            RedisDictionary<string, string> actualDictionary = new RedisDictionary<string, string>(database, testKeyNames[20])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            var expectedDictionary = new Dictionary<string, string>()
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };


            int actualCount = 0;
            foreach (var actualItem in actualDictionary)
            {
                var expectedValue = true;
                var actualValue = expectedDictionary.ContainsKey(actualItem.Key);
                Assert.AreEqual(expectedValue, actualValue);
                actualCount++;
            }

            var expectedCount = 10;

            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void CopyTo_Should_copies_the_elements_of_the_collection_to_an_array_starting_at_a_particular_index_of_that_array()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[21])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            var array = new KeyValuePair<string, string>[10];
            dictionary.CopyTo(array, 0);


            foreach (var item in array)
            {
                var expectedValue = true;
                var actualValue = dictionary.Contains(item);
                Assert.AreEqual(expectedValue, actualValue);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_array_parameter_is_null()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[22])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            try
            {
                dictionary.CopyTo(null, 0);
                Assert.Fail("CopyTo failed to raise ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_index_parameter_is_less_than_zero()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[23])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            try
            {
                var array = new KeyValuePair<string, string>[10];
                dictionary.CopyTo(array, -1);
                Assert.Fail("CopyTo failed to raise ArgumentOutOfRangeException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_index_parameter_is_greater_than_array_parameter_length()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[24])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            try
            {
                var array = new KeyValuePair<string, string>[10];
                dictionary.CopyTo(array, 12);
                Assert.Fail("CopyTo failed to raise ArgumentOutOfRangeException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_array_parameter_length_is_less_than_the_size_of_the_collection()
        {
            RedisDictionary<string, string> dictionary = new RedisDictionary<string, string>(database, testKeyNames[25])
            {
                {"0","value0"},
                {"1","value1"},
                {"2","value2"},
                {"3","value3"},
                {"4","value4"},
                {"5","value5"},
                {"6","value6"},
                {"7","value7"},
                {"8","value8"},
                {"9","value9"}
            };

            try
            {
                var array = new KeyValuePair<string, string>[10];
                dictionary.CopyTo(array, 5);
                Assert.Fail("CopyTo failed to raise ArgumentOutOfRangeException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
        
    }
}
