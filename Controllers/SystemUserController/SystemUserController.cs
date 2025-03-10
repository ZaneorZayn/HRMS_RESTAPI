using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.SystemUserRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.SystemUserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUserRepository _systemuserrepo;

        public SystemUserController(ISystemUserRepository systemuserrepo)
        {
            _systemuserrepo = systemuserrepo;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllSystemUser()
        {
            try
            {
                var systemuser = await _systemuserrepo.GetAllAsync();
                return Ok(systemuser);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetSystemUserById(int id)
        {
            try
            {
                var systemuser = await _systemuserrepo.GetByIdAsync(id);
                return Ok(
                    new {
                        message = "success",
                        StatusCode = 201,
                        Data= systemuser
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]

        public async Task<IActionResult> AddSystemUser (SystemUserDto systemUserDto)
        {

            try
            {   
                await _systemuserrepo.AddAsync(systemUserDto);
                return Ok(new {message = "SystemUser add successfully", StatusCode =200 ,Data = systemUserDto});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> EditSystemUser(int id ,SystemUserDto systemUserDto)
        {
            try
            {
                await _systemuserrepo.UpdateAsync(id, systemUserDto);
                return Ok(new { message = "SystemUser was updated successfully", Data = systemUserDto });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteSystemUser(int id)
        {
            try
            {
                var systemuser = await _systemuserrepo.GetByIdAsync(id);
                if(systemuser == null)
                {
                    return NotFound();
                }

                await _systemuserrepo.DeleteAsync(id);
                
                return Ok(new { message = "SystemUser was deleted successfully", Data = systemuser });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
