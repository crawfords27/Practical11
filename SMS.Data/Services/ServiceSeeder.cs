using SMS.Data.Entities;

namespace SMS.Data.Services;

public static class ServiceSeeder
{
    // use this class to seed the database with dummy test data using an IStudentService 

    public static void Seed(IUserService userSvc, IStudentService studentSvc)
    {  
        // only do this once
        studentSvc.Initialise();

        SeedUsers(userSvc);
        SeedStudents(studentSvc);
    }

    // use this method FIRST to seed the database with dummy test data using an IUserService
    private static void SeedUsers(IUserService svc)
    {
        // Note: do not call initialise here
        var u1 = svc.Register("admin","admin@mail.com","password",Role.admin);
        var u2 = svc.Register("support","support@mail.com","password",Role.support);
        var u3 = svc.Register("guest","guest@mail.com","password",Role.guest);
      
    }

    public static void SeedStudents(IStudentService svc)
    {
        // Note: do not call initialise here
        
        var s1 = svc.AddStudent( new Student {
            Name = "Homer Simpson", Course = "Physics", Email = "homer@mail.com", Age = 41, Grade = 56, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/b/bd/Homer_Simpson.png"
        });
        var s2 = svc.AddStudent( new Student {
            Name = "Marge Simpson", Course = "English", Email = "marge@mail.com", Age = 38, Grade = 69, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/4/4d/MargeSimpson.png" 
        });
        var s3 = svc.AddStudent(
            new Student { Name = "Bart Simpson",Course = "Maths", Email = "bart@mail.com", Age = 14, Grade = 61, PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/a/aa/Bart_Simpson_200px.png" 
        });
        var s4 = svc.AddStudent(
            new Student { Name = "Lisa Simpson", Course = "Poetry", Email = "lisa@mail.com", Age = 13, Grade = 80, PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/e/ec/Lisa_Simpson.png" 
        });
        var s5 = svc.AddStudent(
            new Student { Name = "Mr Burns", Course = "Management", Email = "burns@mail.com", Age = 81, Grade = 63, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/a/a7/Montgomery_Burns.png" 
        });
        var s6 = svc.AddStudent(           
            new Student { Name = "Barney Gumble", Course = "Brewing", Email = "barney@mail.com", Age = 39, Grade = 49, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/6/68/Barney_Gumble_-_shading.png" 
        });


        // seed tickets

        // add tickets for homer
        var t1 = svc.CreateTicket(s1.Id, "Cannot login");
        var t2 = svc.CreateTicket(s1.Id, "Printing doesnt work");
        var t3 = svc.CreateTicket(s1.Id, "Forgot my password");

        // add ticket for marge
        var t4 = svc.CreateTicket(s2.Id, "Please reset password");

        // add ticket for bart
        var t5 = svc.CreateTicket(s3.Id, "No internet connection");
        
        // close homers first ticket 
        // TBC - amend due to change to CloseTicket
        svc.CloseTicket(t1.Id, "DemoResolution");
        
    }
}

