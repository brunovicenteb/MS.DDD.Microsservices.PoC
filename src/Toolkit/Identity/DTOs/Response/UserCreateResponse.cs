namespace Toolkit.Identity.DTOs.Response
{
    public class UserCreateResponse
    {
        public bool Sucess { get; private set; }
        public List<string> Errors { get; private set; }

        public UserCreateResponse() =>
            Errors = new List<string>();

        public UserCreateResponse(bool sucesso = true) : this() =>
            Sucess = sucesso;

        public void AddErrors(IEnumerable<string> errors) =>
            Errors.AddRange(errors);
    }
}