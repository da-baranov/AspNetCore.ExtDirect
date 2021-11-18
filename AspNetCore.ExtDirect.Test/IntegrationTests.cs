using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Test
{
    public class IntegrationTests
    {
        private HttpClient _client;
        private TestApplicationFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new System.Uri("http://localhost:10000/")
            });
        }

        [Test]
        public async Task Test_ComplexArguments()
        {
            var personName = new { Prefix = "Mr.", FirstName = "John", LastName = "Doe" };
            var rv = await _client.CallOrdered<string>("Demo", "makeName", personName);
            Assert.AreEqual(rv, $"{personName.Prefix} {personName.FirstName} {personName.LastName}".Trim());
        }

        [Test]
        public async Task Test_ComplexData()
        {
            var person = new TestPerson
            {
                Prefix = "Mr",
                FirstName = "John",
                LastName = "Doe",
                GivenName = "Adam",
                Address = new TestPersonAddress
                {
                    Country = "Sweden",
                    City = "Stockholm",
                    StreetAddressLine = "Drottningsgatan",
                    Building = "1"
                }
            };
            person.Phones.Add("+46 11 1234 5678");
            var result = await _client.CallOrdered<TestPerson>("Test", "EchoPerson", person);
            Assert.IsTrue(result.Prefix == person.Prefix &&
                          result.FirstName == person.FirstName &&
                          result.LastName == person.LastName &&
                          result.Address != null &&
                          result.Address.StreetAddressLine == person.Address.StreetAddressLine &&
                          result.Phones[0] == person.Phones[0]);
        }

        [Test]
        public async Task Test_LargeList()
        {
            var list = new List<TestListItem>();
            for (var i = 0; i < 10000; i++)
            {
                list.Add(new TestListItem { A = Guid.NewGuid().ToString(), B = Guid.NewGuid().ToString(), C = Guid.NewGuid().ToString() });
            }
            var result = await _client.CallOrdered<List<TestListItem>>("Test", "EchoList", list);
            Assert.IsTrue(list.Count == result.Count);
        }

        [Test]
        public async Task Test_NamedArguments()
        {
            var i = new DemoNamedArguments { A = "111", B = Math.PI, C = DateTime.Now.Year };
            var o = await _client.CallNamed<DemoNamedArguments>("Demo", "namedArguments", i);
            Assert.IsTrue(i.A == o.A &&
                          i.C == o.C);
        }

        [Test]
        public async Task Test_OrderedArguments()
        {
            var a = Guid.NewGuid().ToString();
            var b = DateTime.Now.Year;
            var c = DateTime.Now;
            var o = await _client.CallOrdered<DemoOrderedArguments>("Demo", "orderedArguments", a, b, c);
            Assert.IsTrue(o.A == a && o.C == c);
        }

        [Test]
        public async Task Test_Polling()
        {
            var events = await _client.CallPolling();
            Assert.IsTrue(events.Count != 0);
        }
    }
}