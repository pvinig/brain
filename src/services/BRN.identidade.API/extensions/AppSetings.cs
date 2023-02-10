namespace BRN.identidade.API.extensions
{
    public class AppSetings
    {
        public string Secret { get; set; }

        public int ExpiracaoHoras { get; set; }

        public string Emissor { get; set; }

        public string ValidoEm { get; set; }
    }
}
