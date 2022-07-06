using Refit;
using Benefit.Service.Model;

namespace Benefit.Service.Interfaces;

public interface IImdbApiClient
{
    [Get("/en/API/SearchName/{key}/{name}")]
    Task<ImdbPerson> GetPerson(string key, string name);
}