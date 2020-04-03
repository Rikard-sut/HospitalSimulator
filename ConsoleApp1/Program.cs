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
        public static Object locker = new Object();
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
        //NEDAN LÅSER JAG MOVEPATIENTS OCH moveDeadOrHealthyPatients FÖR DÅ KAN MAN KÖRA SIMULERINGEN UTAN THREAD SLEEPS I MAXHASTIGHET OCH DEN BLIR
        //ÄNDÅ KORREKT.
        //Annars får man OptimisticConcurrencyException för att två trådar ändrar på samma entity samtidigt.
        //I thread.sleep settings vi fick i tentabeskrivningen fungerar dock programmet UTAN locks här för dom hinner nästan alltid bli klara
        //med databasupppdateringar innan en annan tråd går in och ändrar i samma enitity.

        public static void MovePatients()
        {
            while (hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                lock (locker)
                {
                    hospital.MoveAroundPatients();
                }
                //Thread.Sleep(5000);
            }
        }
        public static void ContinueToUpdateSymptoms()
        {
            while (hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                hospital.UpdateSymptomLevelsForPatient();
                //Thread.Sleep(3000);
            }
        }
        public static void ContinueToMoveHealhyOrDeadPatients()
        {
            while (hospital.DismissedPatients < hospital.NumberOfPatientsToSimulate)
            {
                lock (locker)
                {
                    hospital.DismissHealthyOrDeadPatients();
                }
                //Thread.Sleep(5000);
            }
        }
    }
}
