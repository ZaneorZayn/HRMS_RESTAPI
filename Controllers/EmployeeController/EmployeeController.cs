using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.EmployeeRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hrms_api.Filter;

namespace hrms_api.Controllers.EmployeeController
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeRepository _employeerepo;

        public EmployeeController(IEmployeeRepository employeerepo)
        {
            _employeerepo = employeerepo;
        }

        [CustomAuthorize("Admin","SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            try
            {
                var employee = await _employeerepo.GetAllAsync();
                return StatusCode(StatusCodes.Status200OK, employee);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
       
        [CustomAuthorize("Admin","SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeerepo.GetByIdAsync(id);
                return StatusCode(
                    StatusCodes.Status200OK, 
                    employee
                );
            }
            catch (Exception ex) {

                return BadRequest(ex.Message);
            }
        }
        
        
       [CustomAuthorize("Admin","SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromForm] CreateEmployeeDto createEmployeeDto)
        {
            try
            {   
                await _employeerepo.AddAsync(createEmployeeDto); 
                return StatusCode(StatusCodes.Status201Created, new
                {   
                    status = StatusCodes.Status201Created,
                    message = "Successfully added employee." ,
                    Data = createEmployeeDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       [CustomAuthorize("Admin", "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {   
                await _employeerepo.DeleteAsync(id);
                return StatusCode(StatusCodes.Status200OK, new
                    {
                        status = StatusCodes.Status200OK,
                        message = "Successfully deleted employee.",
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        
        [CustomAuthorize("Admin","SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee(int id, [FromForm] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                await _employeerepo.UpdateAsync(id, updateEmployeeDto);
                return StatusCode(
                    StatusCodes.Status200OK, new
                    {
                        message = "Successfully updated employee.",
                        data = updateEmployeeDto
                    }
                );

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [CustomAuthorize("Admin","SuperAdmin")]
        [HttpPost("{employeeId}/link-systemuser/{systemUserId}")]
        public async Task<IActionResult> LinkSystemUser(int employeeId, int systemUserId)
        {
            try
            {
                await _employeerepo.LinkSystemUserAsync(employeeId, systemUserId);
                return StatusCode(
                    StatusCodes.Status200OK, new
                    {
                        message = "Successfully linked employee.",
                       
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuthorize("Admin","SuperAdmin")]
        [HttpPost("{employeeId}/unlink-systemuser")]
        public async Task<IActionResult> UnlinkSystemUser(int employeeId)
        {
            try
            {
                await _employeerepo.UnlinkSystemUserAsync(employeeId);
                return StatusCode(
                    StatusCodes.Status200OK, new
                    {
                        message = "Successfully unlinked employee.",
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
