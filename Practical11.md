# COM741 Web Applications Development

### Practical 11 - Adding Features Described via User Stories

**The process of defining "functional requirements" as user stories and then implementing these requirements, is something you will need to be able to do for your final project dissertation.**

1. Ticket Resolution Feature:  

    **As a user** (admin/support role),
    **I would** like to be able to record Ticket resolution details when closing the ticket,
    **so that** this information can be used as a resource when resolving other tickets.

    Steps to implement feature include:

    - Amend the SMS.Data Ticket model to include ```ResolvedOn``` (DateTime) and ```Resolution``` (string) properties.
    
    - Update the service ``CloseTicket`` interface and implementation to accept the additional ```string resolution``` parameter and ensure the ticket  Resolution and ResolvedOn values are updated.
    
    - Amend CloseTicket tests to prove the operation of the amended ticket service methods
    
    - Amend the Ticket ```Index.cshtml``` view to contain a link to each Tickets "Details" action (removing Close form)
    
    - Complete one of the following:
        - Modal Version: Amend Ticket ```DetailsModal.cshml``` view to display the Ticket ```ResolvedOn``` and ```Resolution``` fields. Then amend the ```_CloseModal.cshtml``` partial to include a ```Resolution``` textarea in the form.
        - Hidden Form Version: Amend Ticket ```Details.cshml``` to contain the resolution textarea in the hidden form. Complete the javascript method to toggle the display of the form and the controls div. Update the Details action return statement to ```return View(ticket);``` to display the alternate view (without the modal).
       - In both cases ensure that the Close button is disabled if the ticket is not Active. We cannot close a ticket that is already closed. 

    - Amend the ```TicketController Close``` action to call the service method to close the ticket, passing in the resolution contained in the model. Also use bind to ensure we only bind the required values Id and Resolution.


2. Ticket Search Feature:

    **As a user** (admin/manager role)
    **I can** search all, open or closed ticket issues or student names using a search query,
    **so that** I can easily find relevant tickets


    Steps to implement feature include:

    - Complete the outline service method ```public IList<Ticket> SearchTickets(TicketRange range, string query) {...}``` Note use of ```TicketRange``` enum found in the ```SMS.Data.Models``` namespace. See the notes for guidance on constructing the query.

    - Verify operation via additional test cases (see example test)

    - Create a ```TicketSearchViewModel``` in ```SMS.Web.Models``` namespace to contain properties ```string Query, TicketRange Range``` and  ```IList<Ticket> Tickets```.
  
    - Create a ```_Search.cshtml``` partial form in Ticket views folder to collect search data (Query, Range)

    - Update the ```Index``` action in the Ticket controller. This should use the Query and Range atttibutes of the viewmodel parameter to call the new SearchTicket service method and assign the results to the view model Tickets property, before returning the View with the viewmodel as a parameter

    - Update the ```Index.cshtml``` view by adding the ```_Search partial```.


3. Use dependency injection system to 

    - add a Scoped instance of IStudentService as StudentServiceDb in Program.cs
    - configure the Student/Ticket and User controllers to use the dependency injection system and verify the application still works


### OPTIONAL
4. Review the documentation below for use of the Remote validator. This provides client side validation to call a remote action on a controller. We could use this to call the service to verify if the email address the user just entered is unique (not already used), as opposed to only checking the email address when the form is submitted to the server. This would provide more immediate feedback to the user when registering.
You would need to do the following as outlined in the Remote validator documentation:
    - Add the ```[Remote]``` attribute to the ```Email``` property in the UserRegisterViewModel.
    - Add a new action to the ```UserController``` which can be called to validate the email.
    - Remove the manual check for a unique email in the Register action

```https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation#remote-attribute```. 

*Note the Remote validator should only be used in ViewModels as its part of the .NET MVC library and we don't want this to be a dependency in our Data project.*