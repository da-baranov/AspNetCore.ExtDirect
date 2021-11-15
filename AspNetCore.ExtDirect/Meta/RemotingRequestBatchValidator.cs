using FluentValidation;

namespace AspNetCore.ExtDirect.Meta
{
    /// <summary>
    /// TODO: check Tid uniqueness
    /// </summary>
    internal sealed class RemotingRequestBatchValidator : AbstractValidator<RemotingRequestBatch>
    {
        public RemotingRequestBatchValidator()
        {
        }
    }
}
