using SMS.Data.Entities;

namespace SMS.Data.Services;
public interface IUserService
{       
    void Initialise();
    
    // ------------- User Management -------------------
    User Register(string name, string email, string password, Role role);
    User Authenticate(string email, string password);
    User GetUserByEmail(string email);
    User GetUser(int id);            
    User UpdateProfile(User updated);               

}
    
