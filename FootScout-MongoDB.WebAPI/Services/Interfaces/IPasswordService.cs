namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(string password);
    }
}