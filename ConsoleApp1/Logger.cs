
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator
{
    /// <summary>
    /// Loggar till skärm OCH fil vid FLYTT OCH RECOVERIES SAMT BORTGÅNG.
    /// </summary>
    class Logger
    {
        private static int DeadPatients = 0;
        private static int RecoveredPatients = 0;
        public static Object streamLocker = new Object();
        public static void Log(object sender, HospitalEventArgs e)
        {
            //Väldigt sällan kraschar programmet för olika trådar använder denna metod samtidigt. isf får dom vänta med hjälp av denna lock.
            lock(streamLocker)
            { 

                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    if (e.Patient != null)
                    {
                        // Om sanatorium är null och IVA är inte null så befann sig patienten i kön och flyttades till iva.
                        if (e.Patient.Sanatorium == null && e.Patient.IVA != null)
                        {
                            Console.WriteLine("Flyttade patient med namn {0} till IVA från Kön. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                            w.WriteLine("Flyttade patient med namn {0} till IVA från Kön. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                        }
                        //Om sanatorium inte är null och iva inte är null så flyttade vi patienten från sana till iva.
                        if (e.Patient.Queue == null && e.Patient.Sanatorium != null && e.Patient.IVA != null)
                        {
                            Console.WriteLine("Flyttade patient med namn {0} till IVA från Sanatorium. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                            w.WriteLine("Flyttade patient med namn {0} till IVA från Kön. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                        }
                        //Om kö inte är null och sanatorium inte är null flyttade vi från kö till sanatorium.
                        if (e.Patient.Queue != null && e.Patient.Sanatorium != null)
                        {
                            Console.WriteLine("Flyttade patient med namn {0} till Sanatorium från Kön. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                            w.WriteLine("Flyttade patient med namn {0} till Sanatorium från Kön. Sjukdomslevel {1}", e.Patient.Name, e.Patient.SymptomLevel);
                        }
                        //Om afterlife inte är null är han död.
                        if (e.Patient.Afterlife != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Patient med namn {0} gick bort", e.Patient.Name);
                            w.WriteLine("Patient med namn {0} gick bort", e.Patient.Name);
                            DeadPatients++;
                            Console.WriteLine("Toalt Döda patienter {0}", DeadPatients);
                            Console.ResetColor();
                        }
                        //om Healthy inte är null är patienten frisk.
                        if (e.Patient.Healthy != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Patient med namn {0} blev frisk", e.Patient.Name);
                            w.WriteLine("Patient med namn {0} gick bort", e.Patient.Name);
                            RecoveredPatients++;
                            Console.WriteLine("Friska patienter {0}", RecoveredPatients);
                            Console.ResetColor();
                        }
                    }
                }
            }
        }
    }
}
