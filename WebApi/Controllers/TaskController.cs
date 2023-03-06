using Infrastructure.Database;
using Infrastructure.Entities;
using Meama_Tasks.Attributes;
using Meama_Tasks.Extensions;
using Meama_Tasks.Models;
using Meama_Tasks.Models.Task;
using Meama_Tasks.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Meama_Tasks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {

        private readonly ILogger<TaskController> _logger;
        private readonly TaskDbContext _dbContext;

        public TaskController(ILogger<TaskController> logger, TaskDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [AuthorizePermission(AuthorizePermissions = Permission.ViewTasks)]
        public async Task<ActionResult<IList<TaskVM>>> GetTasks()
        {
            var tasks = await _dbContext.Tasks!.ToListAsync();

            return Ok(tasks.Select(MapTask));
        }

        [HttpGet("{id}")]
        [AuthorizePermission(AuthorizePermissions = Permission.ViewTasks)]
        public async Task<ActionResult<IList<TaskVM>>> GetTask(string id)
        {
            var task = await _dbContext.Tasks!.FirstOrDefaultAsync(x => x.Id == id);

            if (task is null)
            {
                return BadRequest("Task not found");
            }

            return Ok(MapTask(task));
        }


        [HttpPost]
        [AuthorizePermission(AuthorizePermissions = Permission.CreateTask)]
        public async Task<ActionResult<TaskVM>> PostTask([FromForm] TaskCreateVM taskCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = HttpContext.GetUserData();

            var task = new UserTask()
            {
                Title = taskCreate.Title,
                Description = taskCreate.Description,
                ShortDescription = taskCreate.ShortDescription,
                User = _dbContext.Users.First(x => x.Id == user.Id)
            };

            if (taskCreate.Files is not null && taskCreate.Files.Count != 0)
            {
                var files = new List<TaskFile>();

                foreach (var formFile in taskCreate.Files)
                {
                    var fileUpload = await UploadFileAsync(formFile);
                    if (fileUpload is not null)
                    {
                        files.Add(
                        new TaskFile
                        {
                            Name = fileUpload.Value.name,
                            Path = fileUpload.Value.path
                        });
                    }
                }

                task.Files = files;
            }


            var result = await _dbContext.Tasks.AddAsync(task);

            return Created("", MapTask(result.Entity));
        }

        [HttpPut]
        [AuthorizePermission(AuthorizePermissions = Permission.UpdateTask)]
        public async Task<ActionResult> PutTask([FromForm] TaskUpdateVM taskUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskUpdate.Id);

            if (task is null)
            {
                return BadRequest("Task not found");
            }

            task.ShortDescription = taskUpdate.ShortDescription;
            task.Description = taskUpdate.Description;
            task.Title = taskUpdate.Title;

            //Adding new files
            if (taskUpdate.Files != null && taskUpdate.Files.Count != 0)
            {
                var files = new List<TaskFile>();

                foreach (var formFile in taskUpdate.Files)
                {
                    var fileUpload = await UploadFileAsync(formFile);
                    if (fileUpload is not null)
                    {
                        files.Add(
                        new TaskFile
                        {
                            Name = fileUpload.Value.name,
                            Path = fileUpload.Value.path
                        });
                    }
                }

                task.Files = files;
            }

            // Deleting files
            if (taskUpdate.DeletedFiles != null && taskUpdate.DeletedFiles.Count != 0)
            {
                var deletedTaskFiles = task.Files.Where(x => taskUpdate.DeletedFiles.Contains(x.Id));

                foreach (var deletedTaskFile in deletedTaskFiles)
                {
                    DeleteFile(deletedTaskFile);
                    task.Files.Remove(deletedTaskFile);
                }

            }

            var result = _dbContext.Tasks.Update(task);

            return Ok(MapTask(result.Entity));
        }

        [HttpDelete("{id}")]
        [AuthorizePermission(AuthorizePermissions = Permission.DeleteTask)]
        public async Task<ActionResult> DeleteTask(string id)
        {

            var task = _dbContext.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return BadRequest("Task not found");
            }

            _dbContext.Tasks.Remove(task);

            return Ok();
        }

        private async Task<(string name, string path)?> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var name = file.FileName;
                var onlyName = Path.GetFileNameWithoutExtension(name);
                var ext = Path.GetExtension(name);
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var path = $"Uploads/{year}/{month}/{onlyName}-{Path.GetRandomFileName()}{ext}";

                Directory.CreateDirectory(path);

                using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }

                return (name, path);

            }

            return null;
        }

        private void DeleteFile(TaskFile file)
        {
            var fileInfo = new FileInfo(file.Path);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        private TaskVM MapTask(UserTask userTask)
        {
            return new TaskVM()
            {
                Id = userTask.Id,
                Title = userTask.Title,
                Description = userTask.Description,
                Files = userTask.Files?.Select(x =>
                new TaskFileVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path
                }),
                ShortDescription = userTask.ShortDescription,
                User = new UserVM
                {
                    Email = userTask.User.Email!,
                    Id = userTask.User.Id,
                    Name = userTask.User.UserName!,
                }
            };
        }


    }
}