using ChatApp.Application.Common.Messaging;
using ChatApp.Application.Interfaces;
using MediatR;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse> // Interesują nas tylko komendy
{
    private readonly IUnitOfWork _uow;

    public TransactionBehavior(IUnitOfWork uow) => _uow = uow;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _uow.Begin(); // Start "koszyka"
        try
        {
            var response = await next(); // Wykonanie Handlera
            await _uow.CommitAsync();    // Zapis do bazy
            return response;
        }
        catch
        {
            // Błąd? Nic nie zapisujemy, Dispose wszystko wyczyści
            throw;
        }
        finally
        {
            _uow.Dispose(); // Zwrot połączenia do puli
        }
    }
}