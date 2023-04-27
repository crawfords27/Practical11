using Microsoft.EntityFrameworkCore;
using SMS.Data.Entities;
using SMS.Data.Repository;
using SMS.Data.Security;

namespace SMS.Data.Services;

// EntityFramework Implementation of IUserService
public class UserServiceDb : IUserService
{
    private readonly DataContext db;

    public UserServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== User Authentication/Registration Management ==================
    public User Authenticate(string email, string password)
    {
        // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
        var user = GetUserByEmail(email);

        // Verify the user exists and Hashed User password matches the password provided
        return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;
    }
    
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
            Name = name,
            Email = email,
            Password = Hasher.CalculateHash(password),
            Role = role 
        };

        db.Users.Add(user);
        db.SaveChanges();
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
        
        db.SaveChanges();
        return user;
    }

    public User GetUserByEmail(string email)
    {
        return db.Users.FirstOrDefault(u => u.Email == email);
    }

    public User GetUser(int id)
    {
        return db.Users.FirstOrDefault(u => u.Id == id);
    }

}

