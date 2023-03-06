using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class UserTask
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    virtual public ICollection<TaskFile>? Files { get; set; }
    virtual public AppUser User { get; set; }

}
