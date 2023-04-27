using System.ComponentModel.DataAnnotations;

namespace SMS.Data.Entities;

// used in ticket search feature
public enum TicketRange { OPEN, CLOSED, ALL }

public class Ticket
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(500, MinimumLength = 5)]
    public string Issue { get; set; }

    // TBC -- add string Resolution with validator for string length of 500
    [StringLength(500, MinimumLength = 500)] 
    public string Resolution { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    // TBC -- add ResolvedOn date time and initialise to DateTime.MinValue
    public DateTime ResolvedOn { get; set; } = DateTime.MinValue;
    
    // ticket owned by a student
    public int StudentId { get; set; }      // foreign key
    public Student Student { get; set; }    // navigation property

} 
