
using System.Security.Cryptography;
using System.Text;
using Ejemplo.Domain.FileSystem;


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

        //Nuevos campos
        //RefreshToken
        private string? refreshToken = null;
        public string? RefreshToken {get => refreshToken; set => refreshToken = value;}

        private DateTime? refreshTokenExpiryAt = null;
        public DateTime? RefreshTokenExpiryAt {get => refreshTokenExpiryAt; set => refreshTokenExpiryAt = value;}
        
        //Reset Password

        private string? resetPasswordToken = null;
        public string? ResetPasswordToken {get => resetPasswordToken; set => resetPasswordToken = value;}

        private DateTime? resetPasswordExpiryAt = null;
        public DateTime? ResetPasswordExpiryAt {get => resetPasswordExpiryAt; set => resetPasswordExpiryAt = value;}

        //Auditoria / soft delete

        private DateTime? createdAt = DateTime.UtcNow;
        public DateTime? CreatedAt {get => createdAt; set => createdAt = value;}

        private bool isDeleted = false;
        public bool IsDeleted {get => isDeleted; set => isDeleted = value;}

        //Metodos

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

        // HELPERS NUEVOS (para no meter lógica en el controller)
        /// <summary>
        /// Genera y asigna un refresh token nuevo con expiración.
        /// </summary>
         public virtual string GenerateRefreshToken(int daysValid = 7)
        {
            string token = Guid.NewGuid().ToString("N");
            this.RefreshToken = token;
            this.RefreshTokenExpiryAt = DateTime.UtcNow.AddDays(daysValid);
            return token;
        }
         /// <summary>
        /// Indica si el refresh token actual está vigente.
        /// </summary>
        public virtual bool HasValidRefreshToken()
        {
            return !string.IsNullOrEmpty(this.RefreshToken)
                && this.RefreshTokenExpiryAt.HasValue
                && this.RefreshTokenExpiryAt.Value > DateTime.UtcNow;
        }

        /// <summary>
        /// Limpia el refresh token (logout).
        /// </summary>
        public virtual void ClearRefreshToken()
        {
            this.RefreshToken = null;
            this.RefreshTokenExpiryAt = null;
        }

        /// <summary>
        /// Genera un token de reset password con expiración.
        /// </summary>
        public virtual string GenerateResetPasswordToken(int hoursValid = 1)
        {
            string token = Guid.NewGuid().ToString("N");
            this.ResetPasswordToken = token;
            this.ResetPasswordExpiryAt = DateTime.UtcNow.AddHours(hoursValid);
            return token;
        }

        /// <summary>
        /// Indica si el token de reset está vigente.
        /// </summary>
        public virtual bool HasValidResetToken()
        {
            return !string.IsNullOrEmpty(this.ResetPasswordToken)
                && this.ResetPasswordExpiryAt.HasValue
                && this.ResetPasswordExpiryAt.Value > DateTime.UtcNow;
        }

        /// <summary>
        /// Limpia el token de reset (después de usarlo).
        /// </summary>
        public virtual void ClearResetPasswordToken()
        {
            this.ResetPasswordToken = null;
            this.ResetPasswordExpiryAt = null;
        }
    }
}