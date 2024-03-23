using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Entities
{
    public class User
    {
        public int Id { get; set;  }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<File> Files { get; set; }

        public User()
        {
            
        }
        public User(string fName, string lName, string password, string email) {
            this.FirstName = fName;
            this.LastName = lName;
            this.Email = email;
            this.Password = password;
        }

    }
}
