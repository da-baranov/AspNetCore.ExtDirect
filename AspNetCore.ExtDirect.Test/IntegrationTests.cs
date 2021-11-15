using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Test
{
    public class IntegrationTests
    {
        private TestApplicationFactory _factory;
        private HttpClient _client;

        public class DemoOrderedArguments
        {
            public string A;
            public double B;
            public DateTime C;
        }

        public class DemoNamedArguments
        {
            public string A;
            public double B;
            public int C;
        }

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
        public async Task Test_OrderedArguments()
        {
            var a = Guid.NewGuid().ToString();
            var b = DateTime.Now.Year;
            var c = DateTime.Now;
            var o = await _client.CallOrdered<DemoOrderedArguments>("Demo", "orderedArguments", a, b, c);
            Assert.IsTrue(o.A == a && o.C == c);
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
        public async Task Test_ComplexArguments()
        {
            var personName = new { Prefix = "Mr.", FirstName = "John", LastName = "Doe" };
            var rv = await _client.CallOrdered<string>("Demo", "makeName", personName);
            Assert.AreEqual(rv, $"{personName.Prefix} {personName.FirstName} {personName.LastName}".Trim());
        }

        [Test]
        public async Task Test_Polling()
        {
            var events = await _client.CallPolling();
            Assert.IsTrue(events.Count != 0);
        }
    }
}