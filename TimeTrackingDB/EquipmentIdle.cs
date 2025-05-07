using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using TimeTrackingLib;

namespace TimeTrackingDB
{
    public class EquipmentIdle
    {
        [Key]
        public int EquipmentIdleID { get; set; }
        public DateTime? ShiftStart { get; set; }
        public bool IsNightShift { get; set; }
        public int EquipmentNumber { get; set; }
        public DateTime? IdleStart { get; set; }
        public DateTime? IdleEnd { get; set; }
        public int? MalfunctionReasonTypeID { get; set; }
        public int? MalfunctionReasonProfileID { get; set; }
        public int? MalfunctionReasonNodeID { get; set; }
        public int? MalfunctionReasonElementID { get; set; }
        public int? MalfunctionReasonMalfunctionTextID { get; set; }
        public string MalfunctionReasonMalfunctionTextComment { get; set; }
        /// <summary>
        /// ID работы в сапе
        /// </summary>
        public string SAPOrderID { get; set; }

        //нужен признак, что строка является главной (и можно редактировать конец) и признак, что строка является второстепенной (и можно редактировать начало),
        //и, какая строка является главной (если есть главная строка, то данная строка второстепенная)

        /// <summary>
        /// номер верхней (родительской) строки после разделения единой записи. Ненулевое значение означает, что данная запись является дочерней в разделенной паре. Можно редактировать начало простоя.
        /// Начало периода дублируется в конец периода родительской записи
        /// </summary>
        public int? DivisionParentEquipmentIdleID { get; set; }
        /// <summary>
        /// Ненулевое значение означает, что данная запись является главной в разделенной паре (и у этой записи можно редактировать конец простоя)
        /// Запись о конце периода дублируется в начало периода дочерней записи
        /// </summary>
        public int? DivisionChildEquipmentIdleID { get; set; }
        /// <summary>
        /// Машина стоит, не запущена. Простой открыт.
        /// </summary>
        public bool IsOpenIdle { get; set; }

        [NotMapped]
        public TimeSpan IdleDuration
        {
            get
            {
                if (IdleStart.HasValue && IdleEnd.HasValue)
                {
                    if (IdleEnd.Value < IdleStart.Value)
                    {
                        var duration = IdleEnd.Value.AddDays(1) - IdleStart.Value;// EndValue - StartValue;
                        return duration;

                    }
                    else
                    {
                        var duration = IdleEnd.Value - IdleStart.Value;// EndValue - StartValue;
                        return duration;
                    }
                }
                else return new TimeSpan();
            }
        }

        [NotMapped]
        public bool IsAllowedToEditEndOfPeriod
        {
            get
            {
                return DivisionChildEquipmentIdleID.HasValue;
            }
        }
        [NotMapped]
        public bool IsAllowedToEditStartOfPeriod
        {
            get
            {
                return DivisionParentEquipmentIdleID.HasValue;

            }
        }
        [NotMapped]
        public bool IsAbleToDivide
        {
            get
            {
                return String.IsNullOrEmpty(SAPOrderID) && IdleStart.HasValue && IdleEnd.HasValue;
            }
        }
        [NotMapped]
        public bool IsRecordDivided
        {
            get
            {
                return DivisionParentEquipmentIdleID.HasValue || DivisionChildEquipmentIdleID.HasValue;
            }
        }

        [NotMapped]
        public int ShiftNumber
        {
            get
            {
                return Shift.GetShiftNumber(ShiftStart.Value, IsNightShift);
            }
        }
    }
}
