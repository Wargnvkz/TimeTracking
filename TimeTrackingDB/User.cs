using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string AutoLogonComputerNames { get; set; } //Имя компьютера, на котором можно входить без пароля
        public string PasswordHash { get; set; } //Хэш пароля
        public UserRight Rights { get; set; } // права пользователя

        [NotMapped]
        public List<string> ComputerNames
        {
            get
            {
                var res = new List<string>();
                if (AutoLogonComputerNames == null) return res;
                Array.ForEach(AutoLogonComputerNames.Split(','), cnsc => Array.ForEach(cnsc.Split(';'), cn => res.Add(cn.Trim().ToUpper())));
                return res;
            }
        }

        public bool CheckComputerName(string ComputerName)
        {
            var cns = ComputerNames;
            return cns.Contains(ComputerName.ToUpper()) || cns.Contains("*");
        }
    }

    public enum UserRight
    {
        EngineerMaintainService = 1,
        EngineerOperator = 2,
        Administrator = 4,
        Technologist = 8
    }
}
