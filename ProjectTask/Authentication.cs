using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class Authentication
    {
        //Properties

        
        public string UserName {  get; set; }
        public string Password { get; set; }

        //Private Fields
        private const string _CorrectUsername = "Admin";
        private const string _CorrectPassword = "password";

        public Authentication(string username, string password)
        {
            UserName = username;
            Password = password;
        }
        public bool Authenticate()
        {
            return UserName == _CorrectUsername && Password == _CorrectPassword;
        }
    }
}
