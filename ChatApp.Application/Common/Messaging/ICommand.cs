using MediatR;

namespace ChatApp.Application.Common.Messaging
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
