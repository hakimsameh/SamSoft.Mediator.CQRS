namespace SamSoft.Mediator.CQRS.DefaultBehaviors;


public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>>? validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(TRequest request,
        HandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators is not null)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationFailures =
                await Task.WhenAll(
                    validators.Select(validator =>
                        validator.ValidateAsync(context, cancellationToken)));

            var errors = validationFailures
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .Select(failure => new ValidationError(
                    PropertyName: failure.PropertyName,
                    ErrorMessage: failure.ErrorMessage
                )).ToList();
            if (errors.Count != 0)
            {
                throw new CustomValidationException(errors);
            }
        }

        var response = await next(cancellationToken);
        return response;
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);

public class CustomValidationException(IEnumerable<ValidationError> errors)
    : Exception("Validation Failure")
{
    public IEnumerable<ValidationError> Errors { get; } = errors;
}
