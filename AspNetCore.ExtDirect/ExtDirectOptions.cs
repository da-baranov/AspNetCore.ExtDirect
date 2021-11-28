using FluentValidation;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// AspNetCore Ext Direct options
    /// </summary>
    public class ExtDirectOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExtDirectOptions()
        {
        }

        /// <summary>
        /// Defines a route that describes client-side API and should be referenced by client ExtJS application
        /// </summary>
        /// <example>
        /// &lt;script src="~/ExtDirect.js"&gt;&lt;/script&gt;
        /// </example>
        public string Url { get; set; } = "ExtDirect";

        public string ClientApiUrl => Url + ".js";

        public string RemotingEndpointUrl => Url + "/remoting";

        public string PollingEndpointUrl => Url + "/polling";
    }

    internal class ExtDirectOptionsValidator : AbstractValidator<ExtDirectOptions>
    {
        public ExtDirectOptionsValidator()
        {
            RuleFor(options => options.Url)
                .NotNull()
                .NotEmpty();
        }
    }
}