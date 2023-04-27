using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using SMS.Data.Entities;
using SMS.Data.Services;

using SMS.Web.Models;

namespace SMS.Web.Controllers;

[Authorize]
public class TicketController : BaseController
{
    private readonly IStudentService svc;

    public TicketController()
    {
        svc = new StudentServiceDb();
    } 
     
    // GET/POST /ticket/index
    //[AcceptVerbs("GET","POST")]
    public IActionResult Index(TicketSearchViewModel search)
    {       
        // TBC - replace code below with following
        //   perform service query using values in view model 
        //   assign results to search ViewModel Tickets property
        //   replace return View(tickets) with return View(search)          

        var tickets = svc.GetAllTickets();
        return View(tickets);
    }        
            
    // GET/ticket/{id}
    public IActionResult Details(int id)
    {
        var ticket = svc.GetTicket(id);
        if (ticket == null)
        {
            Alert("Ticket Not Found", AlertType.warning);  
            return RedirectToAction(nameof(Index));             
        }

        // DetailsModal view contains close ticket modal version
        // Details view contains hidden form version
        // TBC - change view depending on which version you complete
        return View("DetailsModal",ticket);
    }
    
     //  POST /ticket/close/{id}
    [Authorize(Roles="admin,support")]
    [HttpPost]
    public IActionResult Close(/* TBC - use bind for Id and Resolution */ Ticket t)
    {
        // close ticket via service
        var ticket = svc.CloseTicket(t.Id); // TBC add resolution from the model */          
        if (ticket == null)
        {
            Alert("No such ticket found", AlertType.warning);       
        }
        else
        {
            Alert($"Ticket {t.Id} closed", AlertType.info);  
        }
       
        return RedirectToAction(nameof(Index));
    } 
    
    // GET /ticket/create
    [Authorize(Roles="admin,support")]
    public IActionResult Create()
    {
        var students = svc.GetStudents();
        // populate viewmodel select list property
        var tvm = new TicketCreateViewModel {
            Students = new SelectList(students,"Id","Name") 
        };
        
        // render blank form
        return View( tvm );
    }
    
    // POST /ticket/create
    [HttpPost]
    [Authorize(Roles="admin,support")]
    public IActionResult Create(TicketCreateViewModel tvm)
    {
        if (ModelState.IsValid)
        {
            svc.CreateTicket(tvm.StudentId, tvm.Issue);
    
            Alert($"Ticket Created", AlertType.info);  
            return RedirectToAction(nameof(Index));
        }
        
        // redisplay the form for editing
        return View(tvm);
    }

}

