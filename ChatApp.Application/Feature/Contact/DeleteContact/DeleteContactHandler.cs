using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Notifications.Contact;
using ChatApp.Domain.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Contact.DeleteContact
{
    public class DeleteContactHandler : IRequestHandler<DeleteContactCommand, bool>
    {
        private readonly IContactRepository _contactRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IMediator _mediator;
        public DeleteContactHandler(IContactRepository contactRepo, IUserChatRepository userChatRepo, IMediator mediator, IChatRepository chatRepo)
        {
            _contactRepo = contactRepo;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
            _chatRepo = chatRepo;
        }
        public async Task<bool> Handle(DeleteContactCommand r, CancellationToken cancellationToken)
        {
            var contactId = await _userChatRepo.GetReceiverUserIdAsync(r.PrivateChatId, r.UserId, cancellationToken);
            if (contactId == Guid.Empty)
            {
                return false;
            }
            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.PrivateChatId, r.UserId);
            if (isArchive)
            {
                return false;
            }
            await _contactRepo.DeleteContactAsync(contactId, r.UserId);
            await _userChatRepo.ArchivePrivateChatAsync(r.PrivateChatId, r.UserId, contactId);
            r.AddEvent(new ContactDeletedNotification(contactId, r.UserId, r.PrivateChatId));
            return true;
        }
    }
}
