using MediatR;

namespace ChatApp.Application.Common.Messaging
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
