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
        /// Defines a route that AspNetCore Ext Direct controller uses to handle client Remoting Manager requests, e.g /ExtDirect
        /// </summary>
        public string RemotingRouteUrl { get; set; } = "ExtDirect";

        /// <summary>
        /// Defines a route that AspNetCore Ext Direct controller uses to handle client Polling Manager requests, e.g /ExtDirectEvents
        /// </summary>
        public string PollingRouteUrl { get; set; } = "ExtDirectEvents";
    }

    internal class ExtDirectOptionsValidator : AbstractValidator<ExtDirectOptions>
    {
        public ExtDirectOptionsValidator()
        {
            RuleFor(options => options.PollingRouteUrl)
                .NotNull()
                .NotEmpty();

            RuleFor(options => options.RemotingRouteUrl)
                .NotNull()
                .NotEmpty()
                .NotEqual(o => o.PollingRouteUrl);
        }
    }
}