namespace Catalog.Core.Domain.Entities
{
    public class User
    {
        public int IdUser { get; set; }
        
        
        public User() { }

        public User(int idUser)
        {
            IdUser = idUser;
           
        }
    }
}