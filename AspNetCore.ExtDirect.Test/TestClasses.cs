using AspNetCore.ExtDirect.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Test
{
    public class TestHandler
    {
        private readonly IExtDirectTransactionService _transactionService;

        public TestHandler(IExtDirectTransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _transactionService.TransactionRollback += (sender, e) =>
            {

            };
        }

        public List<TestListItem> EchoList(List<TestListItem> value)
        {
            return value;
        }

        public TestPerson EchoPerson(TestPerson value)
        {
            return value;
        }

        public async Task<TestPerson> GetPerson()
        {
            var person = new TestPerson { FirstName = "John", LastName = "Doe" };
            return await Task.FromResult(person);
        }
    }

    public class TestPollingHandler
    {
        public IEnumerable<PollResponse> GetEvents(object args)
        {
            for (var i = 0; i < 1000; i++)
            {
                yield return new PollResponse { Name = "ondata", Data = "i = " + i };
            }
        }
    }

    public class TestPersonAddress
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string StreetAddressLine { get; set; }
        public string Building { get; set; }
    }

    public class TestPerson
    {
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public TestPersonAddress Address { get; set; }
        public List<string> Phones { get; } = new List<string>();
    }

    public class TestListItem
    {
        public object A { get; set; }
        public object B { get; set; }
        public object C { get; set; }
    }

    public class DemoNamedArguments
    {
        public string A;
        public double B;
        public int C;
    }

    public class DemoOrderedArguments
    {
        public string A;
        public double B;
        public DateTime C;
    }
}
