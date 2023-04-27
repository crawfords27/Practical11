
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SMS.Data.Entities;
using SMS.Data.Services;

namespace SMS.Web.Controllers;

[Authorize]
public class StudentController : BaseController
{
    private IStudentService svc;
    
    public StudentController(IStudentService _svc)
    { 
        svc = _svc;
    }

    // GET /student
    public IActionResult Index()
    {
        // load students using service and pass to view
        var data = svc.GetStudents();
        
        return View(data);
    }

    // GET /student/details/{id}
    public IActionResult Details(int id)
    {
        var student = svc.GetStudent(id);
      
        // check if student is null and alert/redirect 
        if (student is null) {
            Alert("Student not found", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: /student/create
    [Authorize(Roles="admin,support")]
    public IActionResult Create()
    {
        // display blank form to create a student
        return View();
    }

    // POST /student/create
    [Authorize(Roles="admin,support")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Create([Bind("Name, Email, Course, Age, Grade, PhotoUrl")] Student s)
    {   
        // validate email is unique
        if (svc.GetStudentByEmail(s.Email) != null)
        {
            ModelState.AddModelError(nameof(s.Email), "The email address is already in use");
        } 

        // complete POST action to add student
        if (ModelState.IsValid)
        {
            // call service AddStudent method using data in s
            var student = svc.AddStudent(s);
            if (student is null) 
            {
                Alert("Issue creating the student", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Details), new { Id = student.Id});   
        }
        
        // redisplay the form for editing as there are validation errors
        return View(s);
    }

    // GET /student/edit/{id}
    [Authorize(Roles="admin,support")]
    public IActionResult Edit(int id)
    {
        // load the student using the service
        var student = svc.GetStudent(id);

        // check if student is null and Alert/Redirect
        if (student is null)
        {
            Alert("Student not found", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }  

        // pass student to view for editing
        return View(student);
    }

    // POST /student/edit/{id}
    [ValidateAntiForgeryToken]
    [Authorize(Roles="admin,support")]
    [HttpPost]
    public IActionResult Edit(int id, Student s)
    {
        // check if email exists and is not owned by student being edited 
        var existing = svc.GetStudentByEmail(s.Email);
        if (existing != null && s.Id != existing.Id) 
        {
           ModelState.AddModelError(nameof(s.Email), "The email address is already in use");
        } 

        // complete POST action to save student changes
        if (ModelState.IsValid)
        {            
            var student = svc.UpdateStudent(s);
            if (student is null) 
            {
                Alert("Issue updating the student", AlertType.warning);
            }

            // redirect back to view the student details
            return RedirectToAction(nameof(Details), new { Id = s.Id });
        }

        // redisplay the form for editing as validation errors
        return View(s);
    }

    // GET / student/delete/{id}
    [Authorize(Roles="admin")]      
    public IActionResult Delete(int id)
    {
        // load the student using the service
        var student = svc.GetStudent(id);
        // check the returned student is not null and if so return NotFound()
        if (student == null)
        {
            Alert("Student not found", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }     
        
        // pass student to view for deletion confirmation
        return View(student);
    }

    // POST /student/delete/{id}
    [HttpPost]
    [Authorize(Roles="admin")]
    [ValidateAntiForgeryToken]   
    public IActionResult DeleteConfirm(int id)
    {
        // delete student via service
        var deleted = svc.DeleteStudent(id);
        if (deleted)
        {
            Alert("Student deleted", AlertType.success);            
        }
        else
        {
            Alert("Student could not  be deleted", AlertType.warning);           
        }
        
        // redirect to the index view
        return RedirectToAction(nameof(Index));
    }

     // ============== Student ticket management ==============

    // GET /student/ticketcreate/{id}
    public IActionResult TicketCreate(int id)
    {
        var student = svc.GetStudent(id);
        if (student == null)
        {
            Alert("Student not found", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // create a ticket view model and set foreign key
        var ticket = new Ticket { StudentId = id }; 
        // render blank form
        return View( ticket );
    }

    // POST /student/ticketcreate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TicketCreate([Bind("StudentId, Issue")] Ticket t)
    {
        if (ModelState.IsValid)
        {                
            var ticket = svc.CreateTicket(t.StudentId, t.Issue);
            Alert($"Ticket created successfully for student {t.StudentId}", AlertType.info);            
            // redirect to display student - note how Id is passed
            return RedirectToAction(
                nameof(Details), new { Id = ticket.StudentId }
            );
        }
        // redisplay the form for editing
        return View(t);
    }

     // GET /student/ticketedit/{id}
    public IActionResult TicketEdit(int id)
    {
        var ticket = svc.GetTicket(id);
        if (ticket == null)
        {
            Alert($"Ticket {id} not found", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }        
        return View( ticket );
    }

    // POST /student/ticketedit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TicketEdit(int id, [Bind("StudentId, Issue")] Ticket t)
    {
        if (ModelState.IsValid)
        {                
            var ticket = svc.UpdateTicket(id, t.Issue);
            return RedirectToAction(
                nameof(Details), new { Id = ticket.StudentId }
            );
        }
        // redisplay the form for editing
        return View(t);
    }

    // GET /student/ticketdelete/{id}
    public IActionResult TicketDelete(int id)
    {
        // load the ticket using the service
        var ticket = svc.GetTicket(id);
        // check the returned Ticket is not null and if so return NotFound()
        if (ticket == null)
        {
            Alert("Ticket not found", AlertType.warning);
            return RedirectToAction(nameof(Index));;
        }     
        
        // pass ticket to view for deletion confirmation
        return View(ticket);
    }

    // POST /student/ticketdeleteconfirm/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TicketDeleteConfirm(int id, int studentId)
    {
        // delete ticket via service
        var deleted = svc.DeleteTicket(id);
        if (deleted)
        {
            Alert("Ticket deleted", AlertType.success);            
        }
        else
        {
            Alert("Ticket could not  be deleted", AlertType.warning);           
        }

        // redirect to the student details view
        return RedirectToAction(nameof(Details), new { Id = studentId });
    }

}