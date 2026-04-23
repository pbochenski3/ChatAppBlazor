namespace ChatApp.Application.Interfaces
{
    public interface ITokenProvider
    {
        Task<string?> GetToken();
    }
}
