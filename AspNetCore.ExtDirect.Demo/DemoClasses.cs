using AspNetCore.ExtDirect.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Demo
{
    public class NamedArguments
    {
        public string A;
        public double? B;
        public int? C;
    }

    public class OrderedArguments
    {
        public string A;
        public double B;
        public DateTime C;
    }

    public class Person
    {
        public PersonAddress Address { get; set; }
        public string FirstName { get; set; }
        public string GivenName { get; set; }
        public int Id { get; set; }
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

    public class PollingService
    {
        public IEnumerable<dynamic> GetEvents()
        {
            var r = new Random();

            var a = Convert.ToInt32(r.NextDouble() * 10000);
            var b = Convert.ToInt32(r.NextDouble() * 10000);
            BigInteger c = a * b;
            var data = $"{a} * {b} = {c}";
            yield return data;
        }
    }

    public class PollingTestHandler
    {
        public async Task<IEnumerable> GetEvents(Person testPerson)
        {
            var result = new List<int>();
            for (var i = 0; i < 1000; i++)
            {
                result.Add(i);
            }
            return await Task.FromResult(result);
        }
    }

    public class RemotingCalculatorService
    {
        private readonly ILogger<RemotingCalculatorService> _logger;

        public RemotingCalculatorService(ILogger<RemotingCalculatorService> logger)
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

    /// <summary>
    /// This class handles both Remoting and Polling requests
    /// </summary>
    public class RemotingChatService
    {
        private readonly DemoDbContext _dbContext;

        public RemotingChatService(DemoDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Polling request handler
        /// </summary>
        public IEnumerable<ChatMessage> GetEvents()
        {
            var messages = _dbContext
                .Messages
                .Where(row => row.Read == false);
            foreach (var message in messages)
            {
                message.Read = true;
                yield return message;
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Remoting request handler
        /// </summary>
        public void SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Say something please");
            }
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
    public class RemotingTestHandler
    {
        private readonly DemoDbContext _dbContext;

        public RemotingTestHandler(DemoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<int>> CreatePersons(List<Person> persons)
        {
            persons.ForEach((p) =>
            {
                p.Id = 0;
                if (string.IsNullOrWhiteSpace(p.LastName))
                {
                    throw new Exception("Last name is required");
                }
                if (string.IsNullOrWhiteSpace(p.FirstName))
                {
                    throw new Exception("First name is required");
                }
            });
            _dbContext.Persons.AddRange(persons);
            await _dbContext.SaveChangesAsync();
            return persons.Select(row => row.Id).ToList();
        }

        public async Task<object> DeletePersons(List<Person> persons)
        {
            if (persons != null)
            {
                persons.ForEach((pd) =>
                {
                    var ep = _dbContext.Attach(pd);
                    _dbContext.Remove(pd);
                });
            }
            await _dbContext.SaveChangesAsync();
            return new { Success = true };
        }

        public List<TestListItem> EchoList(List<TestListItem> value)
        {
            return value;
        }

        public Person EchoPerson(Person value)
        {
            return value;
        }

        /// <see href="https://docs.sencha.com/extjs/7.0.0/classic/Ext.form.action.DirectSubmit.html"/>
        /// <see href="https://student.ku.ac.th/history/ext-4.0-beta1/docs/api/Ext.form.action.DirectSubmit.html"/>
        [ExtDirectFormSubmit]
        public async Task<string> FormSubmit(Person person, List<IFormFile> files)
        {
            if (string.IsNullOrWhiteSpace(person.LastName))
            {
                throw new Exception("Last name required");
            }
            return await Task.FromResult("OK");
        }

        public async Task<Person> GetPerson()
        {
            var person = new Person { FirstName = "John", LastName = "Doe" };
            return await Task.FromResult(person);
        }

        public async Task<List<Person>> GetPersons(StoreRequest request)
        {
            var q = _dbContext.Persons.AsQueryable();

            if (request != null && !string.IsNullOrWhiteSpace(request.Filter))
            {
                q = q.Where(row => row.LastName.Contains(request.Filter) ||
                                   row.FirstName.Contains(request.Filter) ||
                                   row.GivenName.Contains(request.Filter));
            }
            return await q
                .OrderBy(row => row.LastName)
                .ThenBy(row => row.FirstName)
                .ToListAsync();
        }

        public async Task<string> Hello(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "No name provided");
            }
            return await Task.FromResult("Hello, " + name + "!");
        }

        [ExtDirectIgnore]
        public string Hidden()
        {
            return "This a hidden method that is not exposed to clients";
        }
        public string MakeName(Person personName)
        {
            return $"{personName.Prefix} {personName.FirstName} {personName.LastName}".Trim();
        }

        [ExtDirectNamedArgs]
        public NamedArguments NamedArguments(string a = "Some string from server", double? b = 3.14, int? c = 2021)
        {
            return new NamedArguments { A = a, B = b, C = c };
        }

        public OrderedArguments OrderedArguments(string a, double b, DateTime c)
        {
            return new OrderedArguments { A = a, B = b, C = c };
        }
    }

    public class StoreRequest
    {
        public string Filter { get; set; }
        public int? Limit { get; set; }
        public int? Page { get; set; }
        public int? Start { get; set; }
    }

    public class TestListItem
    {
        public object A { get; set; }
        public object B { get; set; }
        public object C { get; set; }
    }
}