
using SMS.Data.Entities;
	
namespace SMS.Data.Services;

// This interface describes the operations that a StudentService class should implement
public interface IStudentService
{
    // Initialise the repository (database)  
    void Initialise();
    
    // ---------------- Student Management --------------
    List<Student> GetStudents();
    Student GetStudent(int id);
    Student GetStudentByEmail(string email);
    Student AddStudent(Student s);
    Student UpdateStudent(Student updated);  
    bool DeleteStudent(int id);

    // ---------------- Ticket Management ---------------
    Ticket CreateTicket(int studentId, string issue);
    Ticket GetTicket(int id);
    Ticket UpdateTicket(int id, string issue);
    bool   DeleteTicket(int id);

    /* TBC add resolution parameter to CloseTicket*/
    Ticket CloseTicket(int id, string resolved );

    IList<Ticket> GetAllTickets();
    IList<Ticket> GetOpenTickets();        
    IList<Ticket> SearchTickets(TicketRange range, string query);
    
}
    
