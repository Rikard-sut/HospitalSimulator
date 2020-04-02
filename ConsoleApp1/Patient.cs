using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Methods;
using System.Globalization;
using static Simulator.CodeFirstDB;

namespace Simulator
{
    /// <summary>
    /// Patient ligger här för hade den först inte med i DBcontext.
    /// </summary>
    public class Patient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int patientID { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
        public DateTime BirthDate { get; set; }
        public int SymptomLevel { get; set; }
        public virtual Queue Queue { get; set; }
        public virtual IVA IVA { get; set; }
        public virtual Sanatorium Sanatorium { get; set; }
        public virtual Afterlife Afterlife { get; set; }
        public virtual Healthy Healthy { get; set; }

        public Patient()
        {

        }

        public Patient(string name, string ssn, int symptomLevel)
        {
            Name = name;
            SSN = ssn;
            SymptomLevel = symptomLevel;
            BirthDate = CalculateAge(ssn);
        }
        /// <summary>
        /// Calculates age from the randomized SSN
        /// </summary>
        /// <param name="ssn">the SSN to convert to a birthdate</param>
        /// <returns>Time of birth</returns>
        public DateTime CalculateAge(string ssn)
        {
            string birthDate = ssn.Remove(8, 5);
            DateTime dob = DateTime.ParseExact(birthDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return dob;
        }
    }
}
