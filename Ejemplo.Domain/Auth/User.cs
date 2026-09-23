using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Ejemplo.Domain.FileSystem;
using Ejemplo.Domain.Social;

namespace Ejemplo.Domain.Auth
{
    public class User
    {
        private long id;
        
        public long Id {get => id; set => id = value;}
        
        private string name = string.Empty;
        
        public string Name {get => name; set => name = value;}

        private string lastname = string.Empty;
        
        public string LastName {get => lastname; set => lastname = value;}

        private string username = string.Empty;
        
        public string UserName {get => username; set => username = value;}

        private string email = string.Empty;
        
        public string Email {get => email; set => email = value;}
        
        private string password = string.Empty;
        
        public string Password {get => password; set => password = value;}


        
        private Image? image = null;
        
        public virtual Image? Image {get => image; set => image = value;}

        private static string encrypt(string password)
        {
            
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] byteHash = SHA1.HashData(bytes);


            return (Convert.ToBase64String(byteHash));
        }

        public virtual void SetPassword(string password)
        {
            this.password = User.encrypt(password);
        }

        public virtual bool IsPassword(string password)
        {
            string encryptedpassword = User.encrypt(password);
            if (this.password == encryptedpassword)
            {
                return true;
            }

            return false;
        }

        public string? GetAvatarUrl()
        {
            if (this.Image != null)
            {
                return "image/" + this.Image.ImageUrl();
            }

            return null;
        }
    }
}