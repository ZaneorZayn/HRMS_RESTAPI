using System.Security.Claims;
using hrms_api.Dto.LeaveRequest;
using hrms_api.Model;
using hrms_api.Repository.LeaveRequestRepository;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.LeaveRequestController;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;

    public LeaveRequestController(ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
    }

    private int GetLoggingUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier!));
    }

    [HttpGet]

    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetAll([FromQuery] LeaveRequestFilter filter)
    {
        try
        {
            var leaveRequest =  await _leaveRequestRepository.GetAllAsync(filter);
            return Ok(leaveRequest);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("request-leave")]

    public async Task<IActionResult> AddLeaveRequest(LeaveRequestDto leaveRequestDto)
    {
        try
        {
            var employeeId = GetLoggingUserId();
         
         if(employeeId == null) return Unauthorized("Invalid employee id");
            
         var leaveRequest = await _leaveRequestRepository.CreateAsync(leaveRequestDto , employeeId);
         return Ok(leaveRequest);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("request_approve/{id}")]
    public async Task<IActionResult> ApproveLeaveRequest(int id)
    {
        try
        {
            var managerId = GetLoggingUserId();
            var approveLeave = await _leaveRequestRepository.ApproveAsync(id, managerId);
            
            if (approveLeave == null)
                return NotFound(new { message = "Leave request not found" });

            return Ok(approveLeave);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("request_reject/{id}")]
    public async Task<IActionResult> RejectLeaveRequest(int id)
    {
        try
        {
         var managerId = GetLoggingUserId();
         var rejectLeave = await _leaveRequestRepository.RejectAsync(id, managerId);
         return Ok(rejectLeave);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("my-leave-requests")]
    public async Task<IActionResult> GetMyLeaveRequests([FromQuery] LeaveRequestFilter filter)
    {
        try
        {
         var employeeId = GetLoggingUserId();
         var allRequest = await _leaveRequestRepository.GetAllByEmployeeId(employeeId , filter);
         return Ok(allRequest);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}