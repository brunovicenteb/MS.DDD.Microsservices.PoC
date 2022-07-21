using Refit;
using Benefit.Service.APIs.TheAudioDb.DTO;

namespace Benefit.Service.APIs.TheAudioDb;

public interface ITheAudioDbApiClient
{
    [Get("/api/v1/json/2/search.php?s={name}")]
    Task<TheAudioDbResponse> Search(string name);
}