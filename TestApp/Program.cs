using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
//using TimeTrackingSAP;
//using TimeTrackingLib;
//using TimeTrackingDB;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime? dt = DateTime.Now;
            Console.WriteLine($"{dt:dd-MM-yyyy HH\\:mm\\:ss}");
            //var orders = TimeTrackingSAP.MaintenanceOrders.GetOrders(new DateTime(2024,01,01,08,00,00), new DateTime(2024, 01, 29, 08, 00, 00), false);
            //orders = orders.Where(o => o.Equipment != -1).ToList();
            //var orderList = orders.Select(o=>o.OrderID).ToList();
            /*var now = DateTime.Now;
            var mc = new MachineCycles();
            mc.SetData(
                new List<MachineCycles.RainTCycleCounter>(){
                    new MachineCycles.RainTCycleCounter() { DateTime = now, Counter = 4, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(20), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(40), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(60), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(80), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(100), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(120), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(140), Counter = 5, Eq = 1 },
                    new MachineCycles.RainTCycleCounter() { DateTime = now.AddSeconds(160), Counter = 6, Eq = 1 },

                }
                );*/
            /*var mc = MachineCycles.Get(DateTime.Now.AddMinutes(-15), 14);
            var cycle = mc.AvgCycleTime(120);
            var minTime = cycle.Min();
            var dcycle = cycle.Select(c => c - minTime).ToList();
            var max = dcycle.Max();
            var avg = dcycle.Average();
            var avg2 = dcycle.Average(d => d * d);
            var disp = avg2 - avg * avg;
            var sigm = Math.Sqrt(disp);
            var ms = mc.MachineWasStoppedAt();

            var data1 = new List<double>() { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            var std1 = data1.StdDev();
            var data2 = new List<double>() { 2, 2, 2, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3 };
            var std2 = data2.StdDev();



            Console.BufferWidth = 200;

            var start = new DateTime(2023, 10, 21, 8, 01, 0);
            var end = new DateTime(2023, 10, 21, 20, 0, 0);
            var StartShift = new Shift(start);
            var EndShift = new Shift(end);
            var res = MaintenanceOrders.GetMaintenanceOrderOperations(start, end, true).OrderBy(m => m.StartDateTime).ToList();
            Console.WriteLine($"Заказ\t\tТехкарта\tНачало\t\t\tКонец\t\t\tМашина\tТип ППР\t\tОперация\t\t\t\t\tДлит.опер.\t\t(Конец-Начало)");
            for (int i = 0; i < res.Count; i++)
            {
                var r = res[i];
                Console.WriteLine($"{r.OrderID}\t{r.TechCardID}\t{r.StartDateTime}\t{r.EndDateTime}\t{r.Equipment}\t{r.Action}\t{r.OperationName}\t{r.DurationBase} {r.DurationBaseMeasureUnit}*{r.Number} чел={r.DurationTotal} {r.DurationTotalMeasureUnit}\t({(r.EndDateTime - r.StartDateTime).TotalMinutes.ToString("F0")})");
            }

            using (var database = new DB())
            {
                var NotAuxilaryEmployeePositions = database.EmployeePositions.Where(ep => !ep.IsAuxiliary).Select(ep => ep.EmployeePositionID).ToList();
                var EmployeeShifts = new Dictionary<int, List<Employee>>()
                {
                    {1, database.Employees.Where(e => e.Shift == 1 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },
                    {2, database.Employees.Where(e => e.Shift == 2 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },

                };
                var supervisors = database.Supervisors.Where(s => new int[] { 1, 2 }.Contains(s.MaintenanceShift)).ToDictionary(s => s.MaintenanceShift);


                var shift = StartShift;
                var MaintenanceEmployees = database.MaintainShiftEmployees.Where(e => e.ShiftDate >= StartShift.ShiftDate && e.ShiftDate <= EndShift.ShiftDate && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList();
                while (shift <= EndShift)
                {
                    var MaintananceShift = new MaintananceShift(shift.ShiftDate);
                    var employees = MaintenanceEmployees.FindAll(me => me.ShiftDate == shift.ShiftDate);
                    int AvailableTime;
                    if (employees.Count == 0)
                    {
                        AvailableTime = employees.Sum(e => e.WorkingHours + e.AdditionalHours);
                    }
                    else
                    {
                        AvailableTime = EmployeeShifts[MaintananceShift.ShiftNumber].Sum(e => e.WorkingHours);
                    }
                    Console.WriteLine(new TimeSpan(AvailableTime, 0, 0));

                    shift = shift.NextShift().NextShift();

                }
            }*/
            Console.ReadKey();
        }
    }


}
