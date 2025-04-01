using System.Security.Claims;
using hrms_api.Dto.AttendanceDto;
using hrms_api.Model;
using hrms_api.Repository.AttendanceRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.AttendanceController
{
    [Route("api/[controller]")]
    [ApiController]

    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;

        public AttendanceController(IAttendanceRepository attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }

        [HttpPost("check-in")]

        public async Task<ActionResult> CheckIn()
        {

            try
            {
                var systemUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value); 
                await _attendanceRepo.MarkCheckInAsync(systemUserId); 
                return Ok("Successfully marked check-in");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpPost("check-out")]
        public async Task<ActionResult> CheckOut()
        {
            try
            {
                var systemUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _attendanceRepo.MarkCheckOutAsync(systemUserId);
                return Ok("Successfully checked out");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        
        [HttpGet("attendance-today")]
        public async Task<IActionResult> GetAttendancesByToday()
        {
            var attendances = await _attendanceRepo.GetAttendancesByToday();
            return Ok(attendances);

            
        }

    }
}