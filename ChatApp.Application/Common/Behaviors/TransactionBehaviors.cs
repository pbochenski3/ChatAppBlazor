using ChatApp.Application.Common.Messaging;
using ChatApp.Application.Interfaces;
using MediatR;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public TransactionBehavior(IUnitOfWork uow, IMediator mediator)
    {
        _uow = uow;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var response = await next();

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            if (request is IHasDomainEvents requestWithEvents)
            {
                foreach (var domainEvent in requestWithEvents.DomainEvents)
                {

                    await _mediator.Publish(domainEvent, ct);
                }
                requestWithEvents.ClearDomainEvents();
            }
            return response;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            _uow.Dispose();
        }
    }
}