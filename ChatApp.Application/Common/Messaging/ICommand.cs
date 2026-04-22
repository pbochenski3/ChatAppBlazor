using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Common.Messaging
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
