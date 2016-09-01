using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using StockGeneratorTradeAgent.Core.Configuration;
using StockGeneratorTradeAgent.Core.Tasks;
using StockGeneratorTradeAgent.Properties;

namespace StockGeneratorTradeAgent.UI
{
    public enum WorkType{
        START, STOP
    }

    public enum WorkStatusType
    {
        STARTED, STOPPED, STARTING, STOPPING
    }
    public partial class frm_main : Form
    {
        private NotifyIcon mni_notifyIconServer = null;

        private WorkType mwt_type;
        private WorkStatusType mwst_status = WorkStatusType.STOPPED;

        public frm_main()
        {
            InitializeComponent();
        }

        #region Window
        private void frm_main_Load(object sender, EventArgs e)
        {
            this.Text = System.Windows.Forms.Application.ProductName + "[@Ver]".Replace("@Ver", System.Windows.Forms.Application.ProductVersion);
            this.Icon = Resources.compass_48;
            this.Size = new System.Drawing.Size(380, 450);
            this.MaximizeBox = false;

            this.StartPosition = FormStartPosition.Manual;
            int xWidth = SystemInformation.PrimaryMonitorSize.Width;
            int yHeight = SystemInformation.PrimaryMonitorSize.Height;
            this.Location = new System.Drawing.Point(xWidth - this.Width, 0);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Resize += new EventHandler(frm_main_Resize);
            this.FormClosing += new FormClosingEventHandler(frm_main_FormClosing);

            lst_info.BackColor = pnl_info.BackColor;
            lst_info.HorizontalScrollbar = true;
            lst_info.Enabled = true;
            lst_info.Items.Clear();

            InitialNotifyIcon();

            this.btn_start.Click += new EventHandler(btn_start_Click);
            this.btn_stop.Click += new EventHandler(btn_stop_Click);
            this.btn_cancel.Click += new EventHandler(btn_cancel_Click);
            this.btn_exit.Click += new EventHandler(btn_exit_Click);

            string ls_msg = string.Empty;

            if (!Prepare(ref ls_msg))
            {
                lst_info.Items.Add("Loading failed.");
                lst_info.Items.Add(ls_msg);
            }
            else
            {
                pgb_progress.Minimum = 0;
                pgb_progress.Maximum = TaskManager.Instance.Count;
                lst_info.Items.Add("It`s ready to start.");
            }

            ShowButtons();
        }

        private bool Prepare(ref string as_msg)
        {
            string ls_root = System.Windows.Forms.Application.StartupPath;
            string CONFIG_FILE_NAME = @"config\configuration.json";
            Config.Instance.Load(new FileInfo(Path.Combine(ls_root, CONFIG_FILE_NAME)));
            if (!Config.Instance.IsReady) return false;

            TaskManager.Instance.Initial(EProgress, EComplete);
            if (!TaskManager.Instance.IsReady) return false;
            return true;
        }

        void btn_cancel_Click(object sender, EventArgs e)
        {
            //if (mbg_worker.WorkerSupportsCancellation == true)
            //{
            //    mbg_worker.CancelAsync();
            //}
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            pgb_progress.Value = pgb_progress.Minimum;
            if (TaskManager.Instance.IsReady)
            {
                mwt_type = WorkType.START;
                mwst_status = WorkStatusType.STARTING;
                ShowButtons();
                lst_info.Items.Clear();
                TaskManager.Instance.Start();
            }
            
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            pgb_progress.Value = pgb_progress.Minimum;
            if (TaskManager.Instance.IsReady)
            {
                mwt_type = WorkType.STOP;
                mwst_status = WorkStatusType.STOPPING;
                ShowButtons();
                lst_info.Items.Clear();
                TaskManager.Instance.Stop();
            }
        }

        void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Events
        private void EProgress(string id)
        {
            try
            {
                var msg = "Task({0}) {1}!";
                switch (mwt_type)
                {
                    case WorkType.START:
                        msg = string.Format(msg, id, "started");
                        break;
                    case WorkType.STOP:
                        msg = string.Format(msg, id, "stopped");
                        break;
                }
                this.Invoke(new Action(() =>
                {
                    var progress = pgb_progress.Value + 1;
                    pgb_progress.Value = (progress > pgb_progress.Maximum) ? pgb_progress.Minimum : progress;
                    lst_info.Items.Add(msg);
                }));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void EComplete(long count)
        {
            try
            {
                switch (mwst_status)
                {
                    case WorkStatusType.STARTING:
                        mwst_status = WorkStatusType.STARTED;
                        break;
                    case WorkStatusType.STOPPING:
                        mwst_status = WorkStatusType.STOPPED;
                        break;
                    default:
                        break;
                }

                this.Invoke(new Action(() =>
                {
                    pgb_progress.Value = pgb_progress.Minimum;
                    lst_info.Items.Add(string.Format("Tasks({0}) is completed!", count));

                    ShowButtons();
                }));
            }
            catch (TargetInvocationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        void frm_main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) HideWindow();
        }

        private void frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mwst_status != WorkStatusType.STOPPED)
            {
                e.Cancel = true;
                HideWindow();
                return;
            }
            else
            {
                if (MessageWindow.Show("Sure to exit?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, 
                    MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void HideWindow()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.mni_notifyIconServer.Visible = true;
        }

        private void ShowWindow()
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void InitialNotifyIcon()
        {
            mni_notifyIconServer = new System.Windows.Forms.NotifyIcon();

            mni_notifyIconServer.Icon = Resources.compass_48;
            mni_notifyIconServer.Text = this.Text;

            mni_notifyIconServer.MouseClick += new MouseEventHandler(mni_notifyIconServer_MouseClick);
        }

        private void mni_notifyIconServer_MouseClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        private void ShowButtons()
        {
            btn_start.Enabled = false;
            btn_stop.Enabled = false;
            btn_cancel.Enabled = false;
            btn_exit.Enabled = true;
            btn_capture.Enabled = false;
            btn_operation.Enabled = false;

            if (!TaskManager.Instance.IsReady) return;
            switch (mwst_status)
            {
                case WorkStatusType.STARTED:
                    btn_start.Enabled = false;
                    //CallButton(btn_start, false);
                    btn_stop.Enabled = true;
                    //CallButton(btn_stop, true);
                    btn_cancel.Enabled = false;
                    //CallButton(btn_cancel, false);
                    btn_exit.Enabled = false;
                    //CallButton(btn_exit, false);
                    btn_capture.Enabled = false;
                    //CallButton(btn_capture, false);
                    btn_operation.Enabled = true;
                    //CallButton(btn_operation, true);
                    break;
                case WorkStatusType.STOPPED:
                    btn_start.Enabled = true;
                    //CallButton(btn_start, true);
                    btn_stop.Enabled = false;
                    //CallButton(btn_stop, false);
                    btn_cancel.Enabled = false;
                    //CallButton(btn_cancel, false);
                    btn_exit.Enabled = true;
                    //CallButton(btn_exit, true);
                    btn_capture.Enabled = true;
                    //CallButton(btn_capture, true);
                    btn_operation.Enabled = false;
                    //CallButton(btn_operation, false);
                    break;
                case WorkStatusType.STARTING:
                case WorkStatusType.STOPPING:
                    btn_start.Enabled = false;
                    btn_stop.Enabled = false;
                    btn_cancel.Enabled = false;
                    btn_exit.Enabled = false;
                    btn_capture.Enabled = false;
                    btn_operation.Enabled = false;
                    break;
            }

        }
        #endregion

        private void btn_capture_Click(object sender, EventArgs e)
        {
            //var form = new frm_capture();
            
            //form.StartPosition = FormStartPosition.CenterParent;
            //form.ShowDialog();
        }

        private void btn_operation_Click(object sender, EventArgs e)
        {
            //var form = new frm_trade();

            //form.StartPosition = FormStartPosition.CenterParent;
            //form.ShowDialog();
        }

        
    }

    #region MessageWindow
    public static class MessageWindow
    {
        public static DialogResult Show(string as_msg, MessageBoxButtons ambb_buttons)
        {
            return MessageBox.Show(as_msg, System.Windows.Forms.Application.ProductName, ambb_buttons);
        }

        public static DialogResult Show(string as_msg, MessageBoxButtons ambb_buttons, MessageBoxIcon ambi_icon)
        {
            return MessageBox.Show(as_msg, System.Windows.Forms.Application.ProductName, ambb_buttons, ambi_icon);
        }

        public static DialogResult Show(string as_msg, MessageBoxButtons ambb_buttons, MessageBoxIcon ambi_icon, MessageBoxDefaultButton ambdb_button)
        {
            return MessageBox.Show(as_msg, System.Windows.Forms.Application.ProductName, ambb_buttons, ambi_icon, ambdb_button);
        }
    }
    #endregion
}
