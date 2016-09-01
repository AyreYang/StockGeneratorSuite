namespace StockGeneratorTradeAgent.UI
{
    partial class frm_main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnl_base = new System.Windows.Forms.Panel();
            this.pnl_info = new System.Windows.Forms.Panel();
            this.lst_info = new System.Windows.Forms.ListBox();
            this.pnl_option = new System.Windows.Forms.Panel();
            this.pnl_button = new System.Windows.Forms.Panel();
            this.tlp_button = new System.Windows.Forms.TableLayoutPanel();
            this.btn_exit = new System.Windows.Forms.Button();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.pnl_operation = new System.Windows.Forms.Panel();
            this.tlp_operation = new System.Windows.Forms.TableLayoutPanel();
            this.btn_operation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_capture = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pnl_progress = new System.Windows.Forms.Panel();
            this.pgb_progress = new System.Windows.Forms.ProgressBar();
            this.pnl_base.SuspendLayout();
            this.pnl_info.SuspendLayout();
            this.pnl_option.SuspendLayout();
            this.pnl_button.SuspendLayout();
            this.tlp_button.SuspendLayout();
            this.pnl_operation.SuspendLayout();
            this.tlp_operation.SuspendLayout();
            this.pnl_progress.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_base
            // 
            this.pnl_base.Controls.Add(this.pnl_info);
            this.pnl_base.Controls.Add(this.pnl_option);
            this.pnl_base.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_base.Location = new System.Drawing.Point(0, 0);
            this.pnl_base.Name = "pnl_base";
            this.pnl_base.Size = new System.Drawing.Size(364, 412);
            this.pnl_base.TabIndex = 0;
            // 
            // pnl_info
            // 
            this.pnl_info.Controls.Add(this.lst_info);
            this.pnl_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_info.Location = new System.Drawing.Point(0, 92);
            this.pnl_info.Name = "pnl_info";
            this.pnl_info.Size = new System.Drawing.Size(364, 320);
            this.pnl_info.TabIndex = 1;
            // 
            // lst_info
            // 
            this.lst_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lst_info.Font = new System.Drawing.Font("Nina", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lst_info.FormattingEnabled = true;
            this.lst_info.HorizontalScrollbar = true;
            this.lst_info.ItemHeight = 17;
            this.lst_info.Location = new System.Drawing.Point(0, 0);
            this.lst_info.Name = "lst_info";
            this.lst_info.Size = new System.Drawing.Size(364, 320);
            this.lst_info.TabIndex = 0;
            // 
            // pnl_option
            // 
            this.pnl_option.Controls.Add(this.pnl_button);
            this.pnl_option.Controls.Add(this.pnl_progress);
            this.pnl_option.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_option.Location = new System.Drawing.Point(0, 0);
            this.pnl_option.Name = "pnl_option";
            this.pnl_option.Size = new System.Drawing.Size(364, 92);
            this.pnl_option.TabIndex = 0;
            // 
            // pnl_button
            // 
            this.pnl_button.Controls.Add(this.tlp_button);
            this.pnl_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_button.Location = new System.Drawing.Point(0, 0);
            this.pnl_button.Name = "pnl_button";
            this.pnl_button.Size = new System.Drawing.Size(364, 67);
            this.pnl_button.TabIndex = 2;
            // 
            // tlp_button
            // 
            this.tlp_button.ColumnCount = 5;
            this.tlp_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_button.Controls.Add(this.btn_exit, 3, 0);
            this.tlp_button.Controls.Add(this.btn_start, 0, 0);
            this.tlp_button.Controls.Add(this.btn_stop, 1, 0);
            this.tlp_button.Controls.Add(this.btn_cancel, 2, 0);
            this.tlp_button.Controls.Add(this.pnl_operation, 4, 0);
            this.tlp_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_button.Location = new System.Drawing.Point(0, 0);
            this.tlp_button.Name = "tlp_button";
            this.tlp_button.RowCount = 1;
            this.tlp_button.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_button.Size = new System.Drawing.Size(364, 67);
            this.tlp_button.TabIndex = 3;
            // 
            // btn_exit
            // 
            this.btn_exit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_exit.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_exit.Location = new System.Drawing.Point(219, 3);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(66, 61);
            this.btn_exit.TabIndex = 4;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            // 
            // btn_start
            // 
            this.btn_start.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_start.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_start.Location = new System.Drawing.Point(3, 3);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(66, 61);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            // 
            // btn_stop
            // 
            this.btn_stop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_stop.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_stop.Location = new System.Drawing.Point(75, 3);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(66, 61);
            this.btn_stop.TabIndex = 1;
            this.btn_stop.Text = "Stop";
            this.btn_stop.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_cancel.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cancel.Location = new System.Drawing.Point(147, 3);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(66, 61);
            this.btn_cancel.TabIndex = 1;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // pnl_operation
            // 
            this.pnl_operation.Controls.Add(this.tlp_operation);
            this.pnl_operation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_operation.Location = new System.Drawing.Point(291, 3);
            this.pnl_operation.Name = "pnl_operation";
            this.pnl_operation.Size = new System.Drawing.Size(70, 61);
            this.pnl_operation.TabIndex = 5;
            // 
            // tlp_operation
            // 
            this.tlp_operation.ColumnCount = 2;
            this.tlp_operation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlp_operation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlp_operation.Controls.Add(this.btn_operation, 1, 1);
            this.tlp_operation.Controls.Add(this.label1, 0, 0);
            this.tlp_operation.Controls.Add(this.btn_capture, 1, 0);
            this.tlp_operation.Controls.Add(this.label2, 0, 1);
            this.tlp_operation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_operation.Location = new System.Drawing.Point(0, 0);
            this.tlp_operation.Name = "tlp_operation";
            this.tlp_operation.RowCount = 2;
            this.tlp_operation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_operation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_operation.Size = new System.Drawing.Size(70, 61);
            this.tlp_operation.TabIndex = 2;
            // 
            // btn_operation
            // 
            this.btn_operation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_operation.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_operation.Location = new System.Drawing.Point(31, 33);
            this.btn_operation.Name = "btn_operation";
            this.btn_operation.Size = new System.Drawing.Size(36, 25);
            this.btn_operation.TabIndex = 4;
            this.btn_operation.Text = "▶";
            this.btn_operation.UseVisualStyleBackColor = true;
            this.btn_operation.Click += new System.EventHandler(this.btn_operation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "捕捉";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_capture
            // 
            this.btn_capture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_capture.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_capture.Location = new System.Drawing.Point(31, 3);
            this.btn_capture.Name = "btn_capture";
            this.btn_capture.Size = new System.Drawing.Size(36, 24);
            this.btn_capture.TabIndex = 2;
            this.btn_capture.Text = "▶";
            this.btn_capture.UseVisualStyleBackColor = true;
            this.btn_capture.Click += new System.EventHandler(this.btn_capture_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 31);
            this.label2.TabIndex = 1;
            this.label2.Text = "撤单";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl_progress
            // 
            this.pnl_progress.Controls.Add(this.pgb_progress);
            this.pnl_progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_progress.Location = new System.Drawing.Point(0, 67);
            this.pnl_progress.Name = "pnl_progress";
            this.pnl_progress.Size = new System.Drawing.Size(364, 25);
            this.pnl_progress.TabIndex = 1;
            // 
            // pgb_progress
            // 
            this.pgb_progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgb_progress.Location = new System.Drawing.Point(0, 0);
            this.pgb_progress.Name = "pgb_progress";
            this.pgb_progress.Size = new System.Drawing.Size(364, 25);
            this.pgb_progress.TabIndex = 0;
            // 
            // frm_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 412);
            this.Controls.Add(this.pnl_base);
            this.Name = "frm_main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frm_main";
            this.Load += new System.EventHandler(this.frm_main_Load);
            this.pnl_base.ResumeLayout(false);
            this.pnl_info.ResumeLayout(false);
            this.pnl_option.ResumeLayout(false);
            this.pnl_button.ResumeLayout(false);
            this.tlp_button.ResumeLayout(false);
            this.pnl_operation.ResumeLayout(false);
            this.tlp_operation.ResumeLayout(false);
            this.tlp_operation.PerformLayout();
            this.pnl_progress.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_base;
        private System.Windows.Forms.Panel pnl_info;
        private System.Windows.Forms.Panel pnl_option;
        private System.Windows.Forms.Panel pnl_button;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Panel pnl_progress;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.ProgressBar pgb_progress;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.ListBox lst_info;
        private System.Windows.Forms.Button btn_capture;
        private System.Windows.Forms.TableLayoutPanel tlp_button;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Panel pnl_operation;
        private System.Windows.Forms.TableLayoutPanel tlp_operation;
        private System.Windows.Forms.Button btn_operation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}