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
        private DemoDbContext _dbContext;

        public DemoChatHandler(DemoDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Polling request handler
        /// </summary>
        public IEnumerable<PollResponse> GetEvents()
        {
            var messages = _dbContext.Messages.Where(row => row.Read == false);
            foreach (var message in messages)
            {
                message.Read = true;
                yield return new PollResponse { Name = "onmessage", Data = message };
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Remoting request handler
        /// </summary>
        public void SendMessage(string message)
        {
            var newMessage = new ChatMessage
            {
                Date = DateTime.Now,
                Message = message,
                Read = false
            };
            _dbContext.Messages.Add(newMessage);
            _dbContext.SaveChanges();
        }
    }

    public class CalculatorService
    {
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }

        public double Add(double a, double b)
        {
            _logger.LogInformation("Adding {0} and {1}", a, b);
            return a + b;
        }

        public double Subtract(double a, double b)
        {
            _logger.LogInformation("Subtracting {0} and {1}", a, b);
            return a - b;
        }
    }

    [ExtDirectAction("Demo")]
    public class DemoActionHandler
    {
        private readonly IExtDirectBatchService _transactionService;
        private TransactionScope _transactionScope;

        public DemoActionHandler(IExtDirectBatchService transactionService)
        {
            // transaction service can be consumed via standard AspNet.Core DI as follows:
            _transactionService = transactionService;
            _transactionService.BatchBegin += (sender, e) =>
            {
                _transactionScope = new TransactionScope();
            };
            _transactionService.BatchCommit += (sender, e) =>
            {
                if (_transactionScope != null)
                {
                    _transactionScope.Complete();
                    _transactionScope.Dispose();
                    _transactionScope = null;
                }
            };
            _transactionService.BatchException += (sender, e) =>
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
        public int Id { get; set; }
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
        private readonly IExtDirectBatchService _transactionService;

        public TestHandler(IExtDirectBatchService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _transactionService.BatchException += (sender, e) =>
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