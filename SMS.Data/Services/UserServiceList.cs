
using SMS.Data.Entities;
using SMS.Data.Security;

namespace SMS.Data.Services;

public class UserServiceList : IUserService
{
    private readonly List<User> Users;

    public UserServiceList()
    {
        Users = new List<User>();
    }

    public void Initialise()
    {
        Users.Clear(); // delete all Users from list
    }

    // ------------------ Student Related Operations ------------------------

    public User Register(string name, string email, string password, Role role)
    {
        // check that the user does not already exist (unique user name)
        var exists = GetUserByEmail(email);
        if (exists != null)
        {
            return null;
        }

        // Custom Hasher used to encrypt the password before storing in database
        var user = new User 
        {
            Id = Users.Count + 1,
            Name = name,
            Email = email,
            Password = Hasher.CalculateHash(password),
            Role = role
        };

        Users.Add(user);       
        return user;
        
    }
    
    public User UpdateProfile(User updated)
    {
        // verify the user  exists 
        var user = GetUser(updated.Id);
        if (user == null)
        {
            return null;
        }

        // verify email is still unique
        var exists = GetUserByEmail(updated.Email);
        if (exists != null && exists.Id != updated.Id)
        {
            return null;
        }
        // update user profile
        user.Email = updated.Email;
        user.Name = updated.Name;
        
        // only update password or avatar if values provided
        if (!string.IsNullOrEmpty(updated.Password))
        {
            user.Password = Hasher.CalculateHash(updated.Password);
        }               
        
        return user;
    }

    public User Authenticate(string email, string password)
    {
        // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
        var user = GetUserByEmail(email);

        // Verify the user exists and Hashed User password matches the password provided
        return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
    }

    public User GetUserByEmail(string email)
    {
        return Users.FirstOrDefault(u => u.Email == email);
    }

    public User GetUser(int id)
    {
        return Users.FirstOrDefault(u => u.Id == id);
    }

}

   