namespace BRN.identidade.API.extensions
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public int ExpirateTime { get; set; }

        public string Issuer { get; set; }  // emissor

        public string Audience { get; set; } // valido em
    }
}
