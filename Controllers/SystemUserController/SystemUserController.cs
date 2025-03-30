using hrms_api.Dto;
using hrms_api.Filter;
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
        
        [CustomPermissionAuthorize("View SystemUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllSystemUser()
        {
            try
            {
                var systemUser = await _systemuserrepo.GetAllAsync();
                return Ok(systemUser);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        
        [CustomPermissionAuthorize("View SystemUser")]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetSystemUserById(int id)
        {
            try
            {
                var systemuser = await _systemuserrepo.GetByIdAsync(id);
                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "success",
                    status = StatusCodes.Status200OK,
                    data = systemuser
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [CustomPermissionAuthorize("Create SystemUser")]
        [HttpPost]
        public async Task<IActionResult> AddSystemUser (SystemUserCreateDto systemUserCreateDto)
        {

            try
            {   
                await _systemuserrepo.AddAsync(systemUserCreateDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Successfully create system user",
                    status = StatusCodes.Status201Created,
                    data = systemUserCreateDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        [CustomPermissionAuthorize("Update SystemUser")]
        [HttpPut("{id}")]

        public async Task<IActionResult> EditSystemUser(int id ,SystemUserEditDto systemUserEditDto)
        {
            try
            {
                await _systemuserrepo.UpdateAsync(id, systemUserEditDto);
                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "Successfully update system user",
                    status = StatusCodes.Status200OK,
                    data = systemUserEditDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [CustomPermissionAuthorize("Delete SystemUser")]
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

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "Successfully delete system user",
                    status = StatusCodes.Status200OK,

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
