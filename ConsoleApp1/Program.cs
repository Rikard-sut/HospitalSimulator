using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using static Simulator.CodeFirstDB;
using Methods;
using System.Diagnostics;

namespace Simulator
{
    class Program
    {
        public static Hospital hospital = new Hospital();
        static void Main(string[] args)
        {
            hospital.PatientMovedEventHandler += Logger.Log;
            RunThreads();
            Console.ReadKey();
        }
        public static void RunThreads()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread thread1 = new Thread(hospital.AddPatients);
            Thread thread2 = new Thread(MovePatients);
            Thread thread3 = new Thread(ContinueToUpdateSymptoms);
            Thread thread4 = new Thread(ContinueToMoveHealhyOrDeadPatients);
            thread1.Start();
            thread1.Join();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread4.Join();
            stopwatch.Stop();
            Console.Write("Tid för simulering {0} millisekunder", stopwatch.ElapsedMilliseconds);
        }  
        public static void MovePatients()
        {
            while(hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                hospital.MoveAroundPatients();
                Thread.Sleep(5000);
            }
        }
        public static void ContinueToUpdateSymptoms()
        {
            while (hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                hospital.UpdateSymptomLevelsForPatient();
                Thread.Sleep(3000);
            }
        }
        public static void ContinueToMoveHealhyOrDeadPatients()
        {
            while (hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                hospital.DismissHealthyOrDeadPatients();
                Thread.Sleep(5000);
            }
            Console.WriteLine("Simulering klar");
        }
    }
}
