using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Capstone6.Data;
using Capstone6.Models;
using Microsoft.Extensions.Logging;

namespace Capstone6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsAPIController : ControllerBase
    {
        private readonly ILogger<DepartmentsAPIController> _logger;
        private readonly ApplicationDbContext _context;

        public DepartmentsAPIController(ApplicationDbContext context, ILogger<DepartmentsAPIController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/DepartmentsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            //return await _context.Departments.ToListAsync();
            //return await _context.Departments
            //    .Include("FK_Subjects_Departments_DepartmentID")
            //    .ToListAsync();

            _logger.LogInformation("Extracting List of Departments");

           
                var departments = await _context.Departments
                                 .Include(dept => dept.Subjects)
                                 .ToListAsync();

                return departments;
           
        }

        // GET: api/DepartmentsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/DepartmentsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentID)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            
            return Ok();
        }

        // POST: api/DepartmentsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            if(department.DepartmentName == null)
            {
                return BadRequest();
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentID }, department);
        }

        // DELETE: api/DepartmentsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
