using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Common.Messaging
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
