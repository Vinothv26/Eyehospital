using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.api.Models
{
    [Table("patients")]
    public class Patient
    {

        public string Campcode { get; set; }

        [Key]
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

      
        public string Gender { get; set; }

        public int? Age { get; set; }

        [StringLength(300)]
        public string Village { get; set; }
        public string Upazila { get; set; }
        public string District { get; set; }
        public string Country { get; set; }


        [StringLength(20)]
        public string ContactNo { get; set; }

        public String NID {  get; set; }

        [StringLength(50)]
        public string MedicalRecordNumber { get; set; }

        [StringLength(50)]
        public string PreSurgeryVisualAcuityLeft { get; set; }

        public string PreSurgeryVisualAcuityRight { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfSurgery { get; set; }

        [StringLength(100)]
        public string TypeOfSurgery { get; set; }

       
        public string EyeOperated { get; set; }

        [StringLength(50)]
        public string PostSurgeryVisualAcuity { get; set; }

        public byte[]? BeneficiaryPhoto { get; set; }
    }

    public class PatientDto
    {
        public string Campcode {  get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public string Village { get; set; }

        public string Upazila { get; set; }
        public string District { get; set; }
        public string Country { get; set; }

        public string ContactNo { get; set; }
        public string NID { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string PreSurgeryVisualAcuityLeft { get; set; }
        public string PreSurgeryVisualAcuityRight { get; set; }

        public DateTime? DateOfSurgery { get; set; }
        public string TypeOfSurgery { get; set; }
        public string EyeOperated { get; set; }
        public string PostSurgeryVisualAcuity { get; set; }

        // BASE64 from Angular
        public string? BeneficiaryPhoto { get; set; }
    }
}
