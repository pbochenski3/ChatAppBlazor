using ChatApp.Application.Common.Messaging;
using ChatApp.Application.Interfaces;
using MediatR;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUnitOfWork _uow;

    public TransactionBehavior(IUnitOfWork uow) => _uow = uow;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _uow.Begin(); 
        try
        {
            var response = await next(); 
            await _uow.CommitAsync();    
            return response;
        }
        catch
        {
            throw;
        }
        finally
        {
            _uow.Dispose(); 
        }
    }
}