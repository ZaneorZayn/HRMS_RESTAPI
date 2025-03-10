using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.EmployeeRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hrms_api.Controllers.EmployeeController
{
    [Route("api/[controller]")]
    [Authorize] //User must be authenticated
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeRepository _employeerepo;

        public EmployeeController(IEmployeeRepository employeerepo)
        {
            _employeerepo = employeerepo;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            try
            {
                var employee = await _employeerepo.GetAllAsync();
                return Ok(employee);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeerepo.GetByIdAsync(id);
                return Ok(employee);
            }
            catch (Exception ex) {

                return BadRequest(ex.Message);
            }
               


        }


        
        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeDto employee)
        {
            try
            {
                await _employeerepo.AddAsync(employee); 
                return Ok(new { message = "Employee added successfully", Data = employee });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {   
                await _employeerepo.DeleteAsync(id);
                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee(int id, EmployeeDto employeedto)
        {
            try
            {
                await _employeerepo.UpdateAsync(id, employeedto);
                return Ok(new { message = "Employee was updated successfully", Data = employeedto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpPost("{employeeId}/link-systemuser/{systemUserId}")]
        public async Task<IActionResult> LinkSystemUser(int employeeId, int systemUserId)
        {
            try
            {
                await _employeerepo.LinkSystemUserAsync(employeeId, systemUserId);
                return Ok(new { message = "SystemUser linked to Employee successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        [HttpPost("{employeeId}/unlink-systemuser")]

        public async Task<IActionResult> UnlinkSystemUser(int employeeId)
        {
            try
            {
                await _employeerepo.UnlinkSystemUserAsync(employeeId);
                return Ok(new { message = "SystemUser unlinked from Employee successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
