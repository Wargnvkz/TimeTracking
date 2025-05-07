using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;
using TimeTrackingLib;

namespace TimeTracking
{
    public class TimeTrackingDataForm : Form
    {
        protected FormData CurrentData;
        protected TimeSpan DurationAfterEndOfShift = new TimeSpan(0, 30, 0);
        protected ActionEditAllowed actionEditAlowed;
        protected ActionAddAdditionalCommentAllowed actionAddCommentAllowed;
        public void SetCurrentData(FormData currentData)
        {
            CurrentData = currentData;
            SetActionAllowed(new Shift());
        }

        protected virtual void SetActionAllowed(Shift selectedShift)
        {
            if (CurrentData == null || CurrentData.CurrentUser == null) return;
            var currentshift = new Shift();
            actionEditAlowed = ActionEditAllowed.NoEdit;
            actionAddCommentAllowed = ActionAddAdditionalCommentAllowed.NotAllowed;
            if (CurrentData.CurrentUser.Rights.HasFlag(UserRight.Administrator))
            {
                actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.FullEdit);
                actionAddCommentAllowed = SetActionAddAdditionalCommentAllowed(actionAddCommentAllowed, ActionAddAdditionalCommentAllowed.Allowed);
            }
            if (CurrentData.CurrentUser.Rights.HasFlag(UserRight.EngineerOperator))
                if (currentshift.ShiftDate == selectedShift.ShiftDate && currentshift.IsNight == selectedShift.IsNight)
                {
                    actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.UserEdit);
                    actionAddCommentAllowed = SetActionAddAdditionalCommentAllowed(actionAddCommentAllowed, ActionAddAdditionalCommentAllowed.NotAllowed);

                }
                else
                {
                    if ((DateTime.Now - currentshift.GetShiftStartDateTime()) < DurationAfterEndOfShift)
                    {
                        actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.UserEdit);

                    }
                    else
                    {
                        actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.NoEdit);
                    }
                }
            if (CurrentData.CurrentUser.Rights.HasFlag(UserRight.EngineerMaintainService))
            {
                actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.NoEdit);
                actionAddCommentAllowed = SetActionAddAdditionalCommentAllowed(actionAddCommentAllowed, ActionAddAdditionalCommentAllowed.Allowed);

            }
            if (CurrentData.CurrentUser.Rights.HasFlag(UserRight.Technologist))
            {
                actionEditAlowed = SetEditAllowedLevel(actionEditAlowed, ActionEditAllowed.NoEdit);
                actionAddCommentAllowed = SetActionAddAdditionalCommentAllowed(actionAddCommentAllowed, ActionAddAdditionalCommentAllowed.Allowed);

            }
        }

        private ActionEditAllowed SetEditAllowedLevel(ActionEditAllowed currentRightLevel, ActionEditAllowed NewLevel)
        {
            return currentRightLevel < NewLevel ? NewLevel : currentRightLevel;
        }
        private ActionAddAdditionalCommentAllowed SetActionAddAdditionalCommentAllowed(ActionAddAdditionalCommentAllowed currentRightLevel, ActionAddAdditionalCommentAllowed NewLevel)
        {
            return currentRightLevel < NewLevel ? NewLevel : currentRightLevel;
        }
    }

    public class FormData
    {
        public User CurrentUser;
    }

    public enum ActionEditAllowed
    {
        NoEdit = 0,
        UserEdit = 1,
        FullEdit = 2,
    }
    public enum ActionAddAdditionalCommentAllowed
    {
        NotAllowed = 0,
        Allowed = 1,
    }
}
