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
    /// <summary>
    /// Klass för Patientmetoder och sjukhuslogik.
    /// </summary>
    class Hospital
    {
        //Ändra Denna variabel för att köra mot mer eller mindre patienter.
        public int NumberOfPatientsToSimulate = 30;
        //Variabel för att styra hela simuleringen. Körs till Dismisspatients är > NumberOfPatientsToSimulate.
        public int DismissedPatients = 0;
        //Skapar en instans av alla sjukhusavdelnignar för att lätt kunna ladda in de aktualla avdelningarna från databasen.
        //Hittar alltid därför rätt avdelning i databasen med hjälp av tex CurrentQueue.Id.
        static Queue CurrentQueue = new Queue(); 
        static IVA CurrrentIva = new IVA();
        static Sanatorium CurrentSanatorium = new Sanatorium();
        static Afterlife CurrentAfterLife = new Afterlife();
        static Healthy CurrentHealthy = new Healthy();

        public event EventHandler<HospitalEventArgs> PatientMovedEventHandler;
        /// <summary>
        /// EventRaiser. Kallar på denna när hospital skall raisa ett event.
        /// </summary>
        /// <param name="e"></param>
        internal virtual void OnPatientMoved(HospitalEventArgs e)
        {
            PatientMovedEventHandler?.Invoke(this, e);
        }
        /// <summary>
        /// Lägger till 30 patienter. Ändra på NumberOfPatientsToSimulate om man vill köra större simulering.
        /// </summary>
        public void AddPatients()
        {
            using (var db = new HospitalDB())
            {
                Console.WriteLine("Loading Patients....");

                for (int i = 0; i < NumberOfPatientsToSimulate; i++)
                {
                    Thread.Sleep(150); //För att det inte ska bli dubletter av Patient. Random arbetar med clockcykler
                    var patient = new Patient(Randomers.GenerateName(), Randomers.GenerateSSN(), Randomers.GenerateSymptomLevel());
                    db.Patients.Add(patient);
                    CurrentQueue.PatientsInQueue.Add(patient);
                }

                //sorterar databasen för att det skulle vara lättare att följa under testning osv. fyller ingen funktion för programmets funktionalitet.
                CurrentQueue.PatientsInQueue = CurrentQueue.PatientsInQueue.OrderByDescending(x => x.SymptomLevel).ThenBy(x => x.BirthDate).ToList(); //Sorterar före inläggning i db.
                db.Queue.Add(CurrentQueue);
                db.IVA.Add(CurrrentIva);
                db.Sanatorium.Add(CurrentSanatorium);
                db.AfterLife.Add(CurrentAfterLife);
                db.Healthy.Add(CurrentHealthy);
                //Lägger till alla avdelningar i databasen så dom finns att ladda in senare.
                db.SaveChanges();
            }
        }

        public void MoveAroundPatients()
        {
            using (var db = new HospitalDB())
            {
                //Laddar in rätt avdelningar från databasen.
                var queue = db.Queue.Find(CurrentQueue.Id);
                var sanatorium = db.Sanatorium.Find(CurrentSanatorium.Id);
                var iva = db.IVA.Find(CurrrentIva.Id);
               
                
                //Laddar in avdelningarnas patienter om dom har patienter i sig.
                //Behöver inte load IVA för vi ska inte ta bort ifrån iva i detta läget.
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
                //Fylller på 5 patienter i IVA första varvet, sedan fyller vi på när plats finns.
                while (iva.Patients.Count < 5)
                {
                    int result = FindSickestPatient();
                    Patient patient;
                    //Om patient kom från sanatorium
                    if (result == 0)
                    {
                        patient = sanatorium.Patients.FirstOrDefault();
                        iva.Patients.Add(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient)); //Raisar Event att flytt skett
                        sanatorium.Patients.Remove(patient);
                    }
                    //Om patient kom från kön
                    else
                    {
                        patient = queue.PatientsInQueue.FirstOrDefault();
                        iva.Patients.Add(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient)); //Raisar event att flytt skett
                        queue.PatientsInQueue.Remove(patient);

                    }
                }
                db.SaveChanges();
                //Första gången lägger vi till 10 patienter i sanatorium, annars fyller vi på efterhand.
                while (sanatorium.Patients.Count < 10)
                {
                    var patient = queue.PatientsInQueue.FirstOrDefault();
                    sanatorium.Patients.Add(patient);
                    db.SaveChanges();
                    OnPatientMoved(new HospitalEventArgs(patient));
                    queue.PatientsInQueue.Remove(patient);
                }
                db.SaveChanges();

            }
        }
        /// <summary>
        /// Hittar den sjukaste patienten från kö eller sanatorium
        /// </summary>
        /// <returns>1 eller 0 som representation av vilken avdeling vi ska ta bort ifrån</returns>
        private static int FindSickestPatient()
        {
            using (var db = new HospitalDB())
            {
                var sanatorium = db.Sanatorium.Find(CurrentSanatorium.Id);
                var queue = db.Queue.Find(CurrentQueue.Id);
                Patient patient1 = null;
                Patient patient2 = null;
                
                //Försöker ladda in kölista och sanatoriumlista
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
                
                //Returnerar 0 för sanatorium och 1 för kön.
                //Hittade vi sjuk patient från båda avdelningarna jämför vi först
                //Symptomlevel och sedan Birthdate.
                if (patient1 != null && patient2 != null)
                {
                    if (patient1.SymptomLevel.CompareTo(patient2.SymptomLevel) == 0)
                    {
                        return patient1.BirthDate < patient2.BirthDate ? 0 : 1;
                    }
                    return patient1.SymptomLevel > patient2.SymptomLevel ? 0 : 1;
                }
                //Om sanatorium inte laddade är patient2 null och därför gör vi ingen jämföring ovan och returnerar direkt 0 för kö.
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
        /// <summary>
        /// Uppdaterar patienternas symptomLevelel.
        /// </summary>
        public void UpdateSymptomLevelsForPatient()
        {
            using (var db = new HospitalDB())
            {
                //Laddar in listan för varje avdelning. och för varje patient i denna lista kör vi metoden SymptomUpdater += patient.SymptomLevel.
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
        /// <summary>
        /// Flyttar patienter till Healthy eller Afterlive om deras symptomlevel är under 0 eller 10 och uppåt.
        /// </summary>
        public void DismissHealthyOrDeadPatients()
        {
            using (var db = new HospitalDB())
            {
                //Samma här. Laddar in ALLA listor denna gången för en patient kan bli frisk eller dör VARSOMHELST i sjukhuset.
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
                //Gör samma foreach för VARJE avdelning för att se vilka patienter som skall flyttas till healthy eller afterlife.
                foreach (Patient patient in queue.PatientsInQueue.ToList())
                {
                    if (patient.SymptomLevel <= 0)
                    {
                        healthy.Patients.Add(patient);          // vi lägger till i healthy eller afterlife beroende på IF. sedan tar bort från
                        queue.PatientsInQueue.Remove(patient); // deras nuvarande plats, sen raisar event att det skett. alla loopar ser lika ut.
                        db.SaveChanges();                      // bara vilka avdelningar vi arbetar med som skiljer sig.
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                        DismissedPatients++; //++ på dismissedpatients, denna variabel styr hela programmet. Dvs när alla patienter flyttats ut
                                             // ur sjukhuset. (dom ligger i afterlife eller Healthy) kommer det sluta.
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        queue.PatientsInQueue.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
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
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                        DismissedPatients++;
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        iva.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
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
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                        DismissedPatients++;
                    }
                    if (patient.SymptomLevel >= 10)
                    {
                        afterLife.Patients.Add(patient);
                        sanatorium.Patients.Remove(patient);
                        db.SaveChanges();
                        OnPatientMoved(new HospitalEventArgs(patient)); //raisar event att flytt skett.
                        DismissedPatients++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Eventargs för flytt. Denna funkar även för att skicka med info om tillfriskande och bortgång.
    /// </summary>
    public class HospitalEventArgs : EventArgs
    {
        public Patient Patient { get; set; }

        public HospitalEventArgs(Patient patient)
        {
            this.Patient = patient;
        }
    }
}
