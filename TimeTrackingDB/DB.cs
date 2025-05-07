using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TimeTrackingLib;

namespace TimeTrackingDB
{
    public class DB : DbContext
    {
        static string connectionStringPlanning = ConfigurationManager.ConnectionStrings["connectionStringPlanning"].ConnectionString;
        static string connectionStringPetPro = ConfigurationManager.ConnectionStrings["connectionStringPetPro"].ConnectionString;
        public DB() : base("TimeTrackingDB")
        {
            /*Database.Log = new Action<string>(s =>
            {
                using (var sw = new StreamWriter("sql.log", true))
                {
                    sw.WriteLine(s);
                }
            }
            );*/
            Database.SetInitializer<DB>(new DBInitializer());
        }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }
        //public DbSet<Shift> Shifts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<TypeOfWork> TypeOfWorks { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<MalfunctionReasonType> MalfunctionReasonTypes { get; set; }
        public DbSet<MalfunctionReasonProfile> MalfunctionReasonProfiles { get; set; }
        public DbSet<MalfunctionReasonNode> MalfunctionReasonNodes { get; set; }
        public DbSet<MalfunctionReasonElement> MalfunctionReasonElements { get; set; }
        public DbSet<MalfunctionReasonMalfunctionText> MalfunctionReasonMalfunctionTexts { get; set; }
        public DbSet<EquipmentIdle> EquipmentIdles { get; set; }
        public DbSet<MaintainShiftEmployee> MaintainShiftEmployees { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<MaintainShiftSupervisor> MaintainShiftSupervisor { get; set; }
        public DbSet<EquipmentBlocking> EquipmentBlockings { get; set; }
        public DbSet<AdditionalIdleRecord> AdditionalIdleRecords { get; set; }
        public DbSet<AdditionalIdleRecordFile> AdditionalIdleRecordFiles { get; set; }


        public static List<User> GetUsersAvailableForComputerName(string ComputerName)
        {
            using (var db = new DB())
            {
                return db.Users.Where(u => String.IsNullOrEmpty(u.AutoLogonComputerNames) || u.AutoLogonComputerNames.Contains(ComputerName) || u.AutoLogonComputerNames.Contains("*")).ToList();
            }
        }
        public static User CheckUserPassword(string UserName, string PasswordHash)
        {
            using (var db = new DB())
            {
                return db.Users.Where(u => u.UserName == UserName && u.PasswordHash == PasswordHash).SingleOrDefault();
            }
        }

        public static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static List<User> GetUsers()
        {
            using (var db = new DB())
            {
                return db.Users.ToList();
            }
        }

        public static void AddUser(User user)
        {
            using (var db = new DB())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public static bool UpdateUser(User user)
        {
            using (var db = new DB())
            {
                var entity = db.Users.Find(user.UserID);
                if (entity == null) return false;
                db.Entry(entity).CurrentValues.SetValues(user);
                db.SaveChanges();
            }
            return true;
        }
        public static void AddOrUpdateUser(User user)
        {
            if (!UpdateUser(user))
                AddUser(user);
        }

        public static void DeleteUser(User user)
        {
            using (var db = new DB())
            {
                db.Users.Attach(user);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public void EquipmentIdleDivideRecord(EquipmentIdle ei, Log log)
        {
            log?.Add($"Попытка деления записи с {ei.IdleStart:dd-MM-yyyy HH:mm:ss} по {ei.IdleEnd:dd-MM-yyyy HH:mm:ss} на машине {ei.EquipmentNumber} на две");
            if (ei == null) throw new RecordCannotBeDividedException();
            if (!ei.IsAbleToDivide) throw new RecordCannotBeDividedException();
            RefreshContext();
            ei = EquipmentIdles.Where(e => e.EquipmentIdleID == ei.EquipmentIdleID).FirstOrDefault();
            if (ei == null) throw new RecordCannotBeDividedException();
            var newEI = new EquipmentIdle();

            newEI.ShiftStart = ei.ShiftStart;
            newEI.IsNightShift = ei.IsNightShift;
            newEI.EquipmentNumber = ei.EquipmentNumber;
            newEI.IdleStart = ei.IdleStart;
            newEI.IdleEnd = null;
            newEI.SAPOrderID = ei.SAPOrderID;
            newEI.DivisionParentEquipmentIdleID = null;
            newEI.DivisionChildEquipmentIdleID = ei.EquipmentIdleID;
            newEI.IsOpenIdle = ei.IsOpenIdle;

            ei.IdleStart = null;
            EquipmentIdles.Add(newEI);

            SaveChanges();
            ei.DivisionParentEquipmentIdleID = newEI.EquipmentIdleID;
            SaveChanges();
            log?.Add($"Запись1: с {newEI.IdleStart:dd-MM-yyyy HH:mm:ss} по {newEI.IdleEnd:dd-MM-yyyy HH:mm:ss}");
            log?.Add($"Запись2: с {ei.IdleStart:dd-MM-yyyy HH:mm:ss} по {ei.IdleEnd:dd-MM-yyyy HH:mm:ss}");
        }


        /*public bool IsAbleToInsertSAPRecord(DateTime SAPPeriodStart, DateTime SAPPeriodEnd, Log log)
        {
            Shift shift = new Shift(SAPPeriodStart);
            log.Add("Поверка наличия по");

            var fitPeriods = EquipmentIdles.Where(ei =>
                ei.ShiftStart == shift.ShiftDate
                &&
                ei.IsNightShift == shift.IsNight
                &&
                ei.IdleEnd.HasValue
                &&
                ei.IdleStart.HasValue
                &&
                (
                    (
                        SAPPeriodStart.TimeOfDay <= ei.IdleEnd.Value.TimeOfDay
                        &&
                        SAPPeriodStart.TimeOfDay >= ei.IdleStart.Value.TimeOfDay
                    )
                    ||
                    (
                        SAPPeriodEnd.TimeOfDay >= ei.IdleStart.Value.TimeOfDay
                        &&
                        SAPPeriodEnd.TimeOfDay <= ei.IdleEnd.Value.TimeOfDay
                    )
                )
            ).ToList();
            log.Add();
            if (fitPeriods.Count == 1)
            {
                return true;
            }
            return false;
        }*/

        Dictionary<string, Tuple<int?, int?, int?, int?, int?>> Strategy2TextID = new Dictionary<string, Tuple<int?, int?, int?, int?, int?>>()
        {
            {"5 ППР", new Tuple<int?,int?,int?, int?,int?>(1,null,null,null,394)},
            {"М_ППР", new Tuple<int?,int?,int?, int?,int?>(1,null,null,null,407)},
            {"Г_ППР", new Tuple<int?,int?,int?, int?,int?>(1,null,null,null,408)}
        };
        public void InsertSAPRecord(DateTime SAPPeriodStart, DateTime SAPPeriodEnd, int EquipmentNumber, string SAPOrderID, string SAPStrategy, Log log)
        {
            if (SAPStrategy == null || !Strategy2TextID.ContainsKey(SAPStrategy)) return;

            Shift shift = new Shift(SAPPeriodStart);
            //var starttime = new TimeSpan?(SAPPeriodStart.TimeOfDay);
            //var endtime = new TimeSpan?(SAPPeriodEnd.TimeOfDay);
            var now = DateTime.Now;

            //TODO: проверить, что такого заказа еще не внесено
            var fitPeriods = EquipmentIdles.Where(ei =>
                ei.EquipmentNumber == EquipmentNumber &&
                ei.ShiftStart == shift.ShiftDate &&
                ei.IsNightShift == shift.IsNight &&
                (ei.IdleEnd.HasValue || DateTime.Now > SAPPeriodEnd) &&
                ei.IdleStart.HasValue &&
                // период заказа не полностью до простоя и не полностью после
                !(
                     ei.IdleStart.Value <= SAPPeriodStart
                     &&
                     (ei.IdleEnd.HasValue ? ei.IdleEnd.Value : now) <= SAPPeriodStart
                     ||
                     ei.IdleStart.Value >= SAPPeriodEnd
                     &&
                     (ei.IdleEnd.HasValue ? ei.IdleEnd.Value : now) >= SAPPeriodEnd
                )
                && String.IsNullOrEmpty(ei.SAPOrderID)
            ).ToList();
            /*fitPeriods = fitPeriods.FindAll(p =>
              {
                  var isStartesBeforePeriod = p.IdleStart.Value <= SAPPeriodStart;
                  var isEndedBeforePeriod = (p.IdleEnd.HasValue ? p.IdleEnd.Value : now) <= SAPPeriodStart;
                  var isStartedAfterPeriod = p.IdleStart.Value >= SAPPeriodEnd;
                  var isEndedAfterPeriod = (p.IdleEnd.HasValue ? p.IdleEnd.Value : now) >= SAPPeriodEnd;
                  return !(isStartesBeforePeriod && isEndedBeforePeriod || isStartedAfterPeriod && isEndedAfterPeriod);
              }
            );
            fitPeriods = new List<EquipmentIdle>();*/
            log?.Add($"Простои для {SAPOrderID}:\r\n" + String.Join("\r\n", fitPeriods.Select(p => $"с {p.IdleStart:dd-MM-yyyy HH:mm:ss} по {p.IdleEnd:dd-MM-yyyy HH:mm:ss}; Номер заказа: {p.SAPOrderID}; Причина:{p.MalfunctionReasonTypeID},{p.MalfunctionReasonProfileID},{p.MalfunctionReasonNodeID},{p.MalfunctionReasonElementID},{p.MalfunctionReasonMalfunctionTextID}; Текст:{p.MalfunctionReasonMalfunctionTextComment}")));

            /*  (DbFunctions.CreateTime(ei.IdleStart.Value.Hour, ei.IdleStart.Value.Minute, ei.IdleStart.Value.Second) <= starttime && starttime <= DbFunctions.CreateTime(ei.IdleEnd.Value.Hour, ei.IdleEnd.Value.Minute, ei.IdleEnd.Value.Second)) ||
              (DbFunctions.CreateTime(ei.IdleStart.Value.Hour, ei.IdleStart.Value.Minute, ei.IdleStart.Value.Second) <= endtime && endtime <= DbFunctions.CreateTime(ei.IdleEnd.Value.Hour, ei.IdleEnd.Value.Minute, ei.IdleEnd.Value.Second))
           */
            if (fitPeriods.Any(p => p.SAPOrderID == SAPOrderID)) return;
            if (fitPeriods.Count == 1)
            {
                if (SAPPeriodStart < fitPeriods[0].IdleStart.Value) SAPPeriodStart = fitPeriods[0].IdleStart.Value;
                if (fitPeriods[0].IdleEnd.HasValue && SAPPeriodEnd > fitPeriods[0].IdleEnd.Value) SAPPeriodEnd = fitPeriods[0].IdleEnd.Value;
                var start = fitPeriods[0].IdleStart.Value;
                var startMaintenance = SAPPeriodStart;
                var endMaintenance = SAPPeriodEnd;
                var end = fitPeriods[0].IdleEnd;
                log?.Add($"Деление простоя:");
                var isIdleOpen = fitPeriods[0].IsOpenIdle;

                if (start < startMaintenance)
                {
                    var BeforeSAP = new EquipmentIdle();
                    BeforeSAP.ShiftStart = shift.ShiftDate;
                    BeforeSAP.IsNightShift = shift.IsNight;
                    BeforeSAP.IdleStart = start;
                    BeforeSAP.IdleEnd = startMaintenance;
                    BeforeSAP.MalfunctionReasonTypeID = fitPeriods[0].MalfunctionReasonTypeID;
                    BeforeSAP.MalfunctionReasonProfileID = fitPeriods[0].MalfunctionReasonProfileID;
                    BeforeSAP.MalfunctionReasonNodeID = fitPeriods[0].MalfunctionReasonNodeID;
                    BeforeSAP.MalfunctionReasonElementID = fitPeriods[0].MalfunctionReasonElementID;
                    BeforeSAP.MalfunctionReasonMalfunctionTextID = fitPeriods[0].MalfunctionReasonMalfunctionTextID;
                    BeforeSAP.MalfunctionReasonMalfunctionTextComment = fitPeriods[0].MalfunctionReasonMalfunctionTextComment;
                    BeforeSAP.EquipmentNumber = EquipmentNumber;
                    EquipmentIdles.Add(BeforeSAP);
                    log?.Add($"Перед ППР: { start:dd-MM-yyyy HH:mm:ss} по {startMaintenance:dd-MM-yyyy HH:mm:ss}; Причина:{BeforeSAP.MalfunctionReasonTypeID},{BeforeSAP.MalfunctionReasonProfileID},{BeforeSAP.MalfunctionReasonNodeID},{BeforeSAP.MalfunctionReasonElementID},{BeforeSAP.MalfunctionReasonMalfunctionTextID}; Текст:{BeforeSAP.MalfunctionReasonMalfunctionTextComment}");

                }

                {
                    fitPeriods[0].ShiftStart = shift.ShiftDate;
                    fitPeriods[0].IsNightShift = shift.IsNight;
                    fitPeriods[0].IdleStart = startMaintenance;
                    fitPeriods[0].IdleEnd = endMaintenance;


                    Tuple<int?, int?, int?, int?, int?> Reason;

                    if (SAPStrategy != null && Strategy2TextID.ContainsKey(SAPStrategy))
                    {
                        Reason = Strategy2TextID[SAPStrategy];
                    }
                    else
                    {
                        Reason = new Tuple<int?, int?, int?, int?, int?>(null, null, null, null, null);
                    }


                    fitPeriods[0].MalfunctionReasonTypeID = Reason.Item1;
                    fitPeriods[0].MalfunctionReasonProfileID = Reason.Item2;
                    fitPeriods[0].MalfunctionReasonNodeID = Reason.Item3;
                    fitPeriods[0].MalfunctionReasonElementID = Reason.Item4;
                    fitPeriods[0].MalfunctionReasonMalfunctionTextID = Reason.Item5;
                    fitPeriods[0].MalfunctionReasonMalfunctionTextComment = null;
                    fitPeriods[0].SAPOrderID = SAPOrderID;
                    log?.Add($"ППР: { startMaintenance:dd-MM-yyyy HH:mm:ss} по {endMaintenance:dd-MM-yyyy HH:mm:ss} Причина:{Reason.Item1},{Reason.Item2},{Reason.Item3},{Reason.Item4},{Reason.Item5}");

                }

                //if (end.HasValue && end > endMaintenance ||!end.HasValue)
                //            ||
                //            \/
                if (!end.HasValue || end > endMaintenance)
                {
                    var AfterSAP = new EquipmentIdle();
                    AfterSAP.ShiftStart = shift.ShiftDate;
                    AfterSAP.IsNightShift = shift.IsNight;
                    AfterSAP.IdleStart = endMaintenance;
                    AfterSAP.IdleEnd = end;
                    AfterSAP.MalfunctionReasonTypeID = 4;
                    AfterSAP.EquipmentNumber = EquipmentNumber;
                    EquipmentIdles.Add(AfterSAP);
                    log?.Add($"После  ППР: { endMaintenance:dd-MM-yyyy HH:mm:ss} по {end:dd-MM-yyyy HH:mm:ss};");
                    fitPeriods[0].IsOpenIdle = false;
                    AfterSAP.IsOpenIdle = isIdleOpen;
                }
                else
                {
                    fitPeriods[0].IsOpenIdle = isIdleOpen;
                }
                SaveChanges();
            }
        }

        public void SetMachineStatus(int MachineNumber, MachineStatus newStatus, Log log, DateTime? Time = null)
        {
            DateTime curTime;
            if (Time.HasValue)
                curTime = Time.Value;
            else
                curTime = DateTime.Now;
            var shift = new Shift();
            var idlerecord = EquipmentIdles.Where(ei => ei.EquipmentNumber == MachineNumber && ei.IsOpenIdle && ei.ShiftStart == shift.ShiftDate && ei.IsNightShift == shift.IsNight).ToList();
            log?.Add($"Изменение статуса машины {MachineNumber} на {newStatus}");
            switch (newStatus)
            {
                case MachineStatus.Idle:
                    {
                        if (idlerecord.Count > 0)
                        {
                        }
                        else
                        {
                            var newrecord = new EquipmentIdle();
                            newrecord.ShiftStart = shift.ShiftDate;
                            newrecord.IsNightShift = shift.IsNight;
                            newrecord.EquipmentNumber = MachineNumber;
                            newrecord.IsOpenIdle = true;
                            newrecord.IdleStart = curTime;
                            if (!IsPlanOrdersExist(MachineNumber, shift.ShiftDate))
                            {
                                var machineblocking = EquipmentBlockings.Where(eb => eb.EquipmentNumber == MachineNumber).FirstOrDefault();
                                if (machineblocking != null)
                                {
                                    newrecord.MalfunctionReasonTypeID = machineblocking.MalfunctionReasonTypeID;
                                    newrecord.MalfunctionReasonProfileID = machineblocking.MalfunctionReasonProfileID;
                                    newrecord.MalfunctionReasonNodeID = machineblocking.MalfunctionReasonNodeID;
                                    newrecord.MalfunctionReasonElementID = machineblocking.MalfunctionReasonElementID;
                                    newrecord.MalfunctionReasonMalfunctionTextID = machineblocking.MalfunctionReasonMalfunctionTextID;
                                    newrecord.MalfunctionReasonMalfunctionTextComment = machineblocking.MalfunctionReasonMalfunctionTextComment;

                                }
                                else
                                {
                                    newrecord.MalfunctionReasonTypeID = 2;
                                }
                            }
                            EquipmentIdles.Add(newrecord);
                            SaveChanges();
                        }
                        break;
                    }
                case MachineStatus.Working:
                    {
                        if (idlerecord.Count > 0)
                        {
                            var recordsorderes = idlerecord.OrderBy(r => r.ShiftStart).ThenBy(r1 => r1.IsNightShift).ThenBy(r2 => r2.IdleStart).ToList();

                            var lstrecord = recordsorderes.Last();
                            lstrecord.IdleEnd = curTime;// DateTime.Now;
                            lstrecord.IsOpenIdle = false;
                            if (lstrecord.IdleDuration.TotalMinutes < 15)
                            {
                                EquipmentIdles.Remove(lstrecord);
                            }
                            SaveChanges();
                        }
                        break;
                    }
            }
            SaveChanges();
        }
        public bool GetIsMachineIdle(int MachineNumber)
        {
            var shift = new Shift();
            var idlerecord = EquipmentIdles.Where(ei => ei.EquipmentNumber == MachineNumber && ei.IsOpenIdle && ei.ShiftStart == shift.ShiftDate && ei.IsNightShift == shift.IsNight).ToList();
            return idlerecord.Count != 0;
        }
        public void RefreshContext()
        {
            try
            {
                var undetachedEntriesCopy = this.ChangeTracker.Entries()
                    .Where(e => e.State != EntityState.Detached)
                    .ToList();

                foreach (var entry in undetachedEntriesCopy)
                    entry.State = EntityState.Detached;
            }
            catch { }
        }

        /*public static List<PlanOrder> GetPlanOrders(int MachineNumber, DateTime ShiftDate)
        {
            var connectionString = "";
            var sqlconnection = new SqlConnection(connectionString);
            var command = new SqlCommand("select petLine, startDate, endDate from planOrders where petLine=@machine AND @date BETWEEN startDate AND ADDDATE(day,endDate,1)",sqlconnection);
            command.Parameters.AddWithValue("@date", ShiftDate);
            command.Parameters.AddWithValue("@machine", "PETLINE"+MachineNumber.ToString("D2"));
            var dr = command.ExecuteReader();
            while (dr.Read())
            {
            }
        }*/
        public static bool IsPlanOrdersExist(int MachineNumber, DateTime ShiftDate)
        {
            var sqlconnection = new SqlConnection(connectionStringPlanning);
            var command = new SqlCommand("select count(*) from planOrders where petLine=@machine AND @date BETWEEN startDate AND endDate", sqlconnection);
            command.Parameters.AddWithValue("@date", ShiftDate);
            command.Parameters.AddWithValue("@machine", "PETLINE" + MachineNumber.ToString("D2"));
            sqlconnection.Open();
            var dr = command.ExecuteReader();
            int counter = 0;
            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    counter = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);
                }
            }
            sqlconnection.Close();
            return counter > 0;
        }
        public static List<TimeOrder> GetCycles(DateTime dt)
        {
            List<TimeOrder> L = new List<TimeOrder>();
            //string connectionString = "Server=192.168.0.12;Database=PetPro;Password=DbSyS@dm1n;User ID=sa";
            using (SqlConnection connection = new SqlConnection(connectionStringPetPro))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        //command.CommandText = $"DECLARE @dnt DateTime SET @dnt = (SELECT[date] FROM[PetPro].[dbo].[Time_Of_Changing_Orders] WHERE MachineN = {N}) SELECT[Date],Cycle_Time FROM[PetPro].[dbo].[Cycle_Photometer] WHERE[Date] > @dnt AND MachineNumber = {N} ORDER BY[Date]";
                        command.CommandText = @"select [MachineNumber],Date,Cycle_time from Cycle_Photometer where [Date]>=@dt";
                        command.Connection = connection;
                        command.Parameters.AddWithValue("@dt", dt);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                L.Add(new TimeOrder(reader.GetInt32(0), reader.GetDateTime(1), (reader.IsDBNull(2) ? Double.NaN : reader.GetDouble(2))));
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return L;



        }
        public static List<int> GetMachinesList()
        {
            var L = new List<int>();
            //string connectionString = "Server=192.168.0.12;Database=PetPro;Password=DbSyS@dm1n;User ID=sa";
            using (SqlConnection connection = new SqlConnection(connectionStringPetPro))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        //command.CommandText = $"DECLARE @dnt DateTime SET @dnt = (SELECT[date] FROM[PetPro].[dbo].[Time_Of_Changing_Orders] WHERE MachineN = {N}) SELECT[Date],Cycle_Time FROM[PetPro].[dbo].[Cycle_Photometer] WHERE[Date] > @dnt AND MachineNumber = {N} ORDER BY[Date]";
                        command.CommandText = @"select [MachineNumber] from Cycle_Photometer group by [MachineNumber]";
                        command.Connection = connection;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    L.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return L;



        }


        public class RecordCannotBeDividedException : Exception { }

        public enum MachineStatus
        {
            Idle,
            Working
        }

        public class DBInitializer : CreateDatabaseIfNotExists<DB>
        {
            #region SQLReasonsFill

            string[] sqls = new string[] { @"
SET IDENTITY_INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ON 
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (1, 3, 1, 1, 1, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (3, 3, 1, 1, 1, N'Вибрации или пост.шум')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (4, 3, 1, 1, 2, N'Основной')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (5, 3, 1, 1, 2, N'Малый')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (6, 3, 1, 1, 2, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (7, 3, 1, 1, 3, N'Нет зарядки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (8, 3, 1, 1, 3, N'Заправка АЗОТОМ')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (9, 3, 1, 1, 3, N'Поршень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (10, 3, 1, 1, 3, N'Уплотнения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (11, 3, 1, 1, 4, N'Узла смыкания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (12, 3, 1, 1, 4, N'Выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (13, 3, 1, 1, 4, N'Шиберной заслонки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (14, 3, 1, 1, 4, N'Перемещения ротомата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (15, 3, 1, 1, 4, N'Цилиндр впрыска')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (16, 3, 1, 1, 4, N'Цилиндр перегонки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (17, 3, 1, 1, 4, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (18, 3, 1, 1, 5, N'Выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (19, 3, 1, 1, 5, N'Гидроцилиндра узла смыкания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (20, 3, 1, 1, 5, N'Осн. распред. - распред. узла смыкания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (21, 3, 1, 1, 5, N'Осн. распред. - распред. выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (22, 3, 1, 1, 5, N'Осн. насос - гидроакумулятор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (23, 3, 1, 1, 5, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (24, 3, 1, 1, 6, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (25, 3, 1, 1, 6, N'Не вращает шнек')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (26, 3, 1, 1, 6, N'Вибрации или пост.шум')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (27, 3, 1, 1, 7, N'Распред. узла смыкания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (28, 3, 1, 1, 7, N'Распред. выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (29, 3, 1, 1, 7, N'Распред. шиберной заслонки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (30, 3, 1, 1, 7, N'Распред. цилиндров перемещения ротомата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (31, 3, 1, 1, 7, N'Распред. цилиндра впрыска')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (32, 3, 1, 1, 7, N'Распред. цилиндра перегонки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (33, 3, 1, 1, 7, N'Распред. гидроакумулятора')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (34, 3, 1, 1, 7, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (35, 3, 1, 1, 8, N'Всасывающие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (36, 3, 1, 1, 8, N'Напорные')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (37, 3, 1, 1, 8, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (38, 3, 1, 1, 9, N'Перегрев масла')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (39, 3, 1, 1, 9, N'Уровень масла')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (40, 3, 1, 1, 9, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (41, 3, 1, 1, 10, N'Не герметичен')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (42, 3, 1, 1, 10, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (43, 3, 1, 2, 11, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (44, 3, 1, 2, 11, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (45, 3, 1, 2, 12, N'На входе в систему машины')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (46, 3, 1, 2, 12, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (47, 3, 1, 2, 13, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (48, 3, 1, 2, 14, N'Пневмос-мы управл. Иглами')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (49, 3, 1, 2, 14, N'Пневмосистемы подачи воздуха на выталкиватель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (50, 3, 1, 2, 14, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (51, 3, 1, 2, 15, N'Управл. иглами гор. каналов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (52, 3, 1, 2, 15, N'Подачи воздуха на выталкиватель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (53, 3, 1, 2, 15, N'Управления клапаном сис.охл. гидравлики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (54, 3, 1, 2, 15, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (55, 3, 1, 2, 16, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (56, 3, 1, 2, 16, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (57, 3, 1, 3, 17, N'На входе в систему машины')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (58, 3, 1, 3, 17, N'На колекторах охлаждения оснастки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (59, 3, 1, 3, 17, N'На ответвлениях к переферийным потребителям')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (60, 3, 1, 3, 17, N'На входе в систему охлаждения гидравлики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (61, 3, 1, 3, 17, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (62, 3, 1, 3, 18, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (63, 3, 1, 3, 18, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (64, 3, 1, 3, 19, N'Системы охлаждения масла')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (65, 3, 1, 3, 19, N'В шкафах управления')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (66, 3, 1, 3, 19, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (67, 3, 1, 3, 20, N'Охлаждения гидравлики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (68, 3, 1, 3, 21, N'Быстроразёмные соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (69, 3, 1, 3, 21, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (70, 3, 1, 3, 21, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (71, 3, 1, 4, 22, N'Двигатель-редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (72, 3, 1, 4, 22, N'Шестерни привода гаек')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (73, 3, 1, 4, 22, N'Гайки и прижимные фланцы')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (74, 3, 1, 4, 22, N'Опорные подшипники')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (75, 3, 1, 4, 22, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (76, 3, 1, 4, 23, N'Регулируемая плита')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (77, 3, 1, 4, 23, N'Система рычагов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (78, 3, 1, 4, 23, N'Плита выталк-ля с гидроц-ми')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (79, 3, 1, 4, 23, N'Подвижная плита')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (80, 3, 1, 4, 23, N'Неподвижная плита')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (81, 3, 1, 4, 23, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (84, 3, 1, 4, 24, N'Сопло')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (85, 3, 1, 4, 24, N'Узел шиберной заслонки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (86, 3, 1, 4, 24, N'Обводная головка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (87, 3, 1, 4, 24, N'Инжекц. цилиндр и поршень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (88, 3, 1, 4, 24, N'Материаль. цилиндр и шнек')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (89, 3, 1, 4, 24, N'Привод шнека')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (90, 3, 1, 4, 24, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (91, 3, 1, 4, 24, N'Дозировка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (92, 3, 1, 5, 25, N'Длительность нагрева превышина')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (93, 3, 1, 5, 25, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (94, 3, 1, 5, 25, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (95, 3, 1, 5, 26, N'Длительность нагрева превышина')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (96, 3, 1, 5, 26, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (97, 3, 1, 5, 26, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (98, 3, 1, 5, 27, N'Длительность нагрева превышина')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (99, 3, 1, 5, 27, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (100, 3, 1, 5, 27, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (101, 3, 1, 5, 28, N'Длительность нагрева превышина')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (102, 3, 1, 5, 28, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (103, 3, 1, 5, 28, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (104, 3, 1, 5, 29, N'Длительность нагрева превышина')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (105, 3, 1, 5, 29, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (106, 3, 1, 5, 29, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (107, 3, 1, 6, 30, N'Ультрозвуковая линейка формы')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (108, 3, 1, 6, 30, N'Ультрозвуковая линейка выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (109, 3, 1, 6, 30, N'Ультрозвуковая линейка инжекционного цилиндра')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (110, 3, 1, 6, 30, N'Ультрозвуковая линейка шнэка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (111, 3, 1, 6, 30, N'Индук. линейка цилиндра впрыска')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (112, 3, 1, 6, 30, N'Индуктивная линейка шнэка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (113, 3, 1, 6, 31, N'Индук. положения формы')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (114, 3, 1, 6, 31, N'Индук. положения выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (115, 3, 1, 6, 31, N'Индук. положения ротомата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (116, 3, 1, 6, 31, N'Индук. положения шнэка на HP')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (117, 3, 1, 6, 31, N'Индук. контр. предохр. муфты привода шнэка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (118, 3, 1, 6, 31, N'Индук. контр. поршня гидроаккумулятора на HP.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (119, 3, 1, 6, 31, N'Индук. контр. положения шиберной заслонки.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (120, 3, 1, 6, 31, N'Индук. контр. положения рычагов, узла смыкания.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (121, 3, 1, 6, 31, N'Индук. контр. положения шестерни центр. рег. сис.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (122, 3, 1, 6, 31, N'Индук. контр. золотника в защит. контуре гидр. на HP')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (123, 3, 1, 6, 31, N'Индук. контр. положения стопора,защитной траверсы')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (124, 3, 1, 6, 31, N'Датчики уровня масла в емкостях.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (125, 3, 1, 6, 31, N'Датчики протока воды')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (126, 3, 1, 6, 31, N'Датчики давления 250 bar')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (127, 3, 1, 6, 31, N'Датчики давления 400 bar')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (128, 3, 1, 6, 31, N'Датчик давления системы центральной смазки.')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (129, 3, 1, 7, 33, N'Неисправен или замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (130, 3, 1, 7, 33, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (131, 3, 1, 7, 34, N'Неисправен или замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (132, 3, 1, 7, 34, N'Чистка, продувка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (133, 3, 1, 7, 34, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (134, 3, 1, 7, 35, N'Неисправен или замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (135, 3, 1, 7, 35, N'Чистка, продувка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (136, 3, 1, 7, 35, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (137, 3, 1, 8, 36, N'Замена или ремонт')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (138, 3, 1, 8, 36, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (139, 3, 1, 8, 37, N'Неисправен или замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (140, 3, 1, 8, 37, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (141, 3, 1, 8, 37, N'Сбой параметров')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (142, 3, 1, 8, 38, N'Неисправен или замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (143, 3, 1, 8, 38, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (146, 3, 1, 8, 39, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (148, 3, 1, 8, 40, N'Разрыв шланга')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (149, 3, 1, 8, 40, N'Негерметичны соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (150, 3, 1, 8, 40, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (151, 3, 1, 9, 41, N'Случайно')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (152, 3, 1, 9, 41, N'По причине аварии')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (153, 3, 1, 9, 41, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (154, 3, 1, 9, 42, N'Не срабатывают замки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (155, 3, 1, 9, 42, N'Концевики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (156, 3, 1, 9, 42, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (157, 3, 1, 9, 43, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (160, 3, 1, 12, 44, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (161, 3, 1, 12, 44, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (162, 3, 1, 12, 45, N'Перегорели встаки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (163, 3, 1, 12, 45, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (164, 3, 2, 13, 46, N'Датчики кантроля положения робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (165, 3, 2, 13, 46, N'Вакумный датчик')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (166, 3, 2, 13, 46, N'Оптические датчики и зеркала')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (167, 3, 2, 13, 46, N'Датчик контроля переднего положения робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (168, 3, 2, 13, 46, N'Датчики контроля положения блокирующего уст-ва')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (169, 3, 2, 13, 46, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (170, 3, 2, 13, 47, N'Распределитель упр. вакумными клапонами')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (171, 3, 2, 13, 47, N'Распределитель упр. выталкивателем')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (172, 3, 2, 13, 47, N'Распределитель упр. преремещением робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (173, 3, 2, 13, 47, N'Распределитель блокирующего уст-ва')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (174, 3, 2, 13, 47, N'Пневмоцилиндры выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (175, 3, 2, 13, 47, N'Пневмоцилиндры вакумных клапанов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (176, 3, 2, 13, 47, N'Пневмоцилиндр блокирующего уст-ва')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (177, 3, 2, 13, 47, N'Пневмоцилиндр перемещения робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (178, 3, 2, 13, 47, N'Дроссели и обратные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (179, 3, 2, 13, 47, N'Шланги, соединения и трубки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (180, 3, 2, 13, 47, N'Влагомаслоотделитель и редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (181, 3, 2, 13, 47, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (182, 3, 2, 13, 48, N'Вакумный насос')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (183, 3, 2, 13, 48, N'Трубопроводы и шланги')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (184, 3, 2, 13, 48, N'Ваккумные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (185, 3, 2, 13, 48, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (186, 3, 2, 13, 49, N'Недостаточное кол-во смазки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (187, 3, 2, 13, 49, N'Разрыв трубопроводов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (188, 3, 2, 13, 49, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (189, 3, 2, 13, 50, N'Двигатель-редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (190, 3, 2, 13, 50, N'Напраляющие ролики, ведущий шкив ремня')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (191, 3, 2, 13, 50, N'Ремень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (192, 3, 2, 13, 50, N'Подшипники линейного перемещения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (193, 3, 2, 13, 50, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (194, 3, 2, 13, 51, N'Осталась преформа')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (195, 3, 2, 13, 51, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (196, 3, 2, 14, 52, N'Датчики положения охл.станцый (блока)')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (197, 3, 2, 14, 52, N'Вакумные датчики охл.блока')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (198, 3, 2, 14, 52, N'Оптические датчики, зеркала')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (199, 3, 2, 14, 52, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (200, 3, 2, 14, 53, N'Распределитель упр. вакумными клапонами')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (201, 3, 2, 14, 53, N'Распределители упр.охл.станциями')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (202, 3, 2, 14, 53, N'Пневмоцилиндры охл.станций')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (203, 3, 2, 14, 53, N'Клапаны подачи воздуха сброса')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (204, 3, 2, 14, 53, N'Влагомаслоотделитель и редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (205, 3, 2, 14, 53, N'Дроссели и обратные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (206, 3, 2, 14, 53, N'Шланги, соединения и трубки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (207, 3, 2, 14, 53, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (208, 3, 2, 14, 54, N'Вакумный насос')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (209, 3, 2, 14, 54, N'Трубопроводы и шланги')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (210, 3, 2, 14, 54, N'Вакумные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (211, 3, 2, 14, 54, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (212, 3, 2, 14, 55, N'Двигатель-редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (213, 3, 2, 14, 55, N'Напраляющие ролики, ведущий шкив ремня')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (214, 3, 2, 14, 55, N'Ремень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (215, 3, 2, 14, 55, N'Подшипники линейного перемещения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (216, 3, 2, 14, 55, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (217, 3, 2, 14, 56, N'Осталась преформа(блок не опорожнен)')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (218, 3, 2, 14, 56, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (219, 3, 2, 15, 57, N'Датчики положения перед.захвата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (220, 3, 2, 15, 57, N'Вакумный датчик')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (221, 3, 2, 15, 57, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (222, 3, 2, 15, 58, N'Пневмоцилиндры перед.захвата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (223, 3, 2, 15, 58, N'Распределители перед.захвата')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (224, 3, 2, 15, 58, N'Клапаны подачи воздуха сброса (продува)')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (225, 3, 2, 15, 58, N'Редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (226, 3, 2, 15, 58, N'Дроссели и обратные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (227, 3, 2, 15, 58, N'Шланги, соединения и трубки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (228, 3, 2, 15, 58, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (229, 3, 2, 15, 59, N'Трубопроводы и шланги')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (230, 3, 2, 15, 59, N'Вакумные клапаны')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (231, 3, 2, 15, 59, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (232, 3, 2, 15, 60, N'Двигатель-редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (233, 3, 2, 15, 60, N'Напраляющие ролики, ведущий шкив ремня')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (234, 3, 2, 15, 60, N'Ремень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (235, 3, 2, 15, 60, N'Подшипники линейного перемещения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (236, 3, 2, 15, 60, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (237, 3, 2, 16, 61, N'Вентиль подачи верт. робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (238, 3, 2, 16, 61, N'Вентиль подачи гориз. Робота')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (239, 3, 2, 16, 61, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (240, 3, 2, 16, 62, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (241, 3, 2, 16, 62, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (242, 3, 2, 16, 63, N'Клапаны подачи')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (243, 3, 2, 16, 63, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (244, 3, 2, 16, 64, N'Чистка сетки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (245, 3, 2, 16, 64, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (246, 3, 2, 16, 65, N'Быстроразёмные соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (247, 3, 2, 16, 65, N'Утечка или разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (248, 3, 2, 16, 65, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (249, 3, 2, 16, 66, N'Нет подачи воды')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (250, 3, 2, 16, 66, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (251, 3, 2, 17, 67, N'Разрыв транспортерной ленты')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (252, 3, 2, 17, 67, N'Заклинивание')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (253, 3, 2, 17, 67, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (254, 3, 2, 17, 68, N'Разрыв транспортерной ленты')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (255, 3, 2, 17, 68, N'Заклинивание')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (256, 3, 2, 17, 68, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (257, 3, 2, 17, 69, N'Разрыв транспортерной ленты')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (258, 3, 2, 17, 69, N'Заклинивание')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (259, 3, 2, 17, 69, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (260, 3, 2, 18, 70, N'Случайно')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (261, 3, 2, 18, 70, N'По причине аварии')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (262, 3, 2, 18, 70, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (263, 3, 2, 18, 71, N'Не срабатывают замки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (264, 3, 2, 18, 71, N'Концевики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (265, 3, 2, 18, 71, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (266, 3, 2, 18, 72, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (267, 3, 2, 19, 73, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (268, 3, 2, 19, 73, N'Сбой в работе электроники')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (269, 3, 2, 19, 73, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (270, 3, 2, 20, 74, N'Теряет позиции')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (271, 3, 2, 20, 74, N'Не точное позиционирование')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (272, 3, 2, 20, 74, N'Потертости, царапины на преформе')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (273, 3, 2, 20, 74, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (274, 3, 3, 21, 75, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (275, 3, 3, 21, 75, N'Сбои програмного обеспечения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (276, 3, 3, 21, 75, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (277, 3, 3, 21, 76, N'Сгорел нагреватель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (278, 3, 3, 21, 76, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (279, 3, 3, 21, 77, N'Замена неисправного нагнетателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (280, 3, 3, 21, 77, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (281, 3, 3, 21, 78, N'Забит фильтр')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (282, 3, 3, 21, 78, N'Разрыв воздуховода')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (283, 3, 3, 21, 78, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (284, 3, 3, 21, 79, N'Некоректная работа')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (285, 3, 3, 21, 79, N'Не плотно закрываются')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (286, 3, 3, 21, 79, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (287, 3, 3, 21, 80, N'Пневмоцилиндры')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (288, 3, 3, 21, 80, N'Распределители')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (289, 3, 3, 21, 80, N'Влагомаслоотделитель редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (290, 3, 3, 21, 80, N'Шланги соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (291, 3, 3, 22, 81, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (292, 3, 3, 22, 81, N'Сбой в работе электрики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (293, 3, 3, 22, 81, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (294, 3, 3, 22, 82, N'Сгорел нагреватель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (295, 3, 3, 22, 82, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (296, 3, 3, 22, 83, N'Замена неисправного нагнетателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (297, 3, 3, 22, 83, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (298, 3, 3, 22, 84, N'Забит фильтр')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (299, 3, 3, 22, 84, N'Разрыв воздуховода')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (300, 3, 3, 22, 84, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (301, 3, 3, 22, 85, N'Некоректная работа')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (302, 3, 3, 22, 85, N'Не плотно закрыватся')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (303, 3, 3, 22, 85, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (304, 3, 3, 22, 86, N'Пневмоцилиндр клапана')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (305, 3, 3, 22, 86, N'Распределитель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (306, 3, 3, 22, 86, N'Влагомаслоотделитель редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (307, 3, 3, 22, 86, N'Шланги соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (308, 3, 3, 23, 87, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (309, 3, 3, 23, 87, N'Сбой в работе электрики')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (310, 3, 3, 23, 87, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (311, 3, 3, 23, 88, N'Сгорел нагреватель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (312, 3, 3, 23, 88, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (313, 3, 3, 23, 89, N'Замена неисправного нагнетателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (314, 3, 3, 23, 89, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (315, 3, 3, 23, 90, N'Забит фильтр')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (316, 3, 3, 23, 90, N'Разрыв воздуховода')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (317, 3, 3, 23, 90, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (318, 3, 3, 23, 91, N'Некоректная работа')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (319, 3, 3, 23, 91, N'Не плотно закрыватся')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (320, 3, 3, 23, 91, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (321, 3, 3, 23, 92, N'Пневмоцилиндр клапана')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (322, 3, 3, 23, 92, N'Распределитель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (323, 3, 3, 23, 92, N'Влагомаслоотделитель редуктор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (324, 3, 3, 23, 92, N'Шланги соединения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (325, 3, 3, 24, 93, N'Замена неисправного насоса')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (326, 3, 3, 24, 93, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (327, 3, 3, 24, 94, N'Забит фильтр')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (328, 3, 3, 24, 94, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (329, 3, 3, 24, 95, N'Разрыв, не герметичность')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (330, 3, 3, 24, 95, N'Засорение')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (331, 3, 3, 24, 95, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (332, 3, 3, 24, 96, N'Клапан на фильтре отстойнике')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (333, 3, 3, 24, 96, N'Капан на колпаке загрузчика')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (334, 3, 3, 24, 96, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (335, 3, 3, 25, 97, N'Порван ремень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (336, 3, 3, 25, 97, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (337, 3, 3, 26, 98, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (338, 3, 3, 26, 98, N'Сбой в работе электрики(электроники)')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (339, 3, 3, 26, 98, N'Выбило атомат')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (340, 3, 3, 26, 98, N'Сгорели предохранители')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (341, 3, 3, 26, 98, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (342, 3, 3, 26, 99, N'Разбит вентилятор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (343, 3, 3, 26, 99, N'Порван ремень')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (344, 3, 3, 26, 99, N'Неисправен двигатель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (345, 3, 3, 26, 99, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (346, 3, 3, 26, 100, N'Низкое давл. в контуре №1')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (347, 3, 3, 26, 100, N'Низкое давл.в контуре №2')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (348, 3, 3, 26, 100, N'Высокое давл. в контуре №1')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (349, 3, 3, 26, 100, N'Высокое давл. в контуре №2')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (350, 3, 3, 26, 100, N'Заклинил компрессор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (351, 3, 3, 26, 100, N'Сгорел двигатель компрессора')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (352, 3, 3, 26, 100, N'Низкое давление масла')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (353, 3, 3, 26, 100, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (354, 3, 3, 26, 101, N'Сильное загрезнение')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (355, 3, 3, 26, 101, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (356, 3, 3, 26, 102, N'Опастность замерзания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (357, 3, 3, 26, 102, N'Слабый проток воды через испаритель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (358, 3, 3, 26, 102, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (359, 3, 3, 27, 103, N'Перегрев')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (360, 3, 3, 27, 103, N'Сбой в работе электрики(электроники)')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (361, 3, 3, 27, 103, N'Выбило атомат')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (362, 3, 3, 27, 103, N'Сгорели предохранители')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (363, 3, 3, 27, 103, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (364, 3, 3, 27, 104, N'Течь, не герметичность')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (365, 3, 3, 27, 104, N'Уровень воды слишком мал')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (366, 3, 3, 27, 104, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (367, 3, 3, 27, 105, N'Насос ""форма""')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (368, 3, 3, 27, 105, N'Насос ""гидравлика""')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (369, 3, 3, 27, 105, N'Насос ""холодильник""')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (370, 3, 3, 27, 105, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (371, 3, 3, 28, NULL, N'Сбои в работе компрессора')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (372, 3, 3, 28, NULL, N'Разрыв пневмо.магистрали')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (373, 3, 3, 28, NULL, N'Просадка давления')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (374, 3, 3, 28, NULL, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (375, 2, NULL, NULL, NULL, N'Нет заявки')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (376, 2, NULL, NULL, NULL, N'Нет расходных материалов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (377, 3, 5, NULL, NULL, N'Работы на подстанции')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (378, 3, 5, NULL, NULL, N'Внешние перебои в подаче напряжения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (379, 3, 5, NULL, NULL, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (380, 1, NULL, NULL, NULL, N'ППР + смена формы')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (381, 1, NULL, NULL, NULL, N'ППР + смена слайдов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (382, 1, NULL, NULL, NULL, N'ППР + смена пуансонов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (383, 1, NULL, NULL, NULL, N'ППР + чистка игл')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (384, 3, 1, 31, NULL, N'Сбои прог. обеспечения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (385, 3, 1, 32, NULL, N'Остановка без сообщения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (386, 3, 1, 1, 3, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (387, 3, 1, 3, 20, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (388, 3, 1, 6, 30, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (389, 3, 1, 6, 31, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (390, 3, 3, 21, 80, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (391, 3, 3, 22, 86, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (392, 3, 3, 23, 92, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (393, 3, 1, 12, 44, N'Сбой прогр. обеспечения')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (394, 1, NULL, NULL, NULL, N'еженедельное ТО')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (395, 3, 6, NULL, NULL, N'Человеческий фактор')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (398, 3, 1, 4, 22, N'Усилие смыкания')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (399, 3, 1, 33, NULL, N'Чистка слайдов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (400, 3, 1, 33, NULL, N'Замена уплотнений')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (401, 3, 1, 33, NULL, N'Замена трубок, шлангов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (402, 3, 1, 33, NULL, N'Чистка пуансонов, матриц')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (403, 3, 1, 33, NULL, N'Преформа под плит. выталкивателя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (404, 3, 1, 33, NULL, N'Длинный литник')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (405, 3, 1, 33, NULL, N'Нити, коронка')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (406, 3, 1, 33, NULL, N'Другие')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (407, 1, NULL, NULL, NULL, N'ежемесячное ТО')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (408, 1, NULL, NULL, NULL, N'ежегодное ТО')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (410, 4, NULL, NULL, NULL, N'Запуск после ППР')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (411, 5, 7, NULL, NULL, N'Чистка шнека хим. Гранулятом')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (412, 5, 7, NULL, NULL, N'Чистка шнека хол. Гранулятом')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (413, 5, 7, NULL, NULL, N'Прожиг шнека')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (414, 5, 17, NULL, NULL, N'Чистка премиксера')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (415, 5, 9, NULL, NULL, N'Загрузка материала в бункер')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (416, 5, 9, NULL, NULL, N'Предварительная сушка материала')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (417, 5, 9, NULL, NULL, N'Дополнительная сушка материала')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (418, 5, 17, NULL, NULL, N'Заклинило насос, замена')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (419, 5, 17, NULL, NULL, N'Промывка насоса')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (420, 5, 17, NULL, NULL, N'Чистка трубок')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (421, 5, 17, NULL, NULL, N'Замена красителя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (422, 5, 17, NULL, NULL, N'Переход на другой цвет')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (423, 5, 17, NULL, NULL, N'Нестабильное дозирование, замена насоса')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (424, 5, 7, NULL, NULL, N'Экспериментальное производство')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (425, 5, 17, NULL, NULL, N'Переход на другой краситель')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (426, 5, 9, NULL, NULL, N'Охлаждение материала')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (427, 1, NULL, NULL, NULL, N'ППР + смена пуансонов и слайдов')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (472, 3, 3, 34, NULL, N'Разрыв')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (473, 3, 3, 34, NULL, N'Просадка давления')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (474, 5, 24, NULL, NULL, N'Чистка шнека и полостей дозатора')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (475, 5, 24, NULL, NULL, N'Чистка фильтра загрузчика')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (476, 6, NULL, NULL, NULL, N'Нет гранулята')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (477, 6, NULL, NULL, NULL, N'Нет красителя')
INSERT [dbo].[MalfunctionReasonMalfunctionTexts] ([MalfunctionReasonMalfunctionTextID], [MalfunctionReasonTypeID], [MalfunctionReasonProfileID], [MalfunctionReasonNodeID], [MalfunctionReasonElementID], [MalfunctionReasonMalfunctionTextName]) VALUES (478, 6, NULL, NULL, NULL, N'Нет добавки')
SET IDENTITY_INSERT [dbo].[MalfunctionReasonMalfunctionTexts] OFF
", @"

SET IDENTITY_INSERT [dbo].[MalfunctionReasonElements] ON 
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (1, N'Двигатель', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (2, N'Насосы', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (3, N'Гидроаккумулятор', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (4, N'Гидроцилиндры', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (5, N'Шланги и трубопроводы', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (6, N'Гидромотор', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (7, N'Распределители и клапаны', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (8, N'Фильтры', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (9, N'Масляный бак', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (10, N'Теплообменник', 1)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (11, N'Трубопроводы шланги', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (12, N'Влагомаслоотделители', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (13, N'Вентили', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (14, N'Редуктора', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (15, N'Распределители', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (16, N'Соединения', 2)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (17, N'Вентили', 3)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (18, N'Трубопроводы шланги', 3)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (19, N'Клапаны', 3)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (20, N'Сетчатые фильтры', 3)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (21, N'Соединения', 3)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (22, N'Сис. регулир. узла смыкания', 4)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (23, N'Узел смыкания', 4)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (24, N'Узел впрыска (ротомат)', 4)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (25, N'Сопла', 5)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (26, N'Узел шиберной заслонки', 5)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (27, N'Обводной головки', 5)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (28, N'Инжекционного цилиндра', 5)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (29, N'Материального цилиндра', 5)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (30, N'Линейки', 6)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (31, N'Датчики', 6)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (33, N'Блок управления', 7)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (34, N'Блок смесителя', 7)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (35, N'Клапан-форсунка', 7)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (36, N'Премиксер', 8)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (37, N'Блок управления', 8)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (38, N'Мотор-редуктор', 8)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (39, N'Насос', 8)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (40, N'Шланги, соединения и трубки', 8)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (41, N'Нажата аварийная кнопка', 9)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (42, N'Двери', 9)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (43, N'Согласующие реле', 9)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (44, N'Шкафы управления', 12)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (45, N'Силовой шкаф', 12)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (46, N'Датчики', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (47, N'Пневматика', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (48, N'Вакумная система', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (49, N'Система смазки', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (50, N'Привод', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (51, N'Остановка', 13)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (52, N'Датчики', 14)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (53, N'Пневматика', 14)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (54, N'Вакуумная система', 14)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (55, N'Привод', 14)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (56, N'Остановка', 14)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (57, N'Датчики', 15)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (58, N'Пневматика', 15)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (59, N'Вакуумная система', 15)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (60, N'Привод', 15)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (61, N'Вентили', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (62, N'Трубопроводы шланги', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (63, N'Клапаны', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (64, N'Сетчатые фильтры', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (65, N'Соединения', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (66, N'Насос', 16)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (67, N'Горизонтальный конвер', 17)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (68, N'Наклонный конвеер', 17)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (69, N'Реверсивный конвеер', 17)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (70, N'Нажата аварийная кнопка', 18)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (71, N'Двери', 18)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (72, N'Согласующие реле', 18)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (73, N'Шкаф управления', 19)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (74, N'ЮСТИРОВКА РОБОТА', 20)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (75, N'Шкаф управления', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (76, N'Блок нагревателей', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (77, N'Нагнетатели', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (78, N'Воздуховоды, фильтры.', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (79, N'Клапаны распределения потоков', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (80, N'Пневматика', 21)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (81, N'Шкаф управления', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (82, N'Блок нагревателей', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (83, N'Нагнетатель', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (84, N'Воздуховоды, фильтры.', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (85, N'Клапан распределения потоков', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (86, N'Пневматика', 22)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (87, N'Шкаф управления', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (88, N'Блок нагревателей', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (89, N'Нагнетатель', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (90, N'Воздуховоды, фильтры.', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (91, N'Клапан распределения потоков', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (92, N'Пневматика', 23)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (93, N'Насос', 24)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (94, N'Фильтр отстойник', 24)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (95, N'Трубопроводы', 24)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (96, N'Клапаны', 24)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (97, N'Вентилятор', 25)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (98, N'Шкаф управления', 26)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (99, N'Вытяжные вентиляторы', 26)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (100, N'Компрессоры', 26)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (101, N'Конденсатор', 26)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (102, N'Испаритель', 26)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (103, N'Шкаф управления', 27)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (104, N'Емкость Трубопроводы', 27)
INSERT [dbo].[MalfunctionReasonElements] ([MalfunctionReasonElementID], [MalfunctionReasonElementName], [MalfunctionReasonNodeID]) VALUES (105, N'Насосы', 27)
SET IDENTITY_INSERT [dbo].[MalfunctionReasonElements] OFF
", @"

SET IDENTITY_INSERT [dbo].[MalfunctionReasonProfiles] ON 
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (1, N'Машина', 3)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (2, N'Робот', 3)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (3, N'Периферия', 3)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (5, N'Электроснабжение', 3)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (6, N'Человеческий фактор', 3)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (7, N'Машина', 5)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (9, N'Мотан', 5)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (17, N'Дозатор красителя', 5)
INSERT [dbo].[MalfunctionReasonProfiles] ([MalfunctionReasonProfileID], [MalfunctionReasonProfileName], [MalfunctionReasonTypeID]) VALUES (24, N'Дозатор добавок', 5)
SET IDENTITY_INSERT [dbo].[MalfunctionReasonProfiles] OFF
", @"

SET IDENTITY_INSERT [dbo].[MalfunctionReasonNodes] ON 
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (1, N'Гидравлическая система', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (2, N'Пневматическая система', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (3, N'Система охлаждения', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (4, N'Исполнительные элементы', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (5, N'Нагревательные элементы', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (6, N'Датчики и линейки', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (7, N'Система орошения', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (8, N'Система подачи красителя', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (9, N'Защитный контур', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (12, N'Шкафы', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (13, N'Вертикальный робот', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (14, N'Горизонтальный робот', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (15, N'Передающий захват', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (16, N'Система охлаждения', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (17, N'Конвееры', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (18, N'Защитный контур', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (19, N'Шкаф', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (20, N'Юстировка робота', 2)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (21, N'Мотан', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (22, N'Этабокс', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (23, N'Дополнительный этабокс', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (24, N'Вакумный загрузчик', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (25, N'Эйсбар', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (26, N'Холодильник', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (27, N'Насосная станция', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (28, N'Компрессоры', 3)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (31, N'Сбои прог. Обеспечения', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (32, N'Остановка без сообщения', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (33, N'Оснастка', 1)
INSERT [dbo].[MalfunctionReasonNodes] ([MalfunctionReasonNodeID], [MalfunctionReasonNodeName], [MalfunctionReasonProfileID]) VALUES (34, N'Пневмомагистраль', 3)
SET IDENTITY_INSERT [dbo].[MalfunctionReasonNodes] OFF
",  @"

SET IDENTITY_INSERT [dbo].[MalfunctionReasonTypes] ON 
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (1, N'ППР')
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (2, N'Отсутствие задания')
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (3, N'Технические причины')
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (4, N'Запуск после ППР')
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (5, N'Технологические причины')
INSERT [dbo].[MalfunctionReasonTypes] ([MalfunctionReasonTypeID], [MalfunctionReasonTypeName]) VALUES (6, N'Отсутствие расходных материалов')
SET IDENTITY_INSERT [dbo].[MalfunctionReasonTypes] OFF


",
                @"

SET IDENTITY_INSERT [dbo].[Operators] ON 
INSERT [dbo].[Operators] ([OperatorID], [ShiftNumber], [OperatorName]) VALUES (1, N'Смена №1', N'Оператор смены 1')
INSERT [dbo].[Operators] ([OperatorID], [ShiftNumber], [OperatorName]) VALUES (2, N'Смена №2', N'Оператор смены 2')
INSERT [dbo].[Operators] ([OperatorID], [ShiftNumber], [OperatorName]) VALUES (3, N'Смена №3', N'Оператор смены 3')
INSERT [dbo].[Operators] ([OperatorID], [ShiftNumber], [OperatorName]) VALUES (4, N'Смена №4', N'Оператор смены 4')
INSERT [dbo].[Operators] ([OperatorID], [ShiftNumber], [OperatorName]) VALUES (5, N'Подмена', N'Подмена')
SET IDENTITY_INSERT [dbo].[Operators] OFF


", };//.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            #endregion
            protected override void Seed(DB context)
            {
                base.Seed(context);
                context.Users.Add(new User() { UserName = "Admin", PasswordHash = DB.Hash("Admin"), Rights = UserRight.Administrator });
                context.Users.Add(new User() { UserName = "E_Simonov", PasswordHash = DB.Hash("surface"), AutoLogonComputerNames = "PRG2", Rights = UserRight.Administrator });
                /*StringBuilder sb = new StringBuilder();
                foreach (var sql in sqls)
                {
                    sb.AppendLine(sql);
                    sb.AppendLine("GO");
                }*/
                foreach (var sql in sqls)
                    context.Database.ExecuteSqlCommand(sql);
                context.SaveChanges();
            }
        }

        public class TimeOrder
        {
            public int MachineNumber { get; set; }
            public DateTime measurementTime { get; set; }
            public double measurement { get; set; }
            public TimeOrder(int machineNumber, DateTime T, double d)
            {
                MachineNumber = machineNumber;
                measurementTime = T;
                measurement = d;
            }
        }

    }

}
