using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.RoleRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.RoleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _rolerepo;

        public RoleController(IRoleRepository rolerepo)
        {
            _rolerepo = rolerepo;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllRole()
        {

            try
            {
                var role = await _rolerepo.GetAllAsync();

                return Ok(role);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _rolerepo.GetByIdAsync(id);
                return Ok(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]

        public async Task<IActionResult> AddRole(RoleDto roledto)
        {
            try
            {
                await _rolerepo.AddAsync(roledto);
                return Ok(new { message = "Role added successfully", Data = roledto });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        
        public async Task<IActionResult> updateRole(int id, RoleDto roleDto)
        {
            try
            {
                await _rolerepo.UpdateAsync(id, roleDto);
                return Ok(new { message = "Role updated successfully", Data = roleDto });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]

        public async Task<IActionResult> deleteRole(int id)
        {
            try
            {
                var role = await _rolerepo.GetByIdAsync(id);  
                if (role == null)
                {
                    return NotFound("Id not found ");
                }

                await _rolerepo.DeleteAsync(id);

                return Ok(new { message = "Role deleted successfully", Data =role  });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}
