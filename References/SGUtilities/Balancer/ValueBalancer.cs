using System;
using System.Collections.Generic;
using SGNativeEntities.General;

namespace SGUtilities.Balancer
{
    public class ValueBalancer<T> where T : BasicInfoEntity
    {
        private enum WORK_TIME
        {
            NONE = 0,
            MORNING = 1,
            AFTERNOON = 2
        }

        private T msi_info = null;
        private WORK_TIME mwt_time = WORK_TIME.NONE;
        private DateTime MDT_0000
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0); }
        }
        private DateTime MDT_1200
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 12, 0, 0); }
        }
        private DateTime MDT_2359
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59); }
        }

        public T Info
        {
            get
            {
                return msi_info == null ? null : msi_info.Clone<T>();
            }
        }

        public List<T> Balance(T asi_info)
        {
            WORK_TIME lwt_time = WORK_TIME.NONE;
            List<T> llst_temp = new List<T>();
            if (asi_info == null) return llst_temp;
            if ((asi_info.Time >= MDT_0000 && asi_info.Time <= MDT_1200)) lwt_time = WORK_TIME.MORNING;
            if ((asi_info.Time > MDT_1200 && asi_info.Time <= MDT_2359)) lwt_time = WORK_TIME.AFTERNOON;

            var lsi_info = asi_info.Clone<T>();
            if (lwt_time != WORK_TIME.NONE)
            {
                if (msi_info == null || msi_info.Time.Date != lsi_info.Time.Date)
                {
                    lsi_info.VolAmount = lsi_info.GVolAmount;
                    lsi_info.VolMoney = lsi_info.GVolMoney;
                    llst_temp.Add(lsi_info.Clone<T>());
                }
                else
                {
                    lsi_info.VolAmount = lsi_info.GVolAmount - msi_info.GVolAmount;
                    lsi_info.VolMoney = lsi_info.GVolMoney - msi_info.GVolMoney;

                    var seconds = (mwt_time != lwt_time) ? 1 : (lsi_info.Time - msi_info.Time).Seconds;
                    while (seconds > 0)
                    {
                        T lsi_temp = null;
                        if (seconds == 1)
                        {
                            lsi_temp = lsi_info.Clone<T>();
                        }
                        else
                        {
                            msi_info.Time = msi_info.Time.AddSeconds(1);
                            lsi_temp = msi_info.Clone<T>();
                            lsi_temp.VolAmount = decimal.Zero;
                            lsi_temp.VolMoney = decimal.Zero;
                        }
                        llst_temp.Add(lsi_temp);
                        seconds--;
                    }
                }
            }
            msi_info = lwt_time == WORK_TIME.NONE ? null : lsi_info.Clone<T>();
            mwt_time = lwt_time;

            return llst_temp;
        }
    }
}
