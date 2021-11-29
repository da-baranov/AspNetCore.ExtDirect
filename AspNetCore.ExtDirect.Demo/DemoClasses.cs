using AspNetCore.ExtDirect.Attributes;
using AspNetCore.ExtDirect.Meta;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Transactions;

namespace AspNetCore.ExtDirect.Demo
{
    /// <summary>
    /// This class handles both Remoting and Polling requests
    /// </summary>
    public class DemoChatHandler
    {
        private static int Id = 0;

        private static List<ChatMessage> Messages { get; } = new();

        /// <summary>
        /// Polling request handler
        /// </summary>
        public IEnumerable<PollResponse> GetEvents()
        {
            foreach (var message in Messages.Where(row => row.Read == false))
            {
                message.Read = true;
                yield return new PollResponse { Name = "onmessage", Data = message };
            }
        }

        /// <summary>
        /// Remoting request handler
        /// </summary>
        public void SendMessage(string message)
        {
            Messages.Add(new ChatMessage
            {
                Id = ++Id,
                Date = DateTime.Now,
                Message = message,
                Read = false
            });
        }
    }

    public class CalculatorService
    {
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(ILogger<CalculatorService> logger)
        {
            this._logger = logger;
        }

        public int Add(int a, int b)
        {
            _logger.LogInformation("Adding {0} and {1}", a, b);
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            _logger.LogInformation("Subtracting {0} and {1}", a, b);
            return a - b;
        }
    }

    public class ChatMessage
    {
        public DateTime Date { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        internal bool Read { get; set; } = false;
    }

    [ExtDirectAction("Demo")]
    public class DemoActionHandler
    {
        private readonly IExtDirectTransactionService _transactionService;
        private TransactionScope _transactionScope;

        public DemoActionHandler(IExtDirectTransactionService transactionService)
        {
            // transaction service can be consumed via standard AspNet.Core DI as follows:
            _transactionService = transactionService;
            _transactionService.TransactionBegin += (sender, e) =>
            {
                _transactionScope = new TransactionScope();
            };
            _transactionService.TransactionCommit += (sender, e) =>
            {
                if (_transactionScope != null)
                {
                    _transactionScope.Complete();
                    _transactionScope.Dispose();
                    _transactionScope = null;
                }
            };
            _transactionService.TransactionRollback += (sender, e) =>
            {
                if (_transactionScope != null)
                {
                    _transactionScope.Dispose();
                    _transactionScope = null;
                }
            };
        }

        public void DoSomethingTransactional()
        {
        }

        [ExtDirectIgnore]
        public string Hidden()
        {
            return "This a hidden method that is not exposed to clients";
        }

        public async Task<string> Hello(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "No name provided");
            }
            return await Task.FromResult("Hello, " + name + "!");
        }

        public string MakeName(Person personName)
        {
            return $"{personName.Prefix} {personName.FirstName} {personName.LastName}".Trim();
        }

        [ExtDirectNamedArgs]
        public DemoNamedArguments NamedArguments(string a = "Some string from server", double? b = 3.14, int? c = 2021)
        {
            return new DemoNamedArguments { A = a, B = b, C = c };
        }

        public DemoOrderedArguments OrderedArguments(string a, double b, DateTime c)
        {
            return new DemoOrderedArguments { A = a, B = b, C = c };
        }
    }

    public class DemoNamedArguments
    {
        public string A;
        public double? B;
        public int? C;
    }

    public class DemoOrderedArguments
    {
        public string A;
        public double B;
        public DateTime C;
    }

    public class DemoPollingHandler
    {
        public IEnumerable<PollResponse> GetEvents()
        {
            var r = new Random();

            var a = Convert.ToInt32(r.NextDouble() * 10000);
            var b = Convert.ToInt32(r.NextDouble() * 10000);
            BigInteger c = a * b;
            var data = $"{a} * {b} = {c}";
            yield return new PollResponse { Name = "ondata", Data = data };
        }
    }

    public class Person
    {
        public PersonAddress Address { get; set; }
        public string FirstName { get; set; }
        public string GivenName { get; set; }
        public string LastName { get; set; }
        public List<string> Phones { get; } = new List<string>();
        public string Prefix { get; set; }
    }

    public class PersonAddress
    {
        public string Building { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string StreetAddressLine { get; set; }
    }

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

        public Person EchoPerson(Person value)
        {
            return value;
        }

        public async Task<Person> GetPerson()
        {
            var person = new Person { FirstName = "John", LastName = "Doe" };
            return await Task.FromResult(person);
        }
    }

    public class TestListItem
    {
        public object A { get; set; }
        public object B { get; set; }
        public object C { get; set; }
    }

    public class TestPollingHandler
    {
        public async Task<IEnumerable<PollResponse>> GetEvents(Person testPerson)
        {
            var result = new List<PollResponse>();
            for (var i = 0; i < 1000; i++)
            {
                result.Add(new PollResponse { Name = "ondata", Data = "i = " + i });
            }
            return await Task.FromResult(result);
        }
    }
}