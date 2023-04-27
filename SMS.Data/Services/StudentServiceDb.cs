using Microsoft.EntityFrameworkCore;
using SMS.Data.Entities;
using SMS.Data.Repository;

namespace SMS.Data.Services;

public class StudentServiceDb : IStudentService
{
    private readonly DataContext db;

    public StudentServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== Student Management =======================

    // retrieve list of Students
    public List<Student> GetStudents()
    {
        return db.Students.ToList();
    }


    // Retrive student by Id 
    public Student GetStudent(int id)
    {
        return db.Students
                 .Include(s => s.Tickets)                                
                 .FirstOrDefault(s => s.Id == id);
    }

    // Add a new student
    public Student AddStudent(Student s)
    {
        // check if student with email exists            
        var exists = GetStudentByEmail(s.Email);
        if (exists != null)
        {
            return null;
        } 
        // check grade is valid
        if (s.Grade < 0 || s.Grade > 100)
        {
            return null;
        }

        // create new student
        var student = new Student
        {
            Name = s.Name,
            Course = s.Course,
            Email = s.Email,
            Age = s.Age,
            Grade = s.Grade,
            PhotoUrl = s.PhotoUrl
        };
        db.Students.Add(student); // add student to the list
        db.SaveChanges();
        return student; // return newly added student
    }

    // Delete the student identified by Id returning true if 
    // deleted and false if not found
    public bool DeleteStudent(int id)
    {
        var s = GetStudent(id);
        if (s == null)
        {
            return false;
        }
        db.Students.Remove(s);
        db.SaveChanges();
        return true;
    }

    // Update the student with the details in updated 
    public Student UpdateStudent(Student updated)
    {
        // verify the student exists 
        var student = GetStudent(updated.Id);
        if (student == null)
        {
            return null;
        }

        // verify email is still unique
        var exists = GetStudentByEmail(updated.Email);
        if (exists != null && exists.Id != updated.Id)
        {
            return null;
        }

        // verify grade is valid
        if (updated.Grade < 0 || updated.Grade > 100)
        {
            return null;
        }

        // update the details of the student retrieved and save
        student.Name = updated.Name;
        student.Email = updated.Email;
        student.Course = updated.Course;
        student.Age = updated.Age;
        student.Grade = updated.Grade;
        student.PhotoUrl = updated.PhotoUrl;

        db.SaveChanges();
        return student;
    }

    public Student GetStudentByEmail(string email)
    {
        return db.Students.FirstOrDefault(s => s.Email == email);
    }

    // ===================== Ticket Management ==========================

    // create a ticket for identified student with specified issue and return ticket or null if student does not exist
    public Ticket CreateTicket(int studentId, string issue)
        {
            var student = GetStudent(studentId);
            if (student == null) return null;

            var ticket = new Ticket
            {
                // Id created by Database
                Issue = issue,        
                StudentId = studentId,
                // set by default in model but we can override here if required
                CreatedOn = DateTime.Now,
                Active = true,
            };
            db.Tickets.Add(ticket);
            db.SaveChanges(); // write to database
            return ticket;
        }

        // return ticket identified by id  or null if not found 
        public Ticket GetTicket(int id)
        {
            // return ticket and related student or null if not found
            return db.Tickets
                     .Include(t => t.Student)
                     .FirstOrDefault(t => t.Id == id);
        }

        // update ticket issue of identified by id and return updated ticket or null if not found
        public Ticket UpdateTicket(int id, string issue)
        {
            var ticket = GetTicket(id);
            // cannot update a non-existent or inactive ticket
            if (ticket == null || !ticket.Active) return null;

            ticket.Issue = issue;              
                        
            db.Tickets.Update(ticket);
            db.SaveChanges(); // write to database
            return ticket;
        }
        
        // close ticket identified by id and return updated ticket or null if not found or already closed
        public Ticket CloseTicket(int id, string resolution)
        {
            var ticket = GetTicket(id);
            // if ticket does not exist or is already closed return null
            if (ticket == null || !ticket.Active) return null;
            
            // ticket exists and is active so close
            ticket.Active = false;
            
            // TBC - amend due to change to CloseTicket
            ticket.Resolution = resolution;
            ticket.ResolvedOn = DateTime.Now;


            db.SaveChanges(); // write to database
            return ticket;
        }

        // delete ticket identified by id and return true if deleted otherwise return false
        public bool DeleteTicket(int id)
        {
            // find ticket
            var ticket = GetTicket(id);
            if (ticket == null) return false;
            
            // remove ticket 
            var result = db.Tickets.Remove(ticket);
            
            db.SaveChanges();
            return true;
        }

        // Retrieve all tickets and the student associated with the ticket
        public IList<Ticket> GetAllTickets()
        {
            return db.Tickets
                     .Include(t => t.Student)
                     .ToList();
        }

        // Retrieve all open tickets (Active)
        public IList<Ticket> GetOpenTickets()
        {
            // return open tickets with associated students
            return db.Tickets
                     .Include(t => t.Student) 
                     .Where(t => t.Active)
                     .ToList();
        } 

        // perform a search of the tickets based on a query and
        // an active range 'ALL', 'OPEN', 'CLOSED'
        public IList<Ticket> SearchTickets(TicketRange range, string query) 
        {
            // TBC - complete a Linq query to search Ticket resolutions and Student names for the query string
            //       to allow our query to perform partial searches i.e. find a piece of text with Resolution 
            //       or Name we can use Contains and ToLowerCase. For example:
            //       (t.Resolution.ToLowerCase().Contains(query.ToLowerCase()) || t.Student.Name.ToLowerCase().Contains(query.ToLowerCase))
            //       We can then && to this a query to check the range e.g.
            //      (range == TicketRange.OPEN && t.Active || range == TicketRange.CLOSED && !t.Active || range == TicketRange.ALL)          
            query = query == null ? "" : query.ToLower();

            var tickets = db.Tickets 
                            .Include(t => t.Student)
                            .Where( t => t.Issue.ToLower().Contains(query) ||
                              t.Student.Name.ToLower().Contains(query)
                              ) 
                              &&
                              (range == TicketRange.OPEN && t.Active ||
                               range == TicketRange.CLOSED && !t.Active ||
                               range == TicketRange.ALL
                            ).ToList();

            return new List<Ticket>();          
        }

}
