using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class CodeFirstDB
    {

        public class Queue
        {
            public int Id { get; set; }
            public virtual List<Patient> PatientsInQueue { get; set; }
            public Queue()
            {
                this.PatientsInQueue = new List<Patient>();
            }
            public override string ToString()
            {
                return "Kö";
            }
            //public int ID { get; set; }
            //public string Name { get; set; }
            //public string SSN { get; set; }
            //public int SymptomLevel { get; set; }
            //public QueuePatient()
            //{

            //}
            //public QueuePatient(Patient patient)
            //{
            //    Name = patient.Name;
            //    SSN = patient.SSN;
            //    SymptomLevel = patient.SymptomLevel;
            //}
        }
        public class IVA
        {
            public int Id { get; set; }
            public virtual List<Patient> Patients { get; set; }
            public IVA()
            {
                this.Patients = new List<Patient>();
            }
            public override string ToString()
            {
                return "Iva";
            }
            //public int ID { get; set; }
            //public string Name { get; set; }
            //public string SSN { get; set; }
            //public int SymptomLevel { get; set; }
            //public IVAPatient()
            //{

            //}
            //public IVAPatient(Patient patient)
            //{
            //    Name = patient.Name;
            //    SSN = patient.SSN;
            //    SymptomLevel = patient.SymptomLevel;
            //}

        }
        public class Sanatorium
        {
            public int Id { get; set; }
            public virtual List<Patient> Patients { get; set; }
            public Sanatorium()
            {
                this.Patients = new List<Patient>();
            }
            public override string ToString()
            {
                return "Sanatorium";
            }
            //public int PatientID { get; set; }
            //public virtual Patient Patient { get; set; }
            //public int ID { get; set; }
            //public string Name { get; set; }
            //public string SSN { get; set; }
            //public int SymptomLevel { get; set; }
            //public SanatoriumPatient()
            //{

            //}
            //public SanatoriumPatient(Patient patient)
            //{
            //    Name = patient.Name;
            //    SSN = patient.SSN;
            //    SymptomLevel = patient.SymptomLevel;
            //}
        }
        public class Afterlife
        {
            public int Id { get; set; }
            public virtual List<Patient> Patients { get; set; }
            public Afterlife()
            {
                this.Patients = new List<Patient>();
            }
            //public int PatientID { get; set; }
            //public virtual Patient Patient { get; set; }
            //public int ID { get; set; }
            //public string Name { get; set; }
            //public string SSN { get; set; }
            //public int SymptomLevel { get; set; }
            //public AfterlifePatient()
            //{

            //}
            //public AfterlifePatient(Patient patient)
            //{
            //    Name = patient.Name;
            //    SSN = patient.SSN;
            //    SymptomLevel = patient.SymptomLevel;
            //}
        }
        public class Healthy
        {
            public int Id { get; set; }
            public virtual List<Patient> Patients { get; set; }
            //public int PatientID { get; set; }
            //public virtual Patient Patient { get; set; }
            //public int ID { get; set; }
            //public string Name { get; set; }
            //public string SSN { get; set; }
            //public int SymptomLevel { get; set; }
            //public HealthyPatient()
            //{

            //}
            //public HealthyPatient(Patient patient)
            //{
            //    Name = patient.Name;
            //    SSN = patient.SSN;
            //    SymptomLevel = patient.SymptomLevel;
            //}
        }


        public class HospitalDB : DbContext
        {
            public HospitalDB() : base(@"data source=LAPTOP-Q58DHVN7;initial catalog=HospitalDB;integrated security=True;")
            {

            }
            public DbSet<Patient> Patients { get; set; }
            public DbSet<Queue> Queue { get; set; }
            public DbSet<IVA> IVA { get; set; }
            public DbSet<Sanatorium> Sanatorium { get; set; }
            public DbSet<Afterlife> AfterLife { get; set; }
            public DbSet<Healthy> Healthy { get; set; }

        }
    }
}
