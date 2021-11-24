using AspNetCore.ExtDirect.Meta;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AspNetCore.ExtDirect.Demo
{
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
}
