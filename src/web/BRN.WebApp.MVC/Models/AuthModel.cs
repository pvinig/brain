namespace BRN.WebApp.MVC.Models
{
    public class UserLoginResponse
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public ResponseResult ResponseResult { get; set; }

    }
}
