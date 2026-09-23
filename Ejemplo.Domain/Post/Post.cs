using Ejemplo.Domain.Auth;

namespace Ejemplo.Domain.Post
{
    public class Post
    {
        private long id;

        public long Id { get => id; set => id = value; }

        private string body = string.Empty;

        public string Body { get => body; set => body = value; }

        private Ejemplo.Domain.FileSystem.File? file;

        public virtual Ejemplo.Domain.FileSystem.File? File { get => file; set => file = value; }

        private User? user;
        
        public virtual User? User { get => user; set => user = value; }
    }
}