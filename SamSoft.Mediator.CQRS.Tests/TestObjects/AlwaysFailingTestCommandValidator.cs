using FluentValidation;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class AlwaysFailingTestCommandValidator : AbstractValidator<TestCommand>
{
    public AlwaysFailingTestCommandValidator()
    {
        RuleFor(x => x.Value)
            .Must(x=> x.Contains("fail", StringComparison.CurrentCultureIgnoreCase));
    }
}