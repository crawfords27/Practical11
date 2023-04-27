
using Xunit;
using SMS.Data.Entities;
using SMS.Data.Services;

namespace SMS.Test;

[Collection("Sequential")]
public class StudentServiceTests
{
    private readonly IStudentService svc;

    public StudentServiceTests()
    {
        // general arrangement
        svc = new StudentServiceDb();

        // ensure data source is empty before each test
        svc.Initialise();
    }
    
    // =========================== GET ALL STUDENT TESTS =================================

    [Fact] 
    public void GetAllStudents_WhenNoneExist_ShouldReturn0()
    {
        // act 
        var students = svc.GetStudents();
        var count = students.Count;

        // assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void GetStudents_With2Added_ShouldReturnCount2()
    {
        // arrange       
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Engineering", Email="yyy@email.com", Age=23, Grade=0 }
        );       

        // act
        var students = svc.GetStudents();
        var count = students.Count;

        // assert
        Assert.Equal(2, count);
    }

    // =========================== GET SINGLE STUDENT TESTS =================================

    [Fact] 
    public void GetStudent_WhenNoneExist_ShouldReturnNull()
    {
        // act 
        var student = svc.GetStudent(1); // non existent student

        // assert
        Assert.Null(student);
    }

    [Fact] 
    public void GetStudent_WhenAdded_ShouldReturnStudent()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );

        // act
        var ns = svc.GetStudent(s.Id);

        // assert
        Assert.NotNull(ns);
        Assert.Equal(s.Id, ns.Id);
    }

    [Fact] 
    public void GetStudent_WithTickets_RetrievesStudentAndTickets()
    {
        // arrange
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        svc.CreateTicket(s.Id, "Issue 1");  

        // act      
        var ns = svc.GetStudent(s.Id);
        
        // assert
        Assert.NotNull(ns);
        Assert.Equal(1, ns.Tickets.Count);
  } 


    [Fact] 
    public void GetStudentByEmail_WhenAdded_ShouldReturnStudent()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );

        // act
        var ns = svc.GetStudentByEmail("xxx@email.com");

        // assert
        Assert.NotNull(ns);
        Assert.Equal(s.Email, ns.Email);
    }

    // =========================== ADD STUDENT TESTS =================================

    [Fact]
    public void AddStudent_WhenValid_ShouldAddStudent()
    {
        // arrange - add new student
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        
        // act - try to retrieve the newly added student
        var s = svc.GetStudent(added.Id);

        // assert - that student is not null
        Assert.NotNull(s);
        
        // now assert that the properties were set properly
        Assert.Equal(s.Id, s.Id);
        Assert.Equal("XXX", s.Name);
        Assert.Equal("xxx@email.com", s.Email);
        Assert.Equal("Computing", s.Course);
        Assert.Equal(20, s.Age);
        Assert.Equal(0, s.Grade);
    }

    [Fact] // --- AddStudent Duplicate Test
    public void AddStudent_WhenDuplicateEmail_ShouldReturnNull()
    {
        // arrange
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=50 }
        );

        // act 
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Maths", Email="xxx@email.com", Age=30, Grade=40 }
        );
        
        // assert
        Assert.NotNull(s1);
        Assert.Null(s2);       
    }

    [Fact] // --- AddStudent Invalid Grade Test
    public void AddStudent_WhenInvalidGrade_ShouldReturnNull()
    {
        // arrange

        // act 
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=120 }
        );
        
        // assert
        Assert.Null(added);
    }

    // =========================== UPDATE STUDENT TESTS =================================

    [Fact]
    public void UpdateStudent_ThatExists_ShouldSetAllProperties()
    {
        // arrange - create test student        
        var s = svc.AddStudent(
            new Student { Name="ZZZ", Course="Computing", Email="zzz@email.com", Age=30, Grade=100 }
        );
                              
        // act - ** create a copy and update all student properties (except Id) **
        var u = svc.UpdateStudent(           
            new Student {
                Id = s.Id, // use original Id
                Name = "XXX",
                Email = "xxx@email.com",
                Course = "Engineering",
                Age = 31,
                Grade = 50
            }
        ); 

        // reload updated student from database into new student object (us)
        var us = svc.GetStudent(s.Id);

        // assert
        Assert.NotNull(us);           

        // now assert that the properties were set properly           
        Assert.Equal(u.Name, us.Name);
        Assert.Equal(u.Email, us.Email);
        Assert.Equal(u.Course, us.Course);
        Assert.Equal(u.Age, us.Age);
        Assert.Equal(u.Grade, us.Grade);
    }

    [Fact] // --- UpdateStudent Invalid Grade Test
    public void UpdateStudent_WhenInvalidGrade_ShouldReturnNull()
    {
        // arrange
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=70 }
        );

        // act - update Grade with invalid value
        var updated = svc.UpdateStudent(
            new Student { Id = added.Id, Grade=170, Name=added.Name, Course=added.Course, Email="xxx@email.com", Age=added.Age }    
        );
        
        // assert
        Assert.NotNull(added);
        Assert.Null(updated);
    }

    [Fact]
    public void UpdateStudent_WhenDuplicateEmail_ShouldReturnNull()
    {
        // arrange
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=70 }
        );
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Maths", Email="yyy@email.com", Age=30, Grade=50 }
        );

        // act - update s2 Email with duplicate value from s1
        var updated = svc.UpdateStudent(
            new Student { Email = s1.Email, Id = s2.Id, Name=s2.Name, Course=s2.Course, Age=s2.Age, Grade=s2.Grade }    
        );
        
        // assert
        Assert.NotNull(s1);
        Assert.NotNull(s2);
        Assert.Null(updated);
    }

    // ===========================  DELETE STUDENT TESTS =================================
    [Fact]
    public void DeleteStudent_ThatExists_ShouldReturnTrue()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
       
        // act
        var deleted = svc.DeleteStudent(s.Id);

        // try to retrieve deleted student
        var s1 = svc.GetStudent(s.Id);

        // assert
        Assert.True(deleted); // delete student should return true
        Assert.Null(s1);      // s1 should be null
    }


    [Fact]
    public void DeleteStudent_ThatDoesntExist_ShouldReturnFalse()
    {
        // act 	
        var deleted = svc.DeleteStudent(0);

        // assert
        Assert.False(deleted);
    }  


    // ---------------------- Ticket Tests ------------------------
        
    [Fact] 
    public void Ticket_CreateTicket_ForExistingStudent_ShouldBeCreated()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );

        // act
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        
        // assert
        Assert.NotNull(t);
        Assert.Equal(s.Id, t.StudentId);
        Assert.True(t.Active); 
    }

    [Fact] 
    public void Ticket_UpdateTicket_WhenActive_ShouldUpdateIssue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        
        
        // act
        svc.UpdateTicket(t.Id, "DUMMY TICKET");
        var u = svc.GetTicket(t.Id);

        // assert
        Assert.NotNull(u);
        Assert.Equal(u.StudentId, s.Id);
        Assert.Equal("DUMMY TICKET", u.Issue);
        Assert.True(u.Active); 
    }

    // TBC - amend due to change to CloseTicket
    [Fact] 
    public void Ticket_UpdateTicket_WhenInactive_ShouldNotUpdateIssue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        svc.CloseTicket(t.Id);
       
        // act
        var r = svc.UpdateTicket(t.Id, "DUMMY TICKET");
        t = svc.GetTicket(t.Id);

        // assert
        Assert.Null(r);                 // Update result should be null
        Assert.False(t.Active);         // ticket should be inactive
        Assert.Equal("Dummy Ticket 1", t.Issue);        
    }

    [Fact] // --- GetTicket should include Student
    public void Ticket_GetTicket_WhenExists_ShouldReturnTicketAndStudent()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");

        // act
        var ticket = svc.GetTicket(t.Id);

        // assert
        Assert.NotNull(ticket);
        Assert.NotNull(ticket.Student);
        Assert.Equal(s.Name, ticket.Student.Name); 
    }

    [Fact] // --- GetOpenTickets When two added should return two 
    public void Ticket_GetOpenTickets_WhenTwoAdded_ShouldReturnTwo()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t1 = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        var t2 = svc.CreateTicket(s.Id, "Dummy Ticket 2");

        // act
        var open = svc.GetOpenTickets();

        // assert
        Assert.Equal(2,open.Count);                        
    }

    // TBC - amend due to change to CloseTicket
    [Fact] 
    public void Ticket_CloseTicket_WhenOpen_ShouldReturnTicket()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var r = svc.CloseTicket(t.Id, "Default");

        // assert
        Assert.NotNull(r);              // verify closed ticket returned          
        Assert.False(r.Active);
        Assert.Equal("Default", r.Resolution);
        Assert.NotEqual(DateTime.MinValue, r.ResolvedOn);
    }

    // TBC - amend due to change to CloseTicket
    [Fact] 
    public void Ticket_CloseTicket_WhenAlreadyClosed_ShouldReturnNull()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var closed = svc.CloseTicket(t.Id, "Default");     // close active ticket    
        closed = svc.CloseTicket(t.Id);         // close non active ticket

        // assert
        Assert.Null(closed); // no ticket returned as already closed
        Assert.Equal("Default", closed.Resolution);
        Assert.Equal(DateTime.MinValue, closed.ResolvedOn)
    }

    [Fact] 
    public void Ticket_DeleteTicket_WhenExists_ShouldReturnTrue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var deleted = svc.DeleteTicket(t.Id);     // delete ticket    
        
        // assert
        Assert.True(deleted);                    // ticket should be deleted
    }   

    [Fact] 
    public void Ticket_DeleteTicket_WhenNonExistant_ShouldReturnFalse()
    {
        // arrange
        
        // act
        var deleted = svc.DeleteTicket(1);     // delete non-existent ticket    
        
        // assert
        Assert.False(deleted);                  // ticket should not be deleted
    }  


    [Fact] 
    public void Ticket_Search_WhenShouldFindTwoResults_DoesFindTwo()
    {

         // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Age=20, Grade=0 }
        );
        var t1 = svc.CreateTicket(s.Id, "Dummy Ticket");
        var t2 = svc.CreateTicket(s.Id, "Test Ticket");

        // act
        var results = svc.SearchTickets(TicketRange.ALL,"ticket"); 

        // assert
        Assert.Equal(2, results.Count);       

    }


}
