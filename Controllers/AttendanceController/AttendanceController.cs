using System.Security.Claims;
using hrms_api.Dto.AttendanceDto;
using hrms_api.Filter;
using hrms_api.Model;
using hrms_api.Repository.AttendanceRepository;
using hrms_api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Controllers.AttendanceController
{
    [Route("api/[controller]")]
    [ApiController]

    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly QrCodeService _qrCodeService;

        public AttendanceController(IAttendanceRepository attendanceRepo , QrCodeService qrCodeService)
        {
            _attendanceRepo = attendanceRepo;
            _qrCodeService = qrCodeService;
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
                return BadRequest(new {message = e.Message});
            }
            
        }

        [HttpPost("check-out")]
        public async Task<ActionResult> CheckOut()
        {
            try
            {
                var systemUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _attendanceRepo.MarkCheckOutAsync(systemUserId);
                return Ok(new {Message = "Successfully marked check-out"});
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
        
        [HttpGet("generate")]
        public IActionResult Generate([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required. Format: lat,lng");

            var parts = location.Split(',');
            if (parts.Length != 2) 
                return BadRequest("Invalid format. Use: lat,lng");

            if (!double.TryParse(parts[0], out double lat) ||
                !double.TryParse(parts[1], out double lng))
            {
                return BadRequest("Invalid latitude or longitude values.");
            }

            var qrImage = _qrCodeService.GenerateQrCode(lat, lng);
            return File(qrImage, "image/png", "qrcode.png");
        }

        [HttpPost("scan-attendance")]

        public async Task<IActionResult> ScanAttendance([FromBody] AttendanceScanDto attendanceScanDto)
        {
            try
            {
              var systemUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
              var attendances = await _attendanceRepo.ScanAttendanceAsync( attendanceScanDto, systemUserId);
              return Ok(attendances);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "have some error"});
            }
        }

        [HttpGet("my-attendance")]

        public async Task<IActionResult> GetMyAttendance()
        {
            try
            {
              var systemUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
              var myAttendances = await _attendanceRepo.GetAttendanceUser( systemUserId);
              return Ok(myAttendances);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error retrieving attendance: {e.Message}");
            }
        }
        
        

    }
}