namespace Model
{
    public class ApplicationUserDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string NomeEmpresa { get; set; }

        public string Secret { get; set; }
        public string Id { get; set; }
        public string Provider { get; set; }
        //public string Role { get; set; }
        public string ProviderId { get; set; }

    }
}
