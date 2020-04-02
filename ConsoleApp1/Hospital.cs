using Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Simulator.CodeFirstDB;

namespace Simulator
{
    class Hospital
    {
        public int DismissedPatients = 0;
        static Queue CurrentQueue = new Queue(); //Instansierar en kö för att ALLTID pilla i rätt instans av den akutella kön för simuleringen.
        static IVA CurrrentIva = new IVA();
        static Sanatorium CurrentSanatorium = new Sanatorium();
        static Afterlife CurrentAfterLife = new Afterlife();
        static Healthy CurrentHealthy = new Healthy();

        public event EventHandler<HospitalEventArgs> PatientMovedEventHandler;
        internal virtual void OnPatientMoved(HospitalEventArgs e)
        {
            PatientMovedEventHandler?.Invoke(this, e);
        }
        public void AddPatients()
        {
            using (var db = new HospitalDB())
            {
                Console.WriteLine("Loading Patients....");

                for (int i = 0; i < 30; i++)
                {
                    Thread.Sleep(150); //För att det inte ska bli dubletter av Patient.
                    var patient = new Patient(Randomers.GenerateName(), Randomers.GenerateSSN(), Randomers.GenerateSymptomLevel());
                    db.Patients.Add(patient);
                    CurrentQueue.PatientsInQueue.Add(patient);
                }

                CurrentQueue.PatientsInQueue = CurrentQueue.PatientsInQueue.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList(); //Sorterar före inläggning i db.
                db.Queue.Add(CurrentQueue);
                db.IVA.Add(CurrrentIva);
                db.Sanatorium.Add(CurrentSanatorium);
                db.AfterLife.Add(CurrentAfterLife);
                db.Healthy.Add(CurrentHealthy);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Select patient to move from queue or sanatorium and removes it from correct table.
        /// </summary>
        public void SelectPatientToTreat()
        {
            //-------------------------------
            //using (var db = new HospitalDB())
            //{
            //    //var patients = CurrentQueue.PatientsInQueue.Select(x => x).Take(5);

            //    //EN I TAGET
            //    var patientQ = (from p in db.Patients
            //                    where p.Queue != null
            //                    orderby p.SymptomLevel descending, p.BirthDate
            //                    select p).FirstOrDefault<Patient>();


            //    var patientS = (from p in db.Patients
            //                    where p.Sanatorium != null
            //                    orderby p.SymptomLevel descending, p.BirthDate
            //                    select p).FirstOrDefault<Patient>();

            //    List<Patient> twoPatients = new List<Patient>();
            //    twoPatients.Add(patientQ);
            //    if (patientS != null)
            //        twoPatients.Add(patientS);

            //    twoPatients.OrderByDescending(x => x.SymptomLevel).OrderBy(x => x.BirthDate);
            //    var patient = twoPatients.First();

            //    var iva = db.IVA.Find(CurrrentIva.Id);
            //    var sanat = db.Sanatorium.Find(CurrentSanatorium.Id); 

            //    var queue = db.Queue.Find(CurrentQueue.Id);
            //    db.Entry(queue).Collection(x => x.PatientsInQueue).Load();

            //    if (patient.Sanatorium!= null)
            //    {
            //        db.Entry(sanat).Collection(x => x.Patients).Load();
            //        AddToIvaOrSanatorium(patient);
            //        sanat.Patients.Remove(patient);
            //        db.SaveChanges();
            //    }
            //    if(patient.Queue != null)
            //    {
            //        AddToIvaOrSanatorium(patient);
            //        queue.PatientsInQueue.Remove(patient);
            //        db.SaveChanges();
            //    }
            //---------------------------------------------   


            //var iva = db.IVA.Find(CurrrentIva.Id);
            //Add all 5 patiens to IVA
            //foreach (Patient patient in CurrentQueue.PatientsInQueue)
            //{
            //    iva.Patients.Add(patient);
            //}
            //db.SaveChanges();
            //Removes all 5 patiens from currentQueue
            //foreach (Patient patient in patients)
            //{
            //    patient.Queue = null;
            //}
            //db.Entry(queue).CurrentValues.SetValues(queue.PatientsInQueue);
            //db.SaveChanges();
            //}
            //Runs every 5 seconds

            //foreach(Patient item in patients)
            //{ 
            //    db.Patients.SqlQuery("UPDATE Patients set Queue_Id = null WHERE Patients.Name = @name", item.Name);
            //    db.SaveChanges();

            //}

            //Queue queue = db.Queue.Include("Patient").Select(q => q).FirstOrDefault();
            //var realqueu = db.Queue.Attach(queue);
            //var queue = db.Queue.Find(CurrentQueue.Id);
            //db.Entry(queue).Collection(x => x.PatientsInQueue).Load();
            //foreach (Patient patient in patients)
            //{

            //    Iva.Patients.Add(patient);
            //    queue.PatientsInQueue.Remove(patient);
            //}
            //db.IVA.Add(Iva);
            //db.SaveChanges();

        }
        public void MoveAroundPatients()
        {
            using (var db = new HospitalDB())
            {

                var queue = db.Queue.Find(CurrentQueue.Id);
                var sanatorium = db.Sanatorium.Find(CurrentSanatorium.Id);
                var iva = db.IVA.Find(CurrrentIva.Id);
                //Load Queue with Patients
                

                if (sanatorium.Patients.Count > 0)
                {
                    db.Entry(sanatorium).Collection(p => p.Patients).Load();
                    sanatorium.Patients = sanatorium.Patients.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList();
                }
                if(queue.PatientsInQueue.Count > 0)
                {
                    db.Entry(queue).Collection(p => p.PatientsInQueue).Load();
                    queue.PatientsInQueue = queue.PatientsInQueue.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList();
                }
                
                db.SaveChanges();
                //Add all 5 patients to IVA and removes all 5 patients from currentQueue/Sanatorium
                while (iva.Patients.Count < 5)
                {
                    int result = FindSickestPatient();
                    Patient patient;
                    //If Sanatorium
                    if (result == 0)
                    {
                        patient = sanatorium.Patients.FirstOrDefault();
                        iva.Patients.Add(patient);
                        //CurrrentIva.Patients.Add(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        sanatorium.Patients.Remove(patient);
                    }
                    //If Queue
                    else
                    {
                        patient = queue.PatientsInQueue.FirstOrDefault();
                        iva.Patients.Add(patient);
                        //CurrrentIva.Patients.Add(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        queue.PatientsInQueue.Remove(patient);
                        //CurrentQueue.PatientsInQueue.Remove(patient);

                    }

                    db.SaveChanges();
                }
                db.SaveChanges();
                //---------------------------------------------------------------------------------
                //Add all 10 patients to Sanatorium and removes all 10 patients from currentQueue
                while (sanatorium.Patients.Count < 10)
                {
                    var patient = queue.PatientsInQueue.FirstOrDefault();
                    sanatorium.Patients.Add(patient);
                    //CurrentSanatorium.Patients.Add(patient);
                    db.SaveChanges();
                    OnPatientMoved(new HospitalEventArgs(patient));
                    queue.PatientsInQueue.Remove(patient);
                    //CurrentQueue.PatientsInQueue.Remove(patient);
                }
                db.SaveChanges();

            }
        }
        private static int FindSickestPatient()
        {
            using (var db = new HospitalDB())
            {
                var sanatorium = db.Sanatorium.Find(CurrentSanatorium.Id);
                var queue = db.Queue.Find(CurrentQueue.Id);
                Patient patient1 = null;
                Patient patient2 = null;
                //If program not runs for the first time
                if (sanatorium.Patients.Count > 0)
                {
                    db.Entry(sanatorium).Collection(p => p.Patients).Load();
                    sanatorium.Patients = sanatorium.Patients.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList();
                    patient1 = sanatorium.Patients[0];
                }
                if(queue.PatientsInQueue.Count > 0)
                {
                    db.Entry(queue).Collection(p => p.PatientsInQueue).Load();
                    queue.PatientsInQueue = queue.PatientsInQueue.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList();
                    patient2 = queue.PatientsInQueue[0];
                }
                
                //Returns 0 for Sanatorium and 1 for Queue
                //If Sanatorium has been loaded
                if (patient1 != null && patient2 != null)
                {
                    if (patient1.SymptomLevel.CompareTo(patient2.SymptomLevel) == 0)
                    {
                        return patient1.BirthDate < patient2.BirthDate ? 0 : 1;
                    }
                    return patient1.SymptomLevel > patient2.SymptomLevel ? 0 : 1;
                }
                //If Sanatorium has not been loaded
                else if(patient1 != null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
        public void UpdateSymptomLevelsForPatient()
        {
            using (var db = new HospitalDB())
            {
                var queue = db.Queue.Find(CurrentQueue.Id);
                db.Entry(queue).Collection(x => x.PatientsInQueue).Load();
                foreach (var patient in queue.PatientsInQueue)
                {
                    patient.SymptomLevel += SymptomUpdater(1); //1 för kö
                    db.SaveChanges();
                }
                var iva = db.IVA.Find(CurrrentIva.Id);
                db.Entry(iva).Collection(x => x.Patients).Load();
                foreach (var patient in iva.Patients)
                {
                    patient.SymptomLevel += SymptomUpdater(2); //1 för kö
                    db.SaveChanges();
                }
                var sana = db.Sanatorium.Find(CurrentSanatorium.Id);
                db.Entry(sana).Collection(x => x.Patients).Load();
                foreach (var patient in sana.Patients)
                {
                    patient.SymptomLevel += SymptomUpdater(3); //1 för kö
                    db.SaveChanges();
                }

            }
        }
        /// <summary>
        /// Uppdaterar symptomlevel för rätt department.
        /// </summary>
        /// <param name="department">kö, iva, eller sanatorium.</param>
        /// <returns>symptomförändring</returns>
        public int SymptomUpdater(int department)
        {
            int symptomChange = 0;
            Random rd = new Random();
            if (department == 1) //KÖ
            {
                int chance = rd.Next(1, 100);
                if (chance <= 50)
                {
                    symptomChange = 0;
                }
                if (chance >= 50 && chance <= 80)
                {
                    symptomChange = 1;
                }
                if (chance > 80 && chance <= 90)
                {
                    symptomChange = -1;
                }
                if (chance > 90)
                {
                    symptomChange = 3;
                }
            }
            else if (department == 2) //IVA
            {
                int chance = rd.Next(1, 100);
                if (chance <= 20)
                {
                    symptomChange = 0;
                }
                if (chance > 20 && chance <= 80)
                {
                    symptomChange = -3;
                }
                if (chance > 80 && chance <= 90)
                {
                    symptomChange = 1;
                }
                if (chance > 90)
                {
                    symptomChange = 2;
                }

            }
            else if (department == 3) //SANATORIUM
            {
                int chance = rd.Next(1, 100);
                if (chance <= 65)
                {
                    symptomChange = 0;
                }
                if (chance > 65 && chance <= 85)
                {
                    symptomChange = -1;
                }
                if (chance > 85 && chance <= 95)
                {
                    symptomChange = -1;
                }
                if (chance > 95)
                {
                    symptomChange = 3;
                }
            }
            return symptomChange;
        }
        public void DismissHealthyOrDeadPatients()
        {
            using (var db = new HospitalDB())
            {
                var queue = db.Queue.Find(CurrentQueue.Id);
                var afterLife = db.AfterLife.Find(CurrentAfterLife.Id);
                var healthy = db.Healthy.Find(CurrentHealthy.Id);
                var sanatorium = db.Sanatorium.Find(CurrentSanatorium.Id);
                var iva = db.IVA.Find(CurrrentIva.Id);
                db.Entry(queue).Collection(x => x.PatientsInQueue).Load();
                db.Entry(afterLife).Collection(x => x.Patients).Load();
                db.Entry(healthy).Collection(x => x.Patients).Load();
                db.Entry(sanatorium).Collection(x => x.Patients).Load();
                db.Entry(iva).Collection(x => x.Patients).Load();
                foreach (Patient patient in queue.PatientsInQueue.ToList())
                {
                    if (patient.SymptomLevel <= 0)
                    {
                        healthy.Patients.Add(patient);
                        queue.PatientsInQueue.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        queue.PatientsInQueue.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                }
                foreach (Patient patient in iva.Patients.ToList())
                {
                    if (patient.SymptomLevel <= 0)
                    {
                        healthy.Patients.Add(patient);
                        iva.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        iva.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                }
                foreach (Patient patient in sanatorium.Patients.ToList())
                {
                    if (patient.SymptomLevel <= 0)
                    {
                        healthy.Patients.Add(patient);
                        sanatorium.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        sanatorium.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient));
                        DismissedPatients++;
                    }
                }
            }

        }

    }


    public class HospitalEventArgs : EventArgs
    {
        public Patient Patient { get; set; }

        public HospitalEventArgs(Patient patient)
        {
            this.Patient = patient;
        }

    }
}
