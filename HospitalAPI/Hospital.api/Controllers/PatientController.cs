using Hospital.api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace Hospital.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PatientController(HospitalDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        public IActionResult GetPatients(DateTime? fromDate, DateTime? toDate, string? campCode)
        {
            try
            {
                var query = _context.Patients.AsQueryable();

                // Date filter
                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(x =>
                        x.DateOfSurgery.HasValue &&
                        x.DateOfSurgery.Value.Date >= fromDate.Value.Date &&
                        x.DateOfSurgery.Value.Date <= toDate.Value.Date);
                }

                // CampCode filter
                if (!string.IsNullOrEmpty(campCode))
                {
                    query = query.Where(x => x.Campcode == campCode);
                }

                var result = query.Select(p => new
                {
                    p.Id,
                    p.Campcode,
                    p.Name,
                    p.Gender,
                    p.Age,
                    p.Village,
                    p.Upazila,
                    p.District,
                    p.Country,
                    p.ContactNo,
                    p.NID,
                    p.MedicalRecordNumber,
                    p.PreSurgeryVisualAcuityLeft,
                    p.PreSurgeryVisualAcuityRight,
                    p.DateOfSurgery,
                    p.TypeOfSurgery,
                    p.EyeOperated,
                    p.PostSurgeryVisualAcuity,

                    BeneficiaryPhoto = p.BeneficiaryPhoto != null
                        ? Convert.ToBase64String(p.BeneficiaryPhoto)
                        : null
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= Global search =================
        [HttpGet("globalsearch")]
        public IActionResult GetPatientsDetails(string searchText)
        {
            try
            {
                var query = _context.Patients.AsQueryable();


                // Global filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x => x.MedicalRecordNumber.Contains(searchText) || x.ContactNo.Contains(searchText) || x.Name.Contains(searchText));
                }

                var result = query.Select(p => new
                {
                    p.Id,
                    p.Campcode,
                    p.Name,
                    p.Gender,
                    p.Age,
                    p.Village,
                    p.Upazila,
                    p.District,
                    p.Country,
                    p.ContactNo,
                    p.NID,
                    p.MedicalRecordNumber,
                    p.PreSurgeryVisualAcuityLeft,
                    p.PreSurgeryVisualAcuityRight,
                    p.DateOfSurgery,
                    p.TypeOfSurgery,
                    p.EyeOperated,
                    p.PostSurgeryVisualAcuity,

                    BeneficiaryPhoto = p.BeneficiaryPhoto != null
                        ? Convert.ToBase64String(p.BeneficiaryPhoto)
                        : null
                }).Take(10).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            var p = _context.Patients.FirstOrDefault(x => x.Id == id);

            if (p == null)
                return NotFound("Patient not found");

            return Ok(new
            {
                p.Id,
                p.Campcode,
                p.Name,
                p.Gender,
                p.Age,
                p.Village,
                p.Upazila,
                p.District,
                p.Country,
                p.ContactNo,
                p.NID,
                p.MedicalRecordNumber,
                p.PreSurgeryVisualAcuityLeft,
                p.PreSurgeryVisualAcuityRight,
                p.DateOfSurgery,
                p.TypeOfSurgery,
                p.EyeOperated,
                p.PostSurgeryVisualAcuity,

                BeneficiaryPhoto = p.BeneficiaryPhoto != null
                    ? Convert.ToBase64String(p.BeneficiaryPhoto)
                    : null
            });
        }

        // ================= CREATE =================
        [HttpPost]
        public IActionResult AddPatient([FromBody] PatientDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var patient = new Patient
            {
                Campcode = dto.Campcode,
                Name = dto.Name,
                Gender = dto.Gender,
                Age = dto.Age,
                Village = dto.Village,
                Upazila = dto.Upazila,
                District = dto.District,
                Country = dto.Country,
                ContactNo = dto.ContactNo,
                NID = dto.NID,
                MedicalRecordNumber = dto.MedicalRecordNumber,
                PreSurgeryVisualAcuityLeft = dto.PreSurgeryVisualAcuityLeft,
                PreSurgeryVisualAcuityRight = dto.PreSurgeryVisualAcuityRight,
                DateOfSurgery = dto.DateOfSurgery ?? DateTime.Now,
                TypeOfSurgery = dto.TypeOfSurgery,
                EyeOperated = dto.EyeOperated,
                PostSurgeryVisualAcuity = dto.PostSurgeryVisualAcuity,

                BeneficiaryPhoto = string.IsNullOrEmpty(dto.BeneficiaryPhoto)
                    ? null
                    : Convert.FromBase64String(dto.BeneficiaryPhoto)
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();

            return Ok(patient);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public IActionResult UpdatePatient(int id, [FromBody] PatientDto dto)
        {
            var existing = _context.Patients.FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return NotFound("Patient not found");

            existing.Campcode = dto.Campcode;
            existing.Name = dto.Name;
            existing.Gender = dto.Gender;
            existing.Age = dto.Age;
            existing.Village = dto.Village;
            existing.Upazila = dto.Upazila;
            existing.District = dto.District;
            existing.Country = dto.Country;
            existing.ContactNo = dto.ContactNo;
            existing.NID = dto.NID;
            existing.MedicalRecordNumber = dto.MedicalRecordNumber;
            existing.PreSurgeryVisualAcuityLeft = dto.PreSurgeryVisualAcuityLeft;
            existing.PreSurgeryVisualAcuityRight = dto.PreSurgeryVisualAcuityRight;
            existing.DateOfSurgery = dto.DateOfSurgery;
            existing.TypeOfSurgery = dto.TypeOfSurgery;
            existing.EyeOperated = dto.EyeOperated;
            existing.PostSurgeryVisualAcuity = dto.PostSurgeryVisualAcuity;

            if (!string.IsNullOrWhiteSpace(dto.BeneficiaryPhoto))
            {
                existing.BeneficiaryPhoto = Convert.FromBase64String(dto.BeneficiaryPhoto);
            }

            _context.SaveChanges();

            return Ok(existing);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            var patient = _context.Patients.FirstOrDefault(x => x.Id == id);

            if (patient == null)
                return NotFound("Patient not found");

            _context.Patients.Remove(patient);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}