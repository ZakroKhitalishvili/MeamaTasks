using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class TaskFile
{
    public string Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    virtual public UserTask Task { get; set; }

}
