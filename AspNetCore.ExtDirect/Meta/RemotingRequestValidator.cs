using FluentValidation;

namespace AspNetCore.ExtDirect.Meta
{
    internal sealed class RemotingRequestValidator : AbstractValidator<RemotingRequest>
    {
        public RemotingRequestValidator()
        {
            RuleFor(row => row.Type).NotNull();
            RuleFor(row => row.Action).NotNull().NotEmpty();
            RuleFor(row => row.Method).NotNull().NotEmpty();
            RuleFor(row => row.Tid).GreaterThan(0);
        }
    }
}
