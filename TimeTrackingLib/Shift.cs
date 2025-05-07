using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTrackingLib
{
    public class Shift
    {
        public DateTime ShiftDate { get; protected set; }
        public bool IsNight { get; protected set; }
        public Shift(DateTime dt)
        {
            ShiftDate = GetShiftDate(dt);
            IsNight = GetIsNightShift(dt);
        }
        public Shift()
        {
            var dt = DateTime.Now;
            ShiftDate = GetShiftDate(dt);
            IsNight = GetIsNightShift(dt);
        }
        public Shift(DateTime shiftDate, bool isNightShift)
        {
            ShiftDate = shiftDate.Date;
            IsNight = isNightShift;
        }

        public DateTime GetShiftStartDateTime()
        {
            return ShiftDate.Add(IsNight ? _20 : _8);
        }

        protected DateTime GetShiftDate(DateTime dt)
        {
            if (GetIsNightShift(dt) && dt.TimeOfDay < _8)
            {
                return dt.Date.AddDays(-1);
            }
            else
            {
                return dt.Date;

            }

        }
        protected bool GetIsNightShift(DateTime dt)
        {
            if (dt.TimeOfDay >= _8 && dt.TimeOfDay < _20) return false;
            return true;
        }
        public static DateTime CurrentShiftDate
        {
            get
            {
                var now = DateTime.Now;
                if (CurrentIsNightShift && now.TimeOfDay < _8)
                {
                    return now.Date.AddDays(-1);
                }
                else
                {
                    return now.Date;

                }
            }
        }
        public static bool CurrentIsNightShift
        {
            get
            {
                var now = DateTime.Now;
                if (now.TimeOfDay >= _8 && now.TimeOfDay < _20) return false;
                return true;
            }
        }

        public static DateTime DateTimeForCurrentShift(DateTime CurrentShiftaDate, bool IsNightShift, DateTime TimeInsideShift)
        {
            var time = TimeInsideShift.TimeOfDay;
            if (!IsNightShift)
            {
                if (time < _8) throw new ArgumentOutOfRangeException("Время внутри дневной смены должно быть больше 8 часов утра");
                if (time > _20) throw new ArgumentOutOfRangeException("Время внутри дневной смены должно быть меньше 8 часов вечера");
                return CurrentShiftaDate.Add(time);
            }
            else
            {
                if (time >= _8 && time < _20) throw new ArgumentOutOfRangeException("Время внутри ночной смены должно быть меньше 8 часов утра или больше 8 часов вечера");
                if (time < _8)
                {
                    return CurrentShiftaDate.AddDays(1).Add(time);
                }
                else
                {
                    return CurrentShiftaDate.Add(time);
                }
            }
        }

        public static int GetShiftNumber(DateTime StartShift, bool IsNightShift)
        {
            int ShiftNumber;
            var days = (StartShift - DateTime.MinValue).TotalDays;
            if (IsNightShift)
            {
                ShiftNumber = Convert.ToInt32(Math.Round(((days + 1) % 4 + 1)));
            }
            else
            {
                ShiftNumber = Convert.ToInt32(Math.Round(((days + 2) % 4 + 1)));
            }
            return ShiftNumber;
        }
        public Shift PreviousShift()
        {
            if (IsNight)
            {
                return new Shift(ShiftDate, false);
            }
            else
            {
                return new Shift(ShiftDate.AddDays(-1), true);
            }
        }
        public Shift NextShift()
        {
            if (IsNight)
            {
                return new Shift(ShiftDate.AddDays(1), false);
            }
            else
            {
                return new Shift(ShiftDate, true);
            }
        }
        public DateTime ShiftStartsAt()
        {
            if (!IsNight) return ShiftDate.Date.Add(_8); else return ShiftDate.Date.Add(_20);
        }
        public DateTime ShiftEndsAt()
        {
            if (IsNight) return ShiftDate.Date.Add(_32); else return ShiftDate.Date.Add(_20);
        }

        public bool Equals(Shift s)
        {
            return s.ShiftDate == ShiftDate && s.IsNight == IsNight;
        }
        public static bool operator ==(Shift s1,Shift s2)
        {
            return s1.ShiftDate == s2.ShiftDate && s1.IsNight == s2.IsNight;
        }
        public static bool operator !=(Shift s1, Shift s2)
        {
            return s1.ShiftDate != s2.ShiftDate || s1.IsNight != s2.IsNight;
        }
        public static bool operator >(Shift s1, Shift s2)
        {
            return s1.ShiftStartsAt() > s2.ShiftStartsAt();
        }
        public static bool operator <(Shift s1, Shift s2)
        {
            return s1.ShiftStartsAt() < s2.ShiftStartsAt();
        }
        public static bool operator >=(Shift s1, Shift s2)
        {
            return s1.ShiftStartsAt() > s2.ShiftStartsAt() || s1.ShiftStartsAt() == s2.ShiftStartsAt();
        }
        public static bool operator <=(Shift s1, Shift s2)
        {
            return s1.ShiftStartsAt() < s2.ShiftStartsAt() || s1.ShiftStartsAt() == s2.ShiftStartsAt();
        }


        public static TimeSpan _8 = new TimeSpan(8, 0, 0);
        public static TimeSpan _20 = new TimeSpan(20, 0, 0);
        public static TimeSpan _32 = new TimeSpan(32, 0, 0);
    }
}
