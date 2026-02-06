using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone6.Data;
using Capstone6.Models;
using Capstone6.Services;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Capstone6.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        //Name of queue in azure storage
        private string queueName = "capstone6";

        //To share the value in different functions for edit action
        private static string oldDepartmentValue;

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(ApplicationDbContext context, 
                                     IConfiguration config,
                                     UserManager<IdentityUser> userManager,
                                     IEmailSender emailSender,
                                     ILogger<DepartmentsController> logger)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            //should create the queue when the page loads
            CreateQueue(queueName);
            return View(await _context.Departments.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,DepartmentName")] Department department)
        {
            
            if (ModelState.IsValid)
            {
                #region get user details
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("Create", "User not found. Please login again!");
                }
                #endregion
                
                _context.Add(department);
                await _context.SaveChangesAsync();

                #region Insert message to queue
                DateTime currentDT = DateTime.Now;
                string createMessage = $"{user.UserName} has created {department.DepartmentName} Department on {currentDT}";
                InsertMessage(queueName,createMessage);

                #endregion

                // should be done in function
                #region sendEmail
                //try
                //{
                //    string toEmail = "principle@contosouni.com";
                //    string subject = "New System activities";
                     
                //    await _emailSender.SendEmailAsync(toEmail,subject, createMessage);
                //}
                //catch(Capstone6.Services.MyEmailSenderException ex)
                //{
                //    string message = $"Error: {ex.Message}";
                    
                //}
                #endregion
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            oldDepartmentValue = department.DepartmentName;
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,DepartmentName")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    #region get user details
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        ModelState.AddModelError("Edit", "User not found. Please login again!");
                    }
                    #endregion

                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    #region Insert message to queue
                    DateTime currentDT = DateTime.Now;
                    string createMessage = $"{user.UserName} has updated Department ID {department.DepartmentID} " +
                        $"from {oldDepartmentValue} to {department.DepartmentName} on {currentDT}";
                        
                    InsertMessage(queueName, createMessage);

                    #endregion

                    //should be in function
                    #region sendEmail
                    //try
                    //{
                    //    string toEmail = "principle@contosouni.com";
                    //    string subject = "New System activities";

                    //    await _emailSender.SendEmailAsync(toEmail, subject, createMessage);
                    //}
                    //catch (Capstone6.Services.MyEmailSenderException ex)
                    //{
                    //    string message = $"Error: {ex.Message}";
                        
                    //}
                    #endregion
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            #region get user details
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Delete", "User not found. Please login again!");
            }
            #endregion

            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            #region Insert message to queue
            DateTime currentDT = DateTime.Now;
            string createMessage = $"{user.UserName} has deleted Department ID {department.DepartmentID} " +
                $"with Department Name {department.DepartmentName} on {currentDT}";

            InsertMessage(queueName, createMessage);

            #endregion

            //should be in function
            #region sendEmail
            //try
            //{
            //    string toEmail = "principle@contosouni.com";
            //    string subject = "New System activities";

            //    await _emailSender.SendEmailAsync(toEmail, subject, createMessage);
            //}
            //catch (Capstone6.Services.MyEmailSenderException ex)
            //{
            //    string message = $"Error: {ex.Message}";
                
            //}
            #endregion
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }

        #region Azure Queues methods

        //-------------------------------------------------
        // Create a message queue
        //-------------------------------------------------
        public async void CreateQueue(string queueName)
        {
            try
            {
                // Get the connection string from app settings
                string connectionString = _config.GetValue<string>("StorageConnectionString:AzureQueueConnectionString");

                // Instantiate a QueueClient which will be used to create and manipulate the queue
                QueueClient queueClient = new QueueClient(connectionString, queueName);

                // Create the queue
                await queueClient.CreateIfNotExistsAsync();

                
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Exception: {ex.Message}\n\n");
                _logger.LogInformation($"Make sure the Azurite storage emulator running and try again.");
            }
        }


        //-------------------------------------------------
        // Insert a message into a queue
        //-------------------------------------------------
        public void InsertMessage(string queueName, string message)
        {
            // Get the connection string from app settings
            string connectionString = _config.GetValue<string>("StorageConnectionString:AzureQueueConnectionString");

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            if (queueClient.Exists())
            {
                
                // Send a message to the queue
                queueClient.SendMessage(message);
            }

            _logger.LogInformation($"Inserted: {message}");
        }
    #endregion
    }
}
