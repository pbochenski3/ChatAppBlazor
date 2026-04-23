using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Contact.GetUserContacts
{
    public class GetUserContactsHandler : IRequestHandler<GetUserContactsQuery, List<ContactDTO>>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IContactRepository _contactRepo;
        public GetUserContactsHandler(IChatRepository chatRepo, IContactRepository contactRepo)
        {
            _chatRepo = chatRepo;
            _contactRepo = contactRepo;
        }
        public async Task<List<ContactDTO>> Handle(GetUserContactsQuery r, CancellationToken cancellationToken)
        {

            var isArchive = await _chatRepo.CheckIfChatIsArchive(r.ChatId, r.UserId);
            if (isArchive == true)
            {
                throw new Exception("Nie można pobrać listy!");
            }
            var contacts = await _contactRepo.GetAllContactsAsync(r.UserId);
            if (contacts == null || !contacts.Any())
            {
                return new List<ContactDTO>();
            }

            return contacts.Select(c => new ContactDTO
            {
                ContactUserID = c.ContactUserID,
                Username = c.ContactUser.Username,
                AvatarUrl = c.ContactUser.AvatarUrl,
                IsOnline = c.ContactUser.IsOnline,
            }).ToList();
        }
    }
}
