using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;
using TimeTrackingLib;

namespace TimeTrackingServer
{
    public partial class Form1 : Form
    {
        TimeTrackingDB.DB database;
        Dictionary<int, bool> MachinesChangeShiftData = new Dictionary<int, bool>();
        //static int CalculatingPeriodInMinutes = 10;
        //static int DurationMachineStopInMinutes = 5;

        static int TimeInMinutesMachineCountsWork = 5;
        static int TimeInMinutesMachineCountsStop = 5;
        static int TimeInSecondsToPreliminaryCalculateCycleTime = 120;
        static int PreliminaryCyclesToAccurateCalculateCycleTime = 10;

        DateTime LastMaintenanceOrederChecked = DateTime.MinValue;
        int MaintenanceOrederCheckedPeriodInMinutes = 10;

        HashSet<int> MachinesList = new HashSet<int>();
        int MaxMachines;

        Log log;
        public Form1()
        {
            InitializeComponent();
            log = new Log(Log.LogModules.Server);
            var r = new System.Configuration.AppSettingsReader();
            try
            {
                MaxMachines = (int)r.GetValue("MaximumMachines", typeof(int)) + 1;
            }
            catch { }
            try
            {
                TimeInMinutesMachineCountsWork = (int)r.GetValue("TimeInMinutesMachineCountsWork", typeof(int));
            }
            catch { }
            try
            {
                TimeInMinutesMachineCountsStop = (int)r.GetValue("TimeInMinutesMachineCountsStop", typeof(int));
            }
            catch { }
            try
            {
                TimeInSecondsToPreliminaryCalculateCycleTime = (int)r.GetValue("TimeInSecondsToPreliminaryCalculateCycleTime", typeof(int));
            }
            catch { }
            try
            {
                PreliminaryCyclesToAccurateCalculateCycleTime = (int)r.GetValue("PreliminaryCyclesToAccurateCalculateCycleTime", typeof(int));
            }
            catch { }

            database = new TimeTrackingDB.DB();
            CheckMachinesChangeShiftData();
            timer1_Tick(timer1, new EventArgs());
        }

        private void CheckMachinesChangeShiftData()
        {
            var shift = new TimeTrackingLib.Shift();
            var now = DateTime.Now;
            var from = DateTime.Now.AddMinutes(-TimeInMinutesMachineCountsWork);
            var machines = Enumerable.Range(1, MaxMachines).ToList();
            //var machines = TimeTrackingDB.DB.GetMachinesList();
            //var orders = TimeTrackingDB.DB.GetCycles(from);
            var _d8 = now.TimeOfDay - Shift._8;
            var _d20 = now.TimeOfDay - Shift._20;
            foreach (var machine in machines)
            {
                try
                {
                    if (!MachinesChangeShiftData.ContainsKey(machine))
                        MachinesChangeShiftData.Add(machine, false);
                    MachineCycles cycles = MachineCycles.Get(from, machine);
                    if (cycles.Count == 0 || (now - cycles.LastDataDateTime()).TotalMinutes > TimeInMinutesMachineCountsStop)
                    {
                        MachinesChangeShiftData[machine] = false;
                    }
                    else
                    {
                        var avgCycles = cycles.AvgCycleTime(TimeInSecondsToPreliminaryCalculateCycleTime);
                        if (avgCycles.Count > 0 && avgCycles.Max() < TimeInMinutesMachineCountsStop * 60)
                        {
                            var avg = avgCycles.Average();
                            var avgCyclesNormilized = cycles.AvgCycleTime(avg * PreliminaryCyclesToAccurateCalculateCycleTime);
                            if (avgCyclesNormilized.Count > 0) avgCycles = avgCyclesNormilized;
                            if (avgCycles.Count > 0)
                            {
                                var lastCycleTime = avgCycles.Max();
                                if (lastCycleTime > TimeInMinutesMachineCountsStop * 60)
                                {
                                    MachinesChangeShiftData[machine] = false;
                                }
                                else
                                {
                                    var min = avgCycles.Min();
                                    var max = avgCycles.Max();
                                    if (max - min < 2 && avgCycles.StdDev() < 1)
                                    {
                                        MachinesChangeShiftData[machine] = true;
                                    }
                                    else
                                    {
                                        //MachinesChangeShiftData[machine] = false;
                                    }
                                }
                            }
                            else
                            {
                                MachinesChangeShiftData[machine] = false;
                            }
                        }
                        else
                        {
                            MachinesChangeShiftData[machine] = false;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void StartIdle(int EquipmentNumber, DateTime dt)
        {
            database.SetMachineStatus(EquipmentNumber, TimeTrackingDB.DB.MachineStatus.Idle, Time: dt, log: log);
        }

        public void EndIdle(int EquipmentNumber, DateTime dt)
        {
            database.SetMachineStatus(EquipmentNumber, TimeTrackingDB.DB.MachineStatus.Working, Time: dt, log: log);
        }

        public void CheckSAP()
        {
            var shift = new TimeTrackingLib.Shift();
            var fromShift = shift.PreviousShift().PreviousShift().PreviousShift().PreviousShift();
            var orders = TimeTrackingSAP.MaintenanceOrders.GetOrders(fromShift.ShiftStartsAt(), shift.ShiftEndsAt(), true);
            orders = orders.FindAll(o => o.StartDateTime > fromShift.ShiftDate && o.EndDateTime <= DateTime.Now);
            log.Add("Заказы ТОРО:\r\n" + String.Join("\r\n", orders.Select(o => $"ID:{o.OrderID} с {o.StartDateTime:dd-MM-yyyy HH:mm:ss} по {o.EndDateTime:dd-MM-yyyy HH:mm:ss}, Текст:{o.ActionText}")));
            var OrdersParts = new List<TimeTrackingSAP.MaintenanceOrder>();
            foreach (var ord in orders)
            {
                if (ord.Equipment > 0)
                    OrdersParts.AddRange(ord.DivideOrderForShifts());
            }

            foreach (var order in OrdersParts)
            {
                database.InsertSAPRecord(order.StartDateTime, order.EndDateTime, order.Equipment, order.OrderID, order.Action, log);
            }
        }

        bool ShiftChanged = false;

        public void CheckAreMachinesWorking()
        {
            var now = DateTime.Now;
            //var from = DateTime.Now.AddMinutes(-TimeInMinutesMachineCountsWork);
            var machines = Enumerable.Range(1, MaxMachines).ToList();
            //var machines = TimeTrackingDB.DB.GetMachinesList();
            //var orders = TimeTrackingDB.DB.GetCycles(from);
            var _d8 = now.TimeOfDay - Shift._8;
            var _d20 = now.TimeOfDay - Shift._20;
            var shift = new Shift();

            CheckMachinesChangeShiftData();
            database.RefreshContext();
            //открытые простои предыдущих периодов
            var before = shift.ShiftStartsAt();
            var idles = database.EquipmentIdles.Where(i => !i.IdleEnd.HasValue && i.IdleStart < before).ToList();
            idles.ForEach(e =>
            {
                var idleShift = new Shift(e.IdleStart.Value);
                e.IdleEnd = idleShift.ShiftEndsAt();
                e.IsOpenIdle = false;
            }
            );
            database.SaveChanges();

            var MinutesSinceLastCheckOfMaintenanceOrders = (now - LastMaintenanceOrederChecked).TotalMinutes;
            if (MinutesSinceLastCheckOfMaintenanceOrders > MaintenanceOrederCheckedPeriodInMinutes)
            {
                CheckMaintenanceOrders(shift);
                LastMaintenanceOrederChecked = now;
            }
            // в течении минуты после 8 часов
            if (_d8.TotalSeconds >= 0 && _d8.TotalSeconds < 60 || _d20.TotalSeconds >= 0 && _d20.TotalSeconds < 60)
            {
                // если еще не обрабатывали изменение смены
                if (!ShiftChanged)
                {
                    try
                    {
                        // проходимся по всем машинам
                        foreach (var machine in machines)
                        {
                            if (MachinesChangeShiftData.ContainsKey(machine))
                                // если она работает
                                if (MachinesChangeShiftData[machine])
                                {
                                    // и не работала
                                    if (database.GetIsMachineIdle(machine))
                                    {
                                        // говорим, что запущена
                                        EndIdle(machine, shift.ShiftStartsAt());
                                    }
                                }
                                else //если она не работает
                                {
                                    //и до этого тоже не работала
                                    if (database.GetIsMachineIdle(machine))
                                    {
                                        //то заканчиваем простой предыдущей смены и начинаем в новой
                                        EndIdle(machine, shift.ShiftStartsAt());
                                        StartIdle(machine, shift.ShiftStartsAt());
                                    }
                                    else// если она работала
                                    {
                                        // начинаем простой
                                        StartIdle(machine, shift.ShiftStartsAt());
                                    }

                                }

                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add("Ошибка во время пересменки при обработке простоев", ex);
                    }
                    finally
                    {
                        ShiftChanged = true;
                    }

                    // Заполнить работников текущей смены
                    try
                    {
                        if (!IsMaintenanceEmployeeFilled(shift))
                            FillCurrentShift();
                    }
                    catch (Exception ex)
                    {
                        log.Add("Ошибка во время пересменки при обработке работников", ex);
                    }

                }
            }
            else
            {
                ShiftChanged = false;
            }

            foreach (var machine in machines)
            {
                if (MachinesChangeShiftData.ContainsKey(machine))
                    // если она работает
                    if (MachinesChangeShiftData[machine])
                    {
                        // и не работала
                        if (database.GetIsMachineIdle(machine))
                        {
                            // говорим, что запущена
                            EndIdle(machine, now);
                        }
                    }
                    else //если она не работает
                    {
                        //и до этого работала
                        if (!database.GetIsMachineIdle(machine))
                        {
                            // начинаем простой
                            StartIdle(machine, now);
                        }
                    }
            }

            FillMachineWorksListView(listView1);
        }

        private void FillMachineWorksListView(ListView lv)
        {
            lv.Columns.Clear();
            lv.Columns.Add(new ColumnHeader() { Text = "№ машины", Width = 100 });
            lv.Columns.Add(new ColumnHeader() { Text = "Работа", Width = 150 });

            lv.Items.Clear();
            for (int machine = 1; machine < MaxMachines; machine++)
            {
                lv.Items.Add(new ListViewItem(new string[] { $"Petline {machine}", MachinesChangeShiftData.ContainsKey(machine) ? (MachinesChangeShiftData[machine] ? "Работает" : "Не работает") : "" }));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                log.Add("Очистка неправильных простоев");
                ClearIdles();
            }
            catch (Exception ex)
            {
                log.Add("Ошибка при очистке неправильных простоев", ex);
            }
            try
            {
                log.Add("Определение простоев");
                CheckAreMachinesWorking();
            }
            catch (Exception ex)
            {
                log.Add("Ошибка при определении простоев", ex);
            }
            try
            {
                log.Add("Проверка данных SAP");
                CheckSAP();
            }
            catch (Exception ex)
            {
                log.Add("Ошибка при проверке данных SAP", ex);
            }
            log.Add("Обработка завершена");

        }
        private bool IsMaintenanceEmployeeFilled(Shift shift)
        {
            var cnt = database.MaintainShiftEmployees.Count(me => me.ShiftDate == shift.ShiftDate);
            return cnt > 0;
        }

        private void FillCurrentShift()
        {
            var now = DateTime.Now;
            var MaintananceShift = new MaintananceShift(now);// (((int)((now.Date - DateTime.MinValue).TotalDays + 1)) % 4) / 2 + 1;
            var shift = new Shift();

            var supervisor = GetSupervisorByShift(MaintananceShift.ShiftNumber);
            database.MaintainShiftSupervisor.Add(new MaintainShiftSupervisor() { ShiftDate = shift.ShiftDate, IsNightShift = shift.IsNight, FIO = supervisor.FIO });

            var Workers = GetEmployeeByShift(MaintananceShift.ShiftNumber);

            for (int i = 0; i < Workers.Count; i++)
            {
                var w = Workers[i];
                var mse = Employee2MaintainShiftEmployee(w);

                database.MaintainShiftEmployees.Add(mse);
            }

            if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
            {

                var WeekWorkers = GetEmployeeByShift(0);

                for (int i = 0; i < WeekWorkers.Count; i++)
                {
                    var w = WeekWorkers[i];
                    var mse = Employee2MaintainShiftEmployee(w);

                    database.MaintainShiftEmployees.Add(mse);
                }

            }
            database.SaveChanges();
        }


        private MaintainShiftEmployee Employee2MaintainShiftEmployee(Employee e)
        {
            var currentShift = new Shift();
            var mse = new MaintainShiftEmployee();
            mse.EmployeePositionID = e.EmployeePositionID;
            mse.FIO = e.FIO;
            mse.ShiftDate = currentShift.ShiftDate;
            mse.WorkingHours = e.WorkingHours;
            return mse;
        }

        public List<Supervisor> GetSupervisors()
        {
            using (var db = new DB())
            {
                var supervisors = db.Supervisors.ToList();
                return supervisors;
            }
        }
        public List<Employee> GetEmployeeByShift(int Shift)
        {
            return database.Employees.Where(e => e.Shift == Shift).ToList();
        }
        public Supervisor GetSupervisorByShift(int Shift)
        {
            return database.Supervisors.Where(e => e.MaintenanceShift == Shift).FirstOrDefault();
        }

        private void CheckMaintenanceOrders(Shift shift)
        {
            var prvShift = shift.PreviousShift().PreviousShift().PreviousShift().PreviousShift();
            var orders = TimeTrackingSAP.MaintenanceOrders.GetOrders(prvShift.ShiftStartsAt(), shift.ShiftEndsAt(), false);
            var OrdersParts = new List<TimeTrackingSAP.MaintenanceOrder>();
            foreach (var ord in orders)
            {
                if (ord.Equipment > 0)
                    OrdersParts.AddRange(ord.DivideOrderForShifts());
            }

            using (var db = new DB())
            {
                foreach (var order in OrdersParts)
                {
                    db.InsertSAPRecord(order.StartDateTime, order.EndDateTime, order.Equipment, order.OrderID, order.Action, log);
                }
            }
        }

        public void ClearIdles()
        {
            using (var db = new DB())
            {
                var emptyrecs = db.EquipmentIdles.Where(ei => !ei.IdleStart.HasValue && !ei.IdleEnd.HasValue);
                var emptyrecIDs = emptyrecs.Select(er => er.EquipmentIdleID).ToList();
                foreach (var rec in emptyrecs)
                {
                    db.EquipmentIdles.Remove(rec);

                }
                var parents = db.EquipmentIdles.Where(ei => emptyrecIDs.Contains(ei.DivisionChildEquipmentIdleID.Value));
                var children = db.EquipmentIdles.Where(ei => emptyrecIDs.Contains(ei.DivisionParentEquipmentIdleID.Value));

                foreach (var p in parents)
                {
                    p.DivisionChildEquipmentIdleID = null;
                }
                foreach (var c in children)
                {
                    c.DivisionParentEquipmentIdleID = null;
                }

                var less5minrecs = db.EquipmentIdles.Where(ei => (ei.IdleStart.HasValue && ei.IdleEnd.HasValue) && ((ei.IdleEnd < ei.IdleStart) ? DbFunctions.DiffMinutes(ei.IdleStart, DbFunctions.AddDays(ei.IdleEnd, 1)) : DbFunctions.DiffMinutes(ei.IdleStart, ei.IdleEnd)).Value < 5);
                foreach (var rec in less5minrecs)
                    db.EquipmentIdles.Remove(rec);
                db.SaveChanges();
            }
        }
    }
}
