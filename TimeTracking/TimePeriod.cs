using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracking.ShiftsForms
{
    public class TimePeriod
    {
        public DateTime Start;
        public DateTime End;
        public string Text;
        public TimePeriod(DateTime start, DateTime end, string text)
        {
            Start = start;
            End = end;
            Text = text;
        }
        public TimePeriod(DateTime start, DateTime end, TimePeriod tp)
        {
            Start = start;
            End = end;
            Text = tp.Text;
        }

        private static List<TimePeriod> AddTimePeriod(List<TimePeriod> tpList, TimePeriod tp, bool ListIsMain)
        {
            if (tpList == null || tpList.Count == 0) return new List<TimePeriod>() { tp };
            if (tp == null) return tpList;

            List<TimePeriod> prvLstresult = tpList;
            List<TimePeriod> TPresult = new List<TimePeriod>();

            Stack<TimePeriod> PeriodsStack = new Stack<TimePeriod>();
            PeriodsStack.Push(tp);
            TimePeriod Current;
            while (PeriodsStack.Count > 0)
            {
                List<TimePeriod> LstResult = new List<TimePeriod>();
                Current = PeriodsStack.Pop();
                bool StopFor = false;
                for (int i = 0; i < prvLstresult.Count; i++)
                {
                    var tpE = prvLstresult[i];
                    if (Current == null)
                    {
                        LstResult.Add(tpE);
                    }
                    else
                    {
                        MergeWith(tpE, Current, ListIsMain, out List<TimePeriod> tpElst, out List<TimePeriod> tpLst);
                        LstResult.AddRange(tpElst);

                        switch (tpLst.Count)
                        {
                            case 0:
                                Current = null;
                                StopFor = true; // break for loop/goto outside loop
                                break;
                            case 1:
                                Current = tpLst[0];
                                break;
                            case 2:
                                PeriodsStack.Push(tpLst[1]);
                                Current = tpLst[0];
                                break;
                        }
                        if (StopFor) break;
                    }
                }
                if (Current != null)
                    TPresult.Add(Current);
                prvLstresult = LstResult;
            }
            var result = new List<TimePeriod>();
            result.AddRange(prvLstresult);
            result.AddRange(TPresult);
            result = result.OrderBy(r => r.Start).ToList();
            return result;
        }
        
        private static List<TimePeriod> Merge(List<TimePeriod> List1, List<TimePeriod> List2, bool IsList1Main)
        {
            if (List1 == null || List1.Count == 0) return List2;
            if (List2 == null || List2.Count == 0) return List1;
            var result = List1;
            for (int j = 0; j < List2.Count; j++)
            {
                result = AddTimePeriod(result, List2[j], IsList1Main);
            }
            return result;
        }

        public static List<TimePeriod> CrossPeriods(List<TimePeriod> MachineWorkingPeriods, List<TimePeriod> SAPRepairPeriods, List<TimePeriod> TotalShowedPeriods)
        {
            var res = Merge(MachineWorkingPeriods, SAPRepairPeriods, false);
            var res1 = Merge(res, TotalShowedPeriods, false);

            return res1;
        }

        public void Normalize()
        {
            if (Start > End)
            {
                var tmp = Start;
                Start = End;
                End = tmp;
            }

        }

        private ComparisonResult CompareStartsWith(TimePeriod tp)
        {
            if (tp == null) return ComparisonResult.Error;
            if (tp.Start < Start) return ComparisonResult.ThisIsAfter;
            if (tp.Start > Start) return ComparisonResult.ThisIsBefore;
            return ComparisonResult.ThisIsEqual;
        }
        private ComparisonResult CompareEndsWith(TimePeriod tp)
        {
            if (tp == null) return ComparisonResult.Error;
            if (tp.End < End) return ComparisonResult.ThisIsAfter;
            if (tp.End > End) return ComparisonResult.ThisIsBefore;
            return ComparisonResult.ThisIsEqual;
        }
        private ComparisonResult CompareStartWithEndOf(TimePeriod tp)
        {
            if (tp == null) return ComparisonResult.Error;
            if (tp.End < Start) return ComparisonResult.ThisIsAfter;
            if (tp.End > Start) return ComparisonResult.ThisIsBefore;
            return ComparisonResult.ThisIsEqual;
        }
        private ComparisonResult CompareEndWithStartOf(TimePeriod tp)
        {
            if (tp == null) return ComparisonResult.Error;
            if (tp.Start < End) return ComparisonResult.ThisIsAfter;
            if (tp.Start > End) return ComparisonResult.ThisIsBefore;
            return ComparisonResult.ThisIsEqual;
        }

        public List<TimePeriod> MergeWith(TimePeriod tp, bool IsCurrentMainPeriod)
        {
            this.Normalize();
            tp.Normalize();
            TimePeriod MainPeriod = IsCurrentMainPeriod ? this : tp;
            var StartsCompared = CompareStartsWith(tp);
            var EndsCompared = CompareEndsWith(tp);
            var ThisEndComparedWithStart = CompareEndWithStartOf(tp);
            var ThisStartComparedWithEnd = CompareStartWithEndOf(tp);
            switch (StartsCompared)
            {
                case ComparisonResult.ThisIsBefore:
                    {
                        switch (ThisEndComparedWithStart)
                        {
                            case ComparisonResult.ThisIsBefore:
                            case ComparisonResult.ThisIsEqual:
                                {
                                    return new List<TimePeriod>() { this, tp };
                                }
                            case ComparisonResult.ThisIsAfter:
                                {
                                    switch (EndsCompared)
                                    {
                                        case ComparisonResult.ThisIsBefore:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod>{
                                                    this,
                                                    new TimePeriod(this.End,tp.End,tp)
                                                    };

                                                }
                                                else
                                                {
                                                    return new List<TimePeriod>{
                                                    new TimePeriod(this.Start, tp.Start, this),
                                                    tp
                                                    };

                                                }
                                            }
                                        case ComparisonResult.ThisIsEqual:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod>{
                                                    this
                                                    };

                                                }
                                                else
                                                {
                                                    return new List<TimePeriod>{
                                                    new TimePeriod(this.Start, tp.Start, this),
                                                    tp
                                                    };

                                                }
                                            }
                                        case ComparisonResult.ThisIsAfter:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod> { this };
                                                }
                                                else
                                                {
                                                    return new List<TimePeriod>
                                                    {
                                                        new TimePeriod(this.Start,tp.Start,this),
                                                        tp,
                                                        new TimePeriod(tp.End,this.End,this)
                                                    };
                                                }
                                            }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ComparisonResult.ThisIsEqual:
                    {
                        switch (EndsCompared)
                        {
                            case ComparisonResult.ThisIsBefore:
                                {
                                    if (MainPeriod == this)
                                    {
                                        return new List<TimePeriod> {
                                            this,
                                            new TimePeriod(this.End,tp.End,tp)
                                        };
                                    }
                                    else
                                    {
                                        return new List<TimePeriod> {
                                            tp
                                        };

                                    }
                                }
                            case ComparisonResult.ThisIsEqual:
                                return new List<TimePeriod> { MainPeriod };
                            case ComparisonResult.ThisIsAfter:
                                if (MainPeriod == this)
                                {
                                    return new List<TimePeriod> { this };
                                }
                                else
                                {
                                    return new List<TimePeriod> {
                                            tp,
                                            new TimePeriod(tp.End,this.End,this)
                                        };
                                }
                        }
                    }
                    break;
                case ComparisonResult.ThisIsAfter:
                    {
                        switch (ThisStartComparedWithEnd)
                        {
                            case ComparisonResult.ThisIsBefore:
                                {
                                    switch (EndsCompared)
                                    {
                                        case ComparisonResult.ThisIsBefore:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod>
                                                    {
                                                        new TimePeriod(tp.Start,this.Start,tp),
                                                        this,
                                                        new TimePeriod(this.End,tp.End,tp)
                                                    };
                                                }
                                                else
                                                {
                                                    return new List<TimePeriod> { tp };
                                                }
                                            }
                                        case ComparisonResult.ThisIsEqual:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod>
                                                    {
                                                        new TimePeriod(tp.Start,this.Start,tp),
                                                        this
                                                    };
                                                }
                                                else
                                                {
                                                    return new List<TimePeriod> { tp };
                                                }
                                            }
                                        case ComparisonResult.ThisIsAfter:
                                            {
                                                if (MainPeriod == this)
                                                {
                                                    return new List<TimePeriod>
                                                    {
                                                        new TimePeriod(tp.Start,this.Start,tp),
                                                        this
                                                    };
                                                }
                                                else
                                                {
                                                    return new List<TimePeriod>
                                                    {
                                                        tp,
                                                        new TimePeriod(tp.End,this.End,this),
                                                    };
                                                }
                                            }
                                    }

                                }
                                break;
                            case ComparisonResult.ThisIsEqual:
                            case ComparisonResult.ThisIsAfter:
                                {
                                    return new List<TimePeriod>
                                    {
                                        tp,
                                        this
                                    };
                                }
                        }
                    }
                    break;

            }
            return new List<TimePeriod>();
        }
        public static void MergeWith(TimePeriod TP1, TimePeriod TP2, bool IsTP1MainPeriod, out List<TimePeriod> resTP1Period, out List<TimePeriod> resTP2Period)
        {
            TP1.Normalize();
            TP2.Normalize();
            TimePeriod MainPeriod = IsTP1MainPeriod ? TP1 : TP2;
            var StartsCompared = TP1.CompareStartsWith(TP2);
            var EndsCompared = TP1.CompareEndsWith(TP2);
            var MainEndComparedWithStart = TP1.CompareEndWithStartOf(TP2);
            var MainStartComparedWithEnd = TP1.CompareStartWithEndOf(TP2);
            switch (StartsCompared)
            {
                case ComparisonResult.ThisIsBefore:
                    {
                        switch (MainEndComparedWithStart)
                        {
                            case ComparisonResult.ThisIsBefore:
                            case ComparisonResult.ThisIsEqual:
                                {
                                    resTP1Period = new List<TimePeriod> { TP1 };
                                    resTP2Period = new List<TimePeriod> { TP2 };
                                    return;
                                }
                                break;
                            case ComparisonResult.ThisIsAfter:
                                {
                                    switch (EndsCompared)
                                    {
                                        case ComparisonResult.ThisIsBefore:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> { new TimePeriod(TP1.End, TP2.End, TP2) };
                                                    return;


                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { new TimePeriod(TP1.Start, TP2.Start, TP1) };
                                                    resTP2Period = new List<TimePeriod> { TP2 };

                                                    return;

                                                }
                                            }
                                            break;
                                        case ComparisonResult.ThisIsEqual:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> { };
                                                    return;


                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { new TimePeriod(TP1.Start, TP2.Start, TP1) };
                                                    resTP2Period = new List<TimePeriod> { TP2 };

                                                    return;

                                                }
                                            }
                                            break;
                                        case ComparisonResult.ThisIsAfter:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> { };
                                                    return;
                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { new TimePeriod(TP1.Start, TP2.Start, TP1), new TimePeriod(TP2.End, TP1.End, TP1) };
                                                    resTP2Period = new List<TimePeriod> { TP2 };
                                                    return;

                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ComparisonResult.ThisIsEqual:
                    {
                        switch (EndsCompared)
                        {
                            case ComparisonResult.ThisIsBefore:
                                {
                                    if (MainPeriod == TP1)
                                    {
                                        resTP1Period = new List<TimePeriod> { TP1 };
                                        resTP2Period = new List<TimePeriod> { new TimePeriod(TP1.End, TP2.End, TP2) };
                                        return;

                                    }
                                    else
                                    {
                                        resTP1Period = new List<TimePeriod> { };
                                        resTP2Period = new List<TimePeriod> { TP2 };
                                        return;
                                    }
                                }
                                break;
                            case ComparisonResult.ThisIsEqual:
                                {
                                    if (MainPeriod == TP1)
                                    {
                                        resTP1Period = new List<TimePeriod> { TP1 };
                                        resTP2Period = new List<TimePeriod> { };
                                        return;
                                    }
                                    else
                                    {
                                        resTP1Period = new List<TimePeriod> { };
                                        resTP2Period = new List<TimePeriod> { TP2 };

                                        return;
                                    }
                                }
                                break;
                            case ComparisonResult.ThisIsAfter:
                                if (MainPeriod == TP1)
                                {
                                    resTP1Period = new List<TimePeriod> { TP1 };
                                    resTP2Period = new List<TimePeriod> { };
                                    return;
                                }
                                else
                                {
                                    resTP1Period = new List<TimePeriod> { new TimePeriod(TP2.End, TP1.End, TP1) };
                                    resTP2Period = new List<TimePeriod> { TP2 };
                                    return;
                                }
                                break;
                        }
                    }
                    break;
                case ComparisonResult.ThisIsAfter:
                    {
                        switch (MainStartComparedWithEnd)
                        {
                            case ComparisonResult.ThisIsBefore:
                                {
                                    switch (EndsCompared)
                                    {
                                        case ComparisonResult.ThisIsBefore:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> {
                                                        new TimePeriod(TP2.Start, TP1.Start, TP2),
                                                        new TimePeriod(TP1.End, TP2.End, TP2)
                                                    };
                                                    return;
                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { };
                                                    resTP2Period = new List<TimePeriod> { TP2 };
                                                    return;
                                                }
                                            }
                                            break;
                                        case ComparisonResult.ThisIsEqual:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> { new TimePeriod(TP2.Start, TP1.Start, TP2) };
                                                    return;

                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { };
                                                    resTP2Period = new List<TimePeriod> { TP2 };
                                                    return;
                                                }
                                            }
                                            break;
                                        case ComparisonResult.ThisIsAfter:
                                            {
                                                if (MainPeriod == TP1)
                                                {
                                                    resTP1Period = new List<TimePeriod> { TP1 };
                                                    resTP2Period = new List<TimePeriod> { new TimePeriod(TP2.Start, TP1.Start, TP2) };
                                                    return;

                                                }
                                                else
                                                {
                                                    resTP1Period = new List<TimePeriod> { new TimePeriod(TP2.End, TP1.End, TP1) };
                                                    resTP2Period = new List<TimePeriod> { TP2 };
                                                    return;

                                                }
                                            }
                                            break;
                                    }

                                }
                                break;
                            case ComparisonResult.ThisIsEqual:
                            case ComparisonResult.ThisIsAfter:
                                {
                                    resTP1Period = new List<TimePeriod> { TP1 };
                                    resTP2Period = new List<TimePeriod> { TP2 };
                                    return;
                                }
                                break;
                        }
                    }
                    break;

            }
            resTP1Period = new List<TimePeriod> { };
            resTP2Period = new List<TimePeriod> { };
        }

        private enum ComparisonResult
        {
            ThisIsBefore,
            ThisIsEqual,
            ThisIsAfter,
            Error
        }

    }
}
