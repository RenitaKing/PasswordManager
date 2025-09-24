using System;
using System.Security.Cryptography.X509Certificates;
using SplashKitSDK;

namespace PasswordManager 
{
    public static class Program
    {
        public static AuthenticationService Auth = new AuthenticationService();

        //Get our password store and entries:
        public static PasswordStore Store = new PasswordStore();
        public static EncryptionService Crypto = new EncryptionService();

        public static void Main()
        {
            UIResources.LoadResources();
            //Get our app controller running:
            AppController app = new AppController(Auth, Store, Crypto);
            app.Run();

        }
    }
}

