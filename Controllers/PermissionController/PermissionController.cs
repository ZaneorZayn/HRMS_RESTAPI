using hrms_api.Dto.PermissionDto;
using hrms_api.Model;
using hrms_api.Repository.PermissionRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.PermissionController{

    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Permission>>> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionRepository.GetAllPermissions();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Permission>> GetPermissionById(int id)
        {
            try
            {
             var permission = await _permissionRepository.GetPermissionById(id);
             return StatusCode(StatusCodes.Status200OK, permission);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostPermission(CreatePermissionDto createPermissionDto)
        {
            try
            {
              var createPermission = await _permissionRepository.AddPermission(createPermissionDto);
              return Ok(createPermission);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermission(int id , UpdatePermissionDto updatePermissionDto)
        {
            try
            {
                await _permissionRepository.UpdatePermission(id, updatePermissionDto);
                
                return StatusCode(StatusCodes.Status200OK, new
                {
                    Message = "Permission Updated"
                    
                });

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeletePermission(int id)
        {
            try
            {
                await _permissionRepository.DeletePermission(id);
                return StatusCode(StatusCodes.Status200OK,new
                {
                    Message = "Permission Deleted"
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("assign-permission")]

        public async Task<ActionResult> AssignPermissionToUser(AssignPermissionDto assignPermissionDto)
        {
            try
            {
                await _permissionRepository.AssignRoleToPermission(assignPermissionDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCodes = StatusCodes.Status201Created,
                    message = "Role Assigned To Permission Successfully"

                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}