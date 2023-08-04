using MediatR;

namespace Domain.AuthMiddleware;

public class CommandQueryAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authAttributeList = Attribute.GetCustomAttributes(typeof(TRequest), true);
        var userAccessResults = authAttributeList.Map(CurrentUserHasAccessToCommandOrQuery);

        if(!userAccessResults.Any(x => true))
            throw new AccessViolationException($"Current user does not have access to {typeof(TRequest).Name}"); 

        return await next();
    }

    public Boolean CurrentUserHasAccessToCommandOrQuery(Attribute attribute) {
        return attribute.GetType().Name switch {
            nameof(AdminAccess) => userIsAdminUser(),
            nameof(ApiUserAccess) => userIsApiUser(),
            _ => false  
        };
    }


    //Handled by claims identity usually dummy functions for this example
    public bool userIsAdminUser() => true;
    public bool userIsApiUser() => false;
}
