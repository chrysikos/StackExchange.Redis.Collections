using NUnit.Framework;
using StackExchange.Redis;
using StackExchange.Redis.Collections;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class RedisListTests
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
                "test:234AE631-C12D-4D66-A5D4-F28DC24F08D4", //00
                "test:53C2789B-812E-4CE6-B10E-2F17FFC86088", //01
                "test:57BEA014-ABC2-4D31-B602-EB6412BE672E", //02
                "test:AD8ED6AC-26EB-49F6-BF34-76171517B6A0", //03
                "test:E1B97976-4F6C-44B2-A312-2B241684B230", //04
                "test:AD5717B7-C423-4DCC-91E3-BD64500F411F", //05
                "test:37AB61FE-26DF-4B29-8B49-1CDF041CF79F", //06
                "test:CCCFBCA4-49EC-4A97-8481-7BD20C2E7B1B", //07
                "test:4087B762-7059-4D7E-9EB2-510D36FEE395", //08
                "test:D9DDCC8F-26AA-42E2-B851-F3B5A13B7397", //09
                "test:FAD8C2A4-D650-4F69-AC7A-A12FA2AA92AA", //10
                "test:8ADA7609-4CFD-4B60-B622-A36B71B7EBEF", //11
                "test:AEF9C1FB-61F0-4E68-84AF-7B14740CF3BA", //12
                "test:5AF5C0F0-7EE7-4BA9-87D8-DB58A59CF5A2", //13
                "test:ED1DC91C-D509-4464-A4C5-CE4FA0D6CF23", //14
                "test:F2FCC2BD-B0BA-4E2D-B159-9B5B97E9C6E5", //15
                "test:7C76042C-AB02-4EC1-939D-52EA59F7F799", //16
                "test:F8258127-1325-4079-8E2A-E7E1824C2162", //17
                "test:B2C993D6-7620-4B0A-8059-F01DE1EAFFCB", //18
                "test:B2E121D4-644D-4689-8327-897631C8F87D", //19
            };


            database.KeyDelete(testKeyNames.ToArray());
        }
        [TearDown]
        public void TearDown()
        {
            database.KeyDelete(testKeyNames.ToArray());
        }
        [Test]
        public void Clear_Should_remove_all_objects_from_collection()
        {
            RedisList<Customer> customers = new RedisList<Customer>(database, testKeyNames[0]);
            customers.Add(new Customer() { FirstName = "Efstathios", LastName = "Chrysikos" });
            var excpectedLengthBeforeClear = customers.Count;
            Assert.AreEqual(excpectedLengthBeforeClear, 1);

            customers.Clear();
            var excpectedAfterClear = customers.Count;
            Assert.AreEqual(excpectedAfterClear, 0);
        }
        [Test]
        public void Initializer_Should_add_objects_to_collection()
        {
            RedisList<Customer> customers = new RedisList<Customer>(database, testKeyNames[1])
            {
                new Customer() { FirstName = "Efstathios", LastName = "Chrysikos" },
                new Customer() { FirstName = "Vasiliki", LastName = "Kourgia" },
                new Customer() { FirstName = "Dimitris", LastName = "Daskalopoulos" },
                new Customer() { FirstName = "Athanasios", LastName = "Chrysikos" },
            };
            var excpectedLength = 4;
            var actualLength = customers.Count;
            Assert.AreEqual(excpectedLength, actualLength);
        }
        [Test]
        public void Insert_Should_add_the_object_at_the_specified_index()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[2])
            {
                "0","1","2","4"
            };
            list.Insert(3, "3");

            var expectedValue = "3";
            var actualValue = list[3];
            Assert.AreEqual(expectedValue, actualValue);

            var expectedCount = 5;
            var actualCount = list.Count;
            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void Insert_Should_raise_an_Exception_When_specified_index_does_not_exist()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[3])
            {
                "0"
            };
            try
            {
                list.Insert(9, "3");
            }
            catch (Exception exception)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(exception);
                return;
            }

            Assert.Fail();
        }
        [Test]
        public void Indexer_Should_read_and_write_at_specified_index()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[4])
            {
                "0"
            };

            var expectedValueAt0Index = "0";
            var actualValueAt0Index = list[0];
            Assert.AreEqual(expectedValueAt0Index, actualValueAt0Index);

            list[0] = "1";

            expectedValueAt0Index = "1";
            actualValueAt0Index = list[0];
            Assert.AreEqual(expectedValueAt0Index, actualValueAt0Index);
        }
        [Test]
        public void Indexer_Should_raise_an_Exception_When_specified_index_does_not_exist()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[5])
            {
                "0"
            };

            try
            {
                var value = list[4];
                Assert.Fail("Indexer Failed thrown an IndexOutOfRangeException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }
            try
            {
                list[4] = "5";
                Assert.Fail("Indexer Failed thrown an IndexOutOfRangeException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }

        }
        [Test]
        public void Count_Should_return_the_length_of_collection()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[6]);
            for (int i = 0; i < 100; i++)
                list.Add(i);

            var expectedCount = 100;
            var actualCount = list.Count;

            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void IndexOf_Should_return_the_index_of_a_specific_object()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[7])
            {
                "a","b","c","d","e","f"
            };

            var expectedIndex = 2;
            var actualIndex = list.IndexOf("c");

            Assert.AreEqual(expectedIndex, actualIndex);
        }
        [Test]
        public void RemoveAt_Should_remove_the_object_at_the_specified_index()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[8])
            {
                "a","b","c","d","e","f"
            };

            list.RemoveAt(2);

            var expectedValue = "d";
            var actualValue = list[2];
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void RemoveAt_Should_raise_an_Exception_When_specified_index_does_not_exist()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[9])
            {
                "a","b","c","d","e","f"
            };

            try
            {
                list.RemoveAt(16);
                Assert.Fail("RemoveAt failed thrown an IndexOutOfRangeException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }
        }
        [Test]
        public void Add_Should_add_object_at_the_end_of_the_collection()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[10])
            {
                "a","b","c","d","e","f"
            };

            list.Add("g");

            var expectedValue = "g";
            var actualValue = list[list.Count - 1];

            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Contains_Should_determine_whether_the_collection_contains_a_specific_value()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[11])
            {
                "a","b","c","d","e","f"
            };

            var expectedValue = true;
            var actualValue = list.Contains("e");

            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void Remove_Should_removes_the_first_occurrence_of_a_specific_object()
        {
            RedisList<string> list = new RedisList<string>(database, testKeyNames[12])
            {
                "e","b","c","d","e","f"
            };

            var expectedValue = true;
            var actualValue = list.Remove("e");
            Assert.AreEqual(expectedValue, actualValue);


            var expectedCount = 5;
            var actualCount = list.Count;
            Assert.AreEqual(expectedCount, actualCount);


            var expectedValueAtIndex3 = "e";
            var actualValueAtIndex3 = list[3];
            Assert.AreEqual(expectedValueAtIndex3, actualValueAtIndex3);

            list.Remove("e");

            expectedValueAtIndex3 = "f";
            actualValueAtIndex3 = list[3];
            Assert.AreEqual(expectedValueAtIndex3, actualValueAtIndex3);

        }
        [Test]
        public void Constructor_Should_Raise_an_Exception_When_database_parameter_is_null()
        {
            try
            {
                RedisList<string> list = new RedisList<string>(null, testKeyNames[13]);
                Assert.Fail("Constructor fail to raise the ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void Constructor_Should_Raise_an_Exception_When_keyName_parameter_is_null_or_empty()
        {
            try
            {
                RedisList<string> list = new RedisList<string>(database, string.Empty);
                Assert.Fail("Constructor fail to raise the ArgumentNullException.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void GetEnumerator_Should_returns_an_enumerator_that_iterates_through_the_collection()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[14])
            {
                0,1,2,3,4,5,6,7,8,9
            };

            int expectedValue = 0;
            foreach (var item in list)
            {
                int actualValue = item;
                Assert.AreEqual(expectedValue, actualValue);
                expectedValue++;
            }
        }
        [Test]
        public void CopyTo_Should_copies_the_elements_of_the_collection_to_an_array_starting_at_a_particular_index_of_that_array()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[15])
            {
                0,1,2,3,4,5,6,7,8,9
            };

            int[] array = new int[11];
            array[0] = 999;
            list.CopyTo(array, 1);
            for (int i = 1; i < 11; i++)
            {
                var expectedValue = list[i - 1];
                var actualValue = array[i];
                Assert.AreEqual(expectedValue, actualValue);
            }

            Assert.AreEqual(999, array[0]);
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_array_parameter_is_null()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[16])
            {
                0,1,2,3,4,5,6,7,8,9
            };
            try
            {
                list.CopyTo(null, 0);
                Assert.Fail("CopyTo failed to raise an exception when array parameter is null.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_index_parameter_is_less_than_zero()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[17])
            {
                0,1,2,3,4,5,6,7,8,9
            };

            int[] array = new int[10];
            try
            {
                list.CopyTo(array, -1);
                Assert.Fail("CopyTo failed to raise an exception when index parameter is less than zero.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_array_parameter_length_is_less_than_the_size_of_the_collection()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[18])
            {
                0,1,2,3,4,5,6,7,8,9
            };


            int[] array = new int[5];
            try
            {
                list.CopyTo(array, 11);
                Assert.Fail("CopyTo failed to raise an exception when array size is less tha collection's size.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
        [Test]
        public void CopyTo_Should_raise_Exception_When_index_parameter_is_greater_than_array_parameter_length()
        {
            RedisList<int> list = new RedisList<int>(database, testKeyNames[19])
            {
                0,1,2,3,4,5,6,7,8,9
            };

            int[] array = new int[10];
            try
            {
                list.CopyTo(array, 11);
                Assert.Fail("CopyTo failed to raise an exception when index parameter is greater than array length.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }
    }
}