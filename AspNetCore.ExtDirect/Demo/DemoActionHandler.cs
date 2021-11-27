using System;
using System.Threading.Tasks;
using System.Transactions;

namespace AspNetCore.ExtDirect.Demo
{
    public class PersonAddress
    {
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class Person
    {
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public PersonAddress Address { get; set; }
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

        public async Task<string> Hello(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "No name provided");
            }
            return await Task.FromResult("Hello, " + name + "!");
        }

        public object OrderedArguments(string a, double b, DateTime c)
        {
            return new { a, b, c };
        }

        [ExtDirectNamedArgs]
        public object NamedArguments(string a = "Some string from server", double? b = 3.14, int? c = 2021)
        {
            return new { a, b, c };
        }

        public string MakeName(Person personName)
        {
            return $"{personName.Prefix} {personName.FirstName} {personName.LastName}".Trim();
        }

        public void DoSomethingWithTransaction()
        {

        }
    }
}
