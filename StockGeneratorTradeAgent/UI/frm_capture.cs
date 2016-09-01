using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SGTasks.BusinessTasks;
using SGUtilities.Utilities;
using System.Diagnostics;
using System.Collections.Generic;
using SGNativeEntities.Entities.General;
using System.Threading;
using System.Text;

namespace StockGeneratorTradeAgent.UI
{
    public partial class frm_capture : Form
    {
        private int flag = 0;
        private MouseHook mouse = null;
        private Process process = null;
        private Dictionary<int, Rect> RectList = null;
        private Dictionary<int, Point> PointList = null;

        public frm_capture()
        {
            InitializeComponent();

            RectList = new Dictionary<int, Rect>();
            PointList = new Dictionary<int, Point>();
            mouse = new MouseHook();

            this.Load += new EventHandler(frm_capture_Load);
            this.FormClosing += new FormClosingEventHandler(frm_capture_FormClosing);

            mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
        }

        void frm_capture_FormClosing(object sender, FormClosingEventArgs e)
        {
            mouse.Stop();
        }

        void frm_capture_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            //mouse.Start();
            txt_program.Text = TaskManager.Instance.Configuration.TradeInfoEntity.ApplicationName;
        }

        void mouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            if (flag > 0)
            {
                this.Text = string.Format("Capture({0},{1})", e.X, e.Y);
                if (((e.Button == System.Windows.Forms.MouseButtons.Left) ||
                    (e.Button == System.Windows.Forms.MouseButtons.Right)) && e.Clicks > 0)
                {
                    int hdl = NativeMethods.WindowFromPoint(e.X, e.Y);
                    RECT rect = new RECT();
                    NativeMethods.GetWindowRect(new IntPtr(hdl), ref rect);
                    if (flag < 10)
                    {
                        var wnd_rect = new WindowRect(hdl, rect.Top, rect.Left, rect.Right - rect.Left, rect.Bottom - rect.Top);
                        Capture(flag, wnd_rect);
                    }
                    else
                    {
                        Capture(flag, new Point(e.X, e.Y));
                    }
                    
                    
                    flag = 0;
                    mouse.Stop();
                }
            }
        }

        private void Capture(int flag, WindowRect wnd_rect)
        {
            if (process != null && flag > 0)
            {
                RECT main = new RECT();
                NativeMethods.GetWindowRect(process.MainWindowHandle, ref main);

                Label llab_hnd = null;
                Label llab_rect = null;
                Label llab_cntr = null;

                switch (flag)
                {
                    case 1:
                        llab_hnd = lab_buy_hnd_code;
                        llab_rect = lab_buy_rect_code;
                        llab_cntr = lab_buy_cnt_code;
                        break;
                    case 2:
                        llab_hnd = lab_buy_hnd_price;
                        llab_rect = lab_buy_rect_price;
                        llab_cntr = lab_buy_cnt_price;
                        break;
                    case 3:
                        llab_hnd = lab_buy_hnd_amount;
                        llab_rect = lab_buy_rect_amount;
                        llab_cntr = lab_buy_cnt_amount;
                        break;
                    case 4:
                        llab_hnd = lab_buy_hnd_confirm;
                        llab_rect = lab_buy_rect_confirm;
                        llab_cntr = lab_buy_cnt_confirm;
                        break;

                    case 5:
                        llab_hnd = lab_sale_hnd_code;
                        llab_rect = lab_sale_rect_code;
                        llab_cntr = lab_sale_cnt_code;
                        break;
                    case 6:
                        llab_hnd = lab_sale_hnd_price;
                        llab_rect = lab_sale_rect_price;
                        llab_cntr = lab_sale_cnt_price;
                        break;
                    case 7:
                        llab_hnd = lab_sale_hnd_amount;
                        llab_rect = lab_sale_rect_amount;
                        llab_cntr = lab_sale_cnt_amount;
                        break;
                    case 8:
                        llab_hnd = lab_sale_hnd_confirm;
                        llab_rect = lab_sale_rect_confirm;
                        llab_cntr = lab_sale_cnt_confirm;
                        break;
                }
                RECT rect = new RECT()
                {
                    Top = wnd_rect.Top,
                    Left = wnd_rect.Left,
                    Right = wnd_rect.Left + wnd_rect.Width,
                    Bottom = wnd_rect.Top + wnd_rect.Height
                };
                Rect lrect_final = new Rect(new Point(main.Left, main.Top), rect);
                Point lpnt_center = lrect_final.GetCenterPoint();
                int hdl = lrect_final.GetWindowHandle();

                llab_hnd.Text = "Handle:" + System.Environment.NewLine + hdl.ToString();
                llab_rect.Text = string.Format("X:{0} Y:{1}", lrect_final.LocalPoint.X, lrect_final.LocalPoint.Y) + System.Environment.NewLine + string.Format("W:{0} H:{1}", lrect_final.Width, lrect_final.Height);
                llab_cntr.Text = string.Format("X:{0}", lpnt_center.X) + System.Environment.NewLine + string.Format("Y:{0}", lpnt_center.Y);

                if (RectList.ContainsKey(flag)) RectList.Remove(flag);
                RectList.Add(flag, lrect_final);
            }
        }
        private void Capture(int flag, Point point)
        {
            if (process != null && flag > 0)
            {
                Label llab_pos = null;
                switch (flag)
                {
                    case 11:
                        llab_pos = lab_pos_buy;
                        break;
                    case 12:
                        llab_pos = lab_pos_sell;
                        break;
                    case 13:
                        llab_pos = lab_pos_deal;
                        break;
                    case 14:
                        llab_pos = lab_pos_balance;
                        break;
                    case 15:
                        llab_pos = lab_pos_fresh;
                        break;
                }
                llab_pos.Text = string.Format("X:{0} Y:{1}", point.X, point.Y);
                if (PointList.ContainsKey(flag)) PointList.Remove(flag);
                PointList.Add(flag, point);
            }
        }
        private void Test(int flag, Type type, string value)
        {
            if (flag > 0 && RectList.ContainsKey(flag))
            {
                bool lb_result = false;
                CheckBox chk = null;
                switch (flag)
                {
                    case 1:
                        chk = chk_buy_code;
                        break;
                    case 2:
                        chk = chk_buy_price;
                        break;
                    case 3:
                        chk = chk_buy_amount;
                        break;
                    case 4:
                        chk = chk_buy_confirm;
                        break;

                    case 5:
                        chk = chk_sale_code;
                        break;
                    case 6:
                        chk = chk_sale_price;
                        break;
                    case 7:
                        chk = chk_sale_amount;
                        break;
                    case 8:
                        chk = chk_sale_confirm;
                        break;
                }
                if (chk != null) chk.Checked = lb_result;


                string ls_value = string.Empty;
                Point lpnt_center = RectList[flag].GetCenterPoint();
                int hdl = NativeMethods.WindowFromPoint(lpnt_center.X, lpnt_center.Y);

                if (type == typeof(TextBox))
                {
                    TradeAssistant.SendMessage(hdl, TradeAssistant.WM_SETTEXT, IntPtr.Zero, value);
                    Thread.Sleep(1000);

                    var lsb_value = new StringBuilder(50);
                    TradeAssistant.GetWindowText(hdl, lsb_value, lsb_value.Capacity);
                    TradeAssistant.SendMessage(hdl, TradeAssistant.WM_SETTEXT, IntPtr.Zero, string.Empty);
                    ls_value = lsb_value.ToString();

                    lb_result = value.Equals(ls_value.Trim());
                }
                if (type == typeof(Button))
                {
                    var lsb_value = new StringBuilder(50);
                    TradeAssistant.GetWindowText(hdl, lsb_value, lsb_value.Capacity);
                    ls_value = lsb_value.ToString();
                    lb_result = value.Equals(ls_value.Trim());
                }

                if (chk != null) chk.Checked = lb_result;
            }
        }

        private void btn_capture_program_Click(object sender, EventArgs e)
        {
            string ls_program_name = txt_program.Text;
            string ls_msg = string.Empty;
            process = FindProcess(ls_program_name, ref ls_msg);
            if (process == null)
            {
                MessageBox.Show(ls_msg);
                return;
            }
            RECT lrect_program = new RECT();
            NativeMethods.GetWindowRect(process.MainWindowHandle, ref lrect_program);
            lab_hnd_program.Text = string.Format("Handle:{0}{1}", System.Environment.NewLine, process.MainWindowHandle.ToInt32());
            lab_rect_program.Text = string.Format("X:{0} Y:{1}{2}W:{3} H:{4}", 
                lrect_program.Left, lrect_program.Top,
                System.Environment.NewLine,
                lrect_program.Right - lrect_program.Left, lrect_program.Bottom - lrect_program.Top);
            lab_account.Text = string.Format("Account:{0}{1}", 
                System.Environment.NewLine, 
                TaskManager.Instance.Configuration.TradeInfoEntity.Account);
        }

        private static Process FindProcess(string as_name, ref string as_msg)
        {
            as_msg = string.Empty;
            if (string.IsNullOrEmpty(as_name.Trim()))
            {
                as_msg = "Process name is null or empty.";
                return null;
            }
            string ls_name = as_name.Trim();
            Process[] lpcs_procs = Process.GetProcesses();
            Process lpcs_ret = null;
            foreach (Process lpcs_temp in lpcs_procs)
            {
                if (ls_name.Equals(lpcs_temp.ProcessName.Trim()))
                {
                    lpcs_ret = lpcs_temp;
                    break;
                }
            }

            if (lpcs_ret == null) as_msg = "Process was not found.";

            return lpcs_ret;
        }

        private void btn_capture_buy_code_Click(object sender, EventArgs e)
        {
            flag = 1;
            mouse.Start();
        }

        private void btn_capture_buy_price_Click(object sender, EventArgs e)
        {
            flag = 2;
            mouse.Start();
        }

        private void btn_capture_buy_amount_Click(object sender, EventArgs e)
        {
            flag = 3;
            mouse.Start();
        }

        private void btn_capture_buy_confirm_Click(object sender, EventArgs e)
        {
            flag = 4;
            mouse.Start();
        }

        private void btn_capture_sale_code_Click(object sender, EventArgs e)
        {
            flag = 5;
            mouse.Start();
        }

        private void btn_capture_sale_price_Click(object sender, EventArgs e)
        {
            flag = 6;
            mouse.Start();
        }

        private void btn_capture_sale_amount_Click(object sender, EventArgs e)
        {
            flag = 7;
            mouse.Start();
        }

        private void btn_capture_sale_confirm_Click(object sender, EventArgs e)
        {
            flag = 8;
            mouse.Start();
        }

        private void btn_capture_buy_all_Click(object sender, EventArgs e)
        {
            if (process != null)
            {
                WindowRect wnd_rect;
                RECT main = new RECT();
                NativeMethods.GetWindowRect(process.MainWindowHandle, ref main);

                for (int id = 1; id <= 4; id++)
                {
                    switch (id)
                    {
                        case 1:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.CodeRect;
                            break;
                        case 2:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.PriceRect;
                            break;
                        case 3:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.AmountRect;
                            break;
                        case 4:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.ConfirmRect;
                            break;
                        default:
                            return;
                    }
                    WindowRect wnd_rect_temp = new WindowRect(0, 
                        main.Top + wnd_rect.Top,
                        main.Left + wnd_rect.Left,
                        wnd_rect.Width,
                        wnd_rect.Height);
                    Capture(id, wnd_rect_temp);
                }
            }
        }

        private void btn_capture_sale_all_Click(object sender, EventArgs e)
        {
            if (process != null)
            {
                WindowRect wnd_rect;
                RECT main = new RECT();
                NativeMethods.GetWindowRect(process.MainWindowHandle, ref main);

                for (int id = 5; id <= 8; id++)
                {
                    switch (id)
                    {
                        case 5:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.CodeRect;
                            break;
                        case 6:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.PriceRect;
                            break;
                        case 7:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.AmountRect;
                            break;
                        case 8:
                            wnd_rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.ConfirmRect;
                            break;
                        default:
                            return;
                    }
                    WindowRect wnd_rect_temp = new WindowRect(0,
                        main.Top + wnd_rect.Top,
                        main.Left + wnd_rect.Left,
                        wnd_rect.Width,
                        wnd_rect.Height);
                    Capture(id, wnd_rect_temp);
                }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_test_buy_code_Click(object sender, EventArgs e)
        {
            Test(1, typeof(TextBox), "600000");
        }

        private void btn_test_buy_price_Click(object sender, EventArgs e)
        {
            Test(2, typeof(TextBox), "1");
        }

        private void btn_test_buy_amount_Click(object sender, EventArgs e)
        {
            Test(3, typeof(TextBox), "10000");
        }

        private void confirm_Click(object sender, EventArgs e)
        {
            Test(4, typeof(Button), "买入下单");
        }

        private void btn_test_sale_code_Click(object sender, EventArgs e)
        {
            Test(5, typeof(TextBox), "600000");
        }

        private void btn_test_sale_price_Click(object sender, EventArgs e)
        {
            Test(6, typeof(TextBox), "1000");
        }

        private void btn_test_sale_amount_Click(object sender, EventArgs e)
        {
            Test(7, typeof(TextBox), "10000");
        }

        private void btn_test_sale_confirm_Click(object sender, EventArgs e)
        {
            Test(8, typeof(Button), "卖出下单");
        }

        private void btn_test_buy_all_Click(object sender, EventArgs e)
        {
            Test(1, typeof(TextBox), "600000");
            Test(2, typeof(TextBox), "1");
            Test(3, typeof(TextBox), "10000");
            Test(4, typeof(Button), "买入下单");
        }

        private void btn_test_sale_all_Click(object sender, EventArgs e)
        {
            Test(5, typeof(TextBox), "600000");
            Test(6, typeof(TextBox), "1000");
            Test(7, typeof(TextBox), "10000");
            Test(8, typeof(Button), "卖出下单");
        }

        private void btn_apply_Click(object sender, EventArgs e)
        {
            if (process == null)
            {
                MessageBox.Show("Process is null!", "Capture");
                return;
            }
            if (RectList == null || RectList.Count < 8)
            {
                MessageBox.Show("RectList is null!", "Capture");
                return;
            }
            if (TaskManager.Instance.Configuration == null ||
                TaskManager.Instance.Configuration.TradeInfoEntity == null)
            {
                MessageBox.Show("Configuration is null!", "Capture");
                return;
            }

            // Program
            TaskManager.Instance.Configuration.TradeInfoEntity.ApplicationName = txt_program.Text;
            TaskManager.Instance.Configuration.TradeInfoEntity.Application = process;

            // Buy Button
            TaskManager.Instance.Configuration.TradeInfoEntity.BuyButton = this.PointList[11];
            // Sell Button
            TaskManager.Instance.Configuration.TradeInfoEntity.SellButton = this.PointList[12];
            // Deal Button
            TaskManager.Instance.Configuration.TradeInfoEntity.DealButton = this.PointList[13];
            // Balance Button
            TaskManager.Instance.Configuration.TradeInfoEntity.BalanceButton = this.PointList[14];
            // Freah Button
            TaskManager.Instance.Configuration.TradeInfoEntity.FreshButton = this.PointList[15];

            WindowRect Rect;
            RECT main = new RECT();
            NativeMethods.GetWindowRect(process.MainWindowHandle, ref main);

            // Buy Info
            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.CodeRect;
            Rect.SetWHnd(RectList[1].WHnd);
            Rect.SetCenterPoint(RectList[1].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.CodeRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.PriceRect;
            Rect.SetWHnd(RectList[2].WHnd);
            Rect.SetCenterPoint(RectList[2].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.PriceRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.AmountRect;
            Rect.SetWHnd(RectList[3].WHnd);
            Rect.SetCenterPoint(RectList[3].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.AmountRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.ConfirmRect;
            Rect.SetWHnd(RectList[4].WHnd);
            Rect.SetCenterPoint(RectList[4].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.ConfirmRect = Rect;

            // Sale Info
            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.CodeRect;
            Rect.SetWHnd(RectList[5].WHnd);
            Rect.SetCenterPoint(RectList[5].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.CodeRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.PriceRect;
            Rect.SetWHnd(RectList[6].WHnd);
            Rect.SetCenterPoint(RectList[6].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.PriceRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.AmountRect;
            Rect.SetWHnd(RectList[7].WHnd);
            Rect.SetCenterPoint(RectList[7].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.AmountRect = Rect;

            Rect = TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.ConfirmRect;
            Rect.SetWHnd(RectList[8].WHnd);
            Rect.SetCenterPoint(RectList[8].GetCenterPoint());
            TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.ConfirmRect = Rect;

            lab_buy_info.Text = string.Format("Code:{0}, Price:{1}{2}Amount:{3}, Confirm:{4}",
                TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.CodeRect.WHnd,
                TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.PriceRect.WHnd,
                System.Environment.NewLine,
                TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.AmountRect.WHnd,
                TaskManager.Instance.Configuration.TradeInfoEntity.BuyInfo.ConfirmRect.WHnd
                );

            lab_sale_info.Text = string.Format("Code:{0}, Price:{1}{2}Amount:{3}, Confirm:{4}",
                TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.CodeRect.WHnd,
                TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.PriceRect.WHnd,
                System.Environment.NewLine,
                TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.AmountRect.WHnd,
                TaskManager.Instance.Configuration.TradeInfoEntity.SaleInfo.ConfirmRect.WHnd
                );

            MessageWindow.Show("Apply successfully!", MessageBoxButtons.OK);
        }

        private void btn_buy_capture_Click(object sender, EventArgs e)
        {
            flag = 11;
            mouse.Start();
        }

        private void btn_sell_capture_Click(object sender, EventArgs e)
        {
            flag = 12;
            mouse.Start();
        }

        private void btn_deal_capture_Click(object sender, EventArgs e)
        {
            flag = 13;
            mouse.Start();
        }

        private void btn_balance_capture_Click(object sender, EventArgs e)
        {
            flag = 14;
            mouse.Start();
        }

        private void btn_fresh_capture_Click(object sender, EventArgs e)
        {
            flag = 15;
            mouse.Start();
        }

        //private void btn_buy_Click(object sender, EventArgs e)
        //{
        //    if (process != null)
        //    {
        //        TradeAssistant.SendMessage(process.MainWindowHandle.ToInt32(), TradeAssistant.WM_KEYDOWN, TradeAssistant.VK_F8, 0);
        //    }
        //}

        //private void btn_sale_Click(object sender, EventArgs e)
        //{
        //    if (process != null)
        //    {
        //        TradeAssistant.SendMessage(process.MainWindowHandle.ToInt32(), TradeAssistant.WM_KEYDOWN, TradeAssistant.VK_F9, 0);
        //    }
        //}
    }

}
