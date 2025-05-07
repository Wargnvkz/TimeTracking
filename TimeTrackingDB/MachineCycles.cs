using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class MachineCycles
    {
        private static string ConnectionString = "Data Source=192.168.0.12;Initial Catalog=PetPro;User id=sa;Password=DbSyS@dm1n;";
        private List<RainTCycleCounter> Data;

        public static MachineCycles Get(DateTime From, int machine)
        {
            var mc = new MachineCycles();
            var result = new List<RainTCycleCounter>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select [DateTime],[Eq],[Counter] from RainTCycleCounter where [DateTime]>@from AND [eq]=@eq order by [DateTime]", conn);
                cmd.Parameters.Add(new SqlParameter("@from", From));
                cmd.Parameters.Add(new SqlParameter("@eq", machine));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var cc = new RainTCycleCounter();
                    cc.DateTime = reader.GetDateTime(0);
                    cc.Eq = reader.GetInt32(1);
                    cc.Counter = reader.GetInt32(2);
                    result.Add(cc);
                }
            }
            mc.Data = result;
            mc.PrepareData();
            return mc;
        }
        public static HashSet<int> GetMachines(DateTime From)
        {
            var machines = new HashSet<int>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select DISTINCT [Eq] from RainTCycleCounter where [DateTime]>@from order by [eq]", conn);
                cmd.Parameters.Add(new SqlParameter("@from", From));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var Eq = reader.GetInt32(0);
                    machines.Add(Eq);
                }
            }
            return machines;
        }

        public void SetData(List<RainTCycleCounter> data)
        {
            Data = data;
            PrepareData();
        }
        public int Count { get => Data.Count; }

        /*public bool IsProperlyWorkingAtTime(double minutesAgo)
        {

        }*/

        public List<DateTime> MachineWasStoppedAt()
        {
            var TestPeriod = 300;
            if (Data == null) return new List<DateTime>() { DateTime.Now.AddSeconds(-TestPeriod) };
            var times = new List<DateTime>();
            var cycles = AvgCycleTime(TestPeriod);
            if (cycles.Count < 2) return new List<DateTime>() { DateTime.Now.AddSeconds(-TestPeriod) };
            int prvIndex = -1;
            int idleIndex = -1;
            do
            {
                idleIndex = cycles.FindIndex(prvIndex + 1, c => c == 0);
                if (idleIndex == -1) break;
                times.Add(Data[idleIndex].DateTime);
                prvIndex = idleIndex;
            } while (idleIndex != -1);
            return times;
        }

        /*public double AvgCycleTime()
        {
            PrepareData();
            //X - Datetime, Y= Count
            var N = Data.Count;
            var sumX = Data.Sum(d => d.X);
            var sumX2 = Data.Sum(d => d.X*d.X);
            var sumY = Data.Sum(d => d.Y);
            var sumXY = Data.Sum(d => d.X*d.Y);
            var k = (N * sumXY - sumX * sumY) / (N*sumX2 - sumX * sumX);
            return 1/k;
        }*/
        /// <summary>
        /// Возвращает список из времен циклов. Порядок и индекс результат совпадают с входными данным
        /// </summary>
        /// <param name="periodInSeconds"></param>
        /// <returns></returns>
        public List<double> AvgCycleTime(double periodInSeconds)
        {
            var res = new List<double>();
            for (int i = 0; i < Data.Count; i++)
            {
                var endPeriod = Data[i].DateTime.AddSeconds(periodInSeconds);
                var endPeriodIndex = FindNextIndexOfTime(endPeriod);
                if (endPeriodIndex == -1) break;
                var time = AvgCycleTime(Data, i, endPeriodIndex);
                res.Add(time);

            }
            return res;
        }
        private double AvgCycleTime(List<RainTCycleCounter> data, int startIndex, int endIndex)
        {
            //X - Datetime, Y= Count
            var N = endIndex - startIndex + 1;
            double sumX = 0;
            double sumX2 = 0;
            double sumY = 0;
            double sumXY = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var d = data[i];
                sumX += d.X;
                sumX2 += d.X * d.X;
                sumY += d.Y;
                sumXY += d.X * d.Y;
            }
            var k = (N * sumXY - sumX * sumY) / (N * sumX2 - sumX * sumX);
            return k == 0 ? double.PositiveInfinity: 1 / k;
        }

        public double StdDev(int startIndex, int endIndex)
        {
            //X - Datetime, Y= Count
            var N = endIndex - startIndex + 1;
            double sumX = 0;
            double sumX2 = 0;
            double sumY = 0;
            double sumXY = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var d = Data[i];
                sumX += d.X;
                sumX2 += d.X * d.X;
                sumY += d.Y;
                sumXY += d.X * d.Y;
            }
            var k = (N * sumXY - sumX * sumY) / (N * sumX2 - sumX * sumX);
            return k == 0 ? double.MaxValue : 1 / k;
        }

        public DateTime LastDataDateTime()
        {
            return Data.Max(d => d.DateTime);
        }

        internal void PrepareData()
        {
            if (Data == null || Data.Count < 2) return;
            var minDt = Data.Min(d => d.DateTime);
            Data.ForEach(d => { d.X = (d.DateTime - minDt).TotalSeconds; });
            var prvCounter = Data[0].Counter;
            var addCounter = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                var d = Data[i];
                int dData = 0;
                if (i>0)
                    dData = Data[i].Counter - Data[i - 1].Counter;
                if (dData<0 && i > 0 && Data[i - 1].Counter != 0)
                {
                    addCounter = prvCounter + 1;
                }
                int sumCounter = d.Counter + addCounter;
                d.Y = sumCounter;
                prvCounter = sumCounter;
            }
        }
        private void PrepareData(List<RainTCycleCounter> data)
        {
            var minDt = data.Min(d => d.DateTime);
            data.ForEach(d => { d.X = (d.DateTime - minDt).TotalSeconds; });
            var prvCounter = data[0].Counter;
            var addCounter = 0;
            data.ForEach(d =>
            {
                if (d.Counter == 0)
                {
                    addCounter = prvCounter + 1;
                }
                int sumCounter = d.Counter + addCounter;
                d.Y = sumCounter;
                prvCounter = sumCounter;
            });
        }

        public int FindNextIndexOfTime(DateTime dateTime)
        {
            int leftIndex = 0;
            int rightIndex = this.Data.Count - 1;
            if (Data[leftIndex].DateTime < dateTime && Data[rightIndex].DateTime >= dateTime)
            {
                while (rightIndex - leftIndex > 1)
                {
                    var centerIndex = (leftIndex + rightIndex) / 2;
                    //var lbLess = data[leftIndex].DateTime < dateTime;
                    //var rbMore = data[rightIndex].DateTime > dateTime;
                    var cbLess = Data[centerIndex].DateTime < dateTime;
                    //if (data[centerIndex].DateTime == dateTime) return centerIndex;
                    if (cbLess)
                    {
                        leftIndex = centerIndex;
                    }
                    else
                    {
                        rightIndex = centerIndex;
                    }
                }
                return rightIndex;
            }
            else return -1;
        }
        public class RainTCycleCounter
        {
            public DateTime DateTime;
            public int Eq;
            public int Counter;
            internal double X;
            internal double Y;
        }


    }


    public static class Extensions
    {
        public static double StdDev(this List<double> values, int startIndex, int endIndex)
        {
            double ret = 0;
            int count = values.Count();
            if (endIndex >= count) endIndex = count - 1;
            if (count > 1)
            {
                var avg = 0d;
                var avg2 = 0d;
                for (int i = startIndex; i < endIndex; i++)
                {
                    var v = values[i];

                    avg += v;
                    avg2 += v * v;
                }
                var d = avg2 - avg * avg;
                ret = Math.Sqrt(d);
                /*
                 * //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);*/
            }
            return ret;
        }
        public static double StdDev(this List<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
    }

}
