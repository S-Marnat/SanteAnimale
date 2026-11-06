namespace SanteAnimale.Auth.Api.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public bool IsRevoked { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;
    }
}
