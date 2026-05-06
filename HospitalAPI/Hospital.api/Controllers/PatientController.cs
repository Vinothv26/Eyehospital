using Hospital.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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

        // ================= IMAGE COMPRESSION =================
        private byte[] CompressImage(byte[] imageBytes)
        {
            using var image = Image.Load(imageBytes);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(800, 800),
                Mode = ResizeMode.Max
            }));

            using var ms = new MemoryStream();
            image.Save(ms, new JpegEncoder { Quality = 50 });

            return ms.ToArray();
        }

        // ================= GET ALL =================
        [HttpGet]
        public IActionResult GetPatients(DateTime? fromDate, DateTime? toDate, string? campCode)
        {
            try
            {
                var query = _context.Patients.AsQueryable();

                // ✅ FIXED DATE FILTER
                if (fromDate.HasValue && toDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    var to = toDate.Value.Date.AddDays(1);

                    query = query.Where(x =>
                        x.DateOfSurgery != null &&
                        x.DateOfSurgery >= from &&
                        x.DateOfSurgery < to);
                }

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

                    // ✅ USE COMPRESSED IMAGE
                    BeneficiaryPhoto = p.BeneficiaryPhotoCompressed != null
                        ? Convert.ToBase64String(p.BeneficiaryPhotoCompressed)
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

        // ================= GLOBAL SEARCH =================
        [HttpGet("globalsearch")]
        public IActionResult GetPatientsDetails(string searchText)
        {
            try
            {
                var query = _context.Patients.AsQueryable();

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        x.MedicalRecordNumber.Contains(searchText) ||
                        x.ContactNo.Contains(searchText) ||
                        x.Name.Contains(searchText));
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

                    // ✅ COMPRESSED IMAGE
                    BeneficiaryPhoto = p.BeneficiaryPhotoCompressed != null
                        ? Convert.ToBase64String(p.BeneficiaryPhotoCompressed)
                        : null
                })
                .Take(10)
                .ToList();

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

                // ✅ COMPRESSED IMAGE
                BeneficiaryPhoto = p.BeneficiaryPhotoCompressed != null
                    ? Convert.ToBase64String(p.BeneficiaryPhotoCompressed)
                    : null
            });
        }

        
        // ================= CREATE =================
        [HttpPost]
        public IActionResult AddPatient([FromBody] PatientDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            byte[]? original = null;
            byte[]? compressed = null;

            if (!string.IsNullOrEmpty(dto.BeneficiaryPhoto))
            {
                original = Convert.FromBase64String(dto.BeneficiaryPhoto);
                compressed = CompressImage(original);
            }

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

                BeneficiaryPhoto = original,
                BeneficiaryPhotoCompressed = compressed
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
                var original = Convert.FromBase64String(dto.BeneficiaryPhoto);
                var compressed = CompressImage(original);

                existing.BeneficiaryPhoto = original;
                existing.BeneficiaryPhotoCompressed = compressed;
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
