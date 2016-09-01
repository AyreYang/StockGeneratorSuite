namespace StockGeneratorTradeAgent.UI
{
    partial class frm_trade
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
            this.sc_main = new System.Windows.Forms.SplitContainer();
            this.pnl_detail = new System.Windows.Forms.Panel();
            this.txt_trade_charge = new System.Windows.Forms.TextBox();
            this.txt_trade_per = new System.Windows.Forms.TextBox();
            this.txt_trade_amount = new System.Windows.Forms.TextBox();
            this.txt_trade_price = new System.Windows.Forms.TextBox();
            this.txt_trade_date = new System.Windows.Forms.TextBox();
            this.txt_trade_no = new System.Windows.Forms.TextBox();
            this.txt_trade_type = new System.Windows.Forms.TextBox();
            this.txt_code = new System.Windows.Forms.TextBox();
            this.lab_trade_date = new System.Windows.Forms.Label();
            this.lab_per = new System.Windows.Forms.Label();
            this.lab_trade_charge = new System.Windows.Forms.Label();
            this.lab_trade_per = new System.Windows.Forms.Label();
            this.lab_trade_amount = new System.Windows.Forms.Label();
            this.lab_trade_price = new System.Windows.Forms.Label();
            this.lab_trade_no = new System.Windows.Forms.Label();
            this.lab_code = new System.Windows.Forms.Label();
            this.tlp_buttons = new System.Windows.Forms.TableLayoutPanel();
            this.btn_F1 = new System.Windows.Forms.Button();
            this.btn_F2 = new System.Windows.Forms.Button();
            this.btn_F3 = new System.Windows.Forms.Button();
            this.btn_F5 = new System.Windows.Forms.Button();
            this.btn_F10 = new System.Windows.Forms.Button();
            this.dgv_view = new System.Windows.Forms.DataGridView();
            this.sc_main.Panel1.SuspendLayout();
            this.sc_main.Panel2.SuspendLayout();
            this.sc_main.SuspendLayout();
            this.pnl_detail.SuspendLayout();
            this.tlp_buttons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_view)).BeginInit();
            this.SuspendLayout();
            // 
            // sc_main
            // 
            this.sc_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_main.Location = new System.Drawing.Point(0, 0);
            this.sc_main.Name = "sc_main";
            this.sc_main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sc_main.Panel1
            // 
            this.sc_main.Panel1.Controls.Add(this.pnl_detail);
            this.sc_main.Panel1.Controls.Add(this.tlp_buttons);
            // 
            // sc_main.Panel2
            // 
            this.sc_main.Panel2.Controls.Add(this.dgv_view);
            this.sc_main.Size = new System.Drawing.Size(466, 448);
            this.sc_main.SplitterDistance = 164;
            this.sc_main.TabIndex = 0;
            // 
            // pnl_detail
            // 
            this.pnl_detail.Controls.Add(this.txt_trade_charge);
            this.pnl_detail.Controls.Add(this.txt_trade_per);
            this.pnl_detail.Controls.Add(this.txt_trade_amount);
            this.pnl_detail.Controls.Add(this.txt_trade_price);
            this.pnl_detail.Controls.Add(this.txt_trade_date);
            this.pnl_detail.Controls.Add(this.txt_trade_no);
            this.pnl_detail.Controls.Add(this.txt_trade_type);
            this.pnl_detail.Controls.Add(this.txt_code);
            this.pnl_detail.Controls.Add(this.lab_trade_date);
            this.pnl_detail.Controls.Add(this.lab_per);
            this.pnl_detail.Controls.Add(this.lab_trade_charge);
            this.pnl_detail.Controls.Add(this.lab_trade_per);
            this.pnl_detail.Controls.Add(this.lab_trade_amount);
            this.pnl_detail.Controls.Add(this.lab_trade_price);
            this.pnl_detail.Controls.Add(this.lab_trade_no);
            this.pnl_detail.Controls.Add(this.lab_code);
            this.pnl_detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_detail.Location = new System.Drawing.Point(0, 0);
            this.pnl_detail.Name = "pnl_detail";
            this.pnl_detail.Size = new System.Drawing.Size(466, 120);
            this.pnl_detail.TabIndex = 1;
            // 
            // txt_trade_charge
            // 
            this.txt_trade_charge.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_charge.Location = new System.Drawing.Point(386, 87);
            this.txt_trade_charge.Name = "txt_trade_charge";
            this.txt_trade_charge.Size = new System.Drawing.Size(68, 27);
            this.txt_trade_charge.TabIndex = 2;
            this.txt_trade_charge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_per
            // 
            this.txt_trade_per.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_per.Location = new System.Drawing.Point(108, 87);
            this.txt_trade_per.Name = "txt_trade_per";
            this.txt_trade_per.Size = new System.Drawing.Size(69, 27);
            this.txt_trade_per.TabIndex = 2;
            this.txt_trade_per.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_amount
            // 
            this.txt_trade_amount.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_amount.Location = new System.Drawing.Point(386, 60);
            this.txt_trade_amount.Name = "txt_trade_amount";
            this.txt_trade_amount.Size = new System.Drawing.Size(68, 27);
            this.txt_trade_amount.TabIndex = 2;
            this.txt_trade_amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_price
            // 
            this.txt_trade_price.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_price.Location = new System.Drawing.Point(108, 60);
            this.txt_trade_price.Name = "txt_trade_price";
            this.txt_trade_price.Size = new System.Drawing.Size(91, 27);
            this.txt_trade_price.TabIndex = 2;
            this.txt_trade_price.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_date
            // 
            this.txt_trade_date.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_date.Location = new System.Drawing.Point(294, 33);
            this.txt_trade_date.Name = "txt_trade_date";
            this.txt_trade_date.ReadOnly = true;
            this.txt_trade_date.Size = new System.Drawing.Size(160, 27);
            this.txt_trade_date.TabIndex = 2;
            this.txt_trade_date.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_no
            // 
            this.txt_trade_no.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_no.Location = new System.Drawing.Point(94, 33);
            this.txt_trade_no.Name = "txt_trade_no";
            this.txt_trade_no.ReadOnly = true;
            this.txt_trade_no.Size = new System.Drawing.Size(105, 27);
            this.txt_trade_no.TabIndex = 2;
            this.txt_trade_no.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_trade_type
            // 
            this.txt_trade_type.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_trade_type.Location = new System.Drawing.Point(411, 6);
            this.txt_trade_type.Name = "txt_trade_type";
            this.txt_trade_type.ReadOnly = true;
            this.txt_trade_type.Size = new System.Drawing.Size(43, 27);
            this.txt_trade_type.TabIndex = 1;
            this.txt_trade_type.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_code
            // 
            this.txt_code.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_code.Location = new System.Drawing.Point(68, 6);
            this.txt_code.Name = "txt_code";
            this.txt_code.ReadOnly = true;
            this.txt_code.Size = new System.Drawing.Size(75, 27);
            this.txt_code.TabIndex = 1;
            // 
            // lab_trade_date
            // 
            this.lab_trade_date.AutoSize = true;
            this.lab_trade_date.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_date.Location = new System.Drawing.Point(205, 36);
            this.lab_trade_date.Name = "lab_trade_date";
            this.lab_trade_date.Size = new System.Drawing.Size(87, 19);
            this.lab_trade_date.TabIndex = 0;
            this.lab_trade_date.Text = "TradeDate:";
            // 
            // lab_per
            // 
            this.lab_per.AutoSize = true;
            this.lab_per.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_per.Location = new System.Drawing.Point(173, 90);
            this.lab_per.Name = "lab_per";
            this.lab_per.Size = new System.Drawing.Size(26, 19);
            this.lab_per.TabIndex = 0;
            this.lab_per.Text = "%";
            // 
            // lab_trade_charge
            // 
            this.lab_trade_charge.AutoSize = true;
            this.lab_trade_charge.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_charge.Location = new System.Drawing.Point(276, 90);
            this.lab_trade_charge.Name = "lab_trade_charge";
            this.lab_trade_charge.Size = new System.Drawing.Size(104, 19);
            this.lab_trade_charge.TabIndex = 0;
            this.lab_trade_charge.Text = "TradeCharge:";
            // 
            // lab_trade_per
            // 
            this.lab_trade_per.AutoSize = true;
            this.lab_trade_per.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_per.Location = new System.Drawing.Point(12, 90);
            this.lab_trade_per.Name = "lab_trade_per";
            this.lab_trade_per.Size = new System.Drawing.Size(92, 19);
            this.lab_trade_per.TabIndex = 0;
            this.lab_trade_per.Text = "TradePrctg:";
            // 
            // lab_trade_amount
            // 
            this.lab_trade_amount.AutoSize = true;
            this.lab_trade_amount.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_amount.Location = new System.Drawing.Point(270, 63);
            this.lab_trade_amount.Name = "lab_trade_amount";
            this.lab_trade_amount.Size = new System.Drawing.Size(110, 19);
            this.lab_trade_amount.TabIndex = 0;
            this.lab_trade_amount.Text = "TradeAmount:";
            // 
            // lab_trade_price
            // 
            this.lab_trade_price.AutoSize = true;
            this.lab_trade_price.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_price.Location = new System.Drawing.Point(12, 63);
            this.lab_trade_price.Name = "lab_trade_price";
            this.lab_trade_price.Size = new System.Drawing.Size(90, 19);
            this.lab_trade_price.TabIndex = 0;
            this.lab_trade_price.Text = "TradePrice:";
            // 
            // lab_trade_no
            // 
            this.lab_trade_no.AutoSize = true;
            this.lab_trade_no.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_trade_no.Location = new System.Drawing.Point(12, 36);
            this.lab_trade_no.Name = "lab_trade_no";
            this.lab_trade_no.Size = new System.Drawing.Size(76, 19);
            this.lab_trade_no.TabIndex = 0;
            this.lab_trade_no.Text = "TradeNO:";
            // 
            // lab_code
            // 
            this.lab_code.AutoSize = true;
            this.lab_code.Font = new System.Drawing.Font("Nina", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_code.Location = new System.Drawing.Point(12, 9);
            this.lab_code.Name = "lab_code";
            this.lab_code.Size = new System.Drawing.Size(50, 19);
            this.lab_code.TabIndex = 0;
            this.lab_code.Text = "Code:";
            // 
            // tlp_buttons
            // 
            this.tlp_buttons.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_buttons.ColumnCount = 10;
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_buttons.Controls.Add(this.btn_F1, 0, 0);
            this.tlp_buttons.Controls.Add(this.btn_F2, 1, 0);
            this.tlp_buttons.Controls.Add(this.btn_F3, 2, 0);
            this.tlp_buttons.Controls.Add(this.btn_F5, 4, 0);
            this.tlp_buttons.Controls.Add(this.btn_F10, 9, 0);
            this.tlp_buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlp_buttons.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlp_buttons.Location = new System.Drawing.Point(0, 120);
            this.tlp_buttons.Name = "tlp_buttons";
            this.tlp_buttons.RowCount = 1;
            this.tlp_buttons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_buttons.Size = new System.Drawing.Size(466, 44);
            this.tlp_buttons.TabIndex = 0;
            // 
            // btn_F1
            // 
            this.btn_F1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_F1.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_F1.Location = new System.Drawing.Point(4, 4);
            this.btn_F1.Name = "btn_F1";
            this.btn_F1.Size = new System.Drawing.Size(39, 36);
            this.btn_F1.TabIndex = 0;
            this.btn_F1.Text = "F1\r\n引用";
            this.btn_F1.UseVisualStyleBackColor = true;
            // 
            // btn_F2
            // 
            this.btn_F2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_F2.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_F2.Location = new System.Drawing.Point(50, 4);
            this.btn_F2.Name = "btn_F2";
            this.btn_F2.Size = new System.Drawing.Size(39, 36);
            this.btn_F2.TabIndex = 1;
            this.btn_F2.Text = "F2\r\n撤单";
            this.btn_F2.UseVisualStyleBackColor = true;
            // 
            // btn_F3
            // 
            this.btn_F3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_F3.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_F3.Location = new System.Drawing.Point(96, 4);
            this.btn_F3.Name = "btn_F3";
            this.btn_F3.Size = new System.Drawing.Size(39, 36);
            this.btn_F3.TabIndex = 2;
            this.btn_F3.Text = "F3\r\n改单";
            this.btn_F3.UseVisualStyleBackColor = true;
            // 
            // btn_F5
            // 
            this.btn_F5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_F5.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_F5.Location = new System.Drawing.Point(188, 4);
            this.btn_F5.Name = "btn_F5";
            this.btn_F5.Size = new System.Drawing.Size(39, 36);
            this.btn_F5.TabIndex = 3;
            this.btn_F5.Text = "F5\r\n刷新";
            this.btn_F5.UseVisualStyleBackColor = true;
            // 
            // btn_F10
            // 
            this.btn_F10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_F10.Font = new System.Drawing.Font("Nina", 9F);
            this.btn_F10.Location = new System.Drawing.Point(418, 4);
            this.btn_F10.Name = "btn_F10";
            this.btn_F10.Size = new System.Drawing.Size(44, 36);
            this.btn_F10.TabIndex = 4;
            this.btn_F10.Text = "F10\r\n退出";
            this.btn_F10.UseVisualStyleBackColor = true;
            // 
            // dgv_view
            // 
            this.dgv_view.AllowUserToAddRows = false;
            this.dgv_view.AllowUserToDeleteRows = false;
            this.dgv_view.AllowUserToOrderColumns = true;
            this.dgv_view.AllowUserToResizeRows = false;
            this.dgv_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_view.Location = new System.Drawing.Point(0, 0);
            this.dgv_view.MultiSelect = false;
            this.dgv_view.Name = "dgv_view";
            this.dgv_view.ReadOnly = true;
            this.dgv_view.RowTemplate.Height = 23;
            this.dgv_view.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_view.Size = new System.Drawing.Size(466, 280);
            this.dgv_view.TabIndex = 0;
            // 
            // frm_trade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 448);
            this.Controls.Add(this.sc_main);
            this.Name = "frm_trade";
            this.Text = "frm_trade";
            this.sc_main.Panel1.ResumeLayout(false);
            this.sc_main.Panel2.ResumeLayout(false);
            this.sc_main.ResumeLayout(false);
            this.pnl_detail.ResumeLayout(false);
            this.pnl_detail.PerformLayout();
            this.tlp_buttons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_view)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer sc_main;
        private System.Windows.Forms.TableLayoutPanel tlp_buttons;
        public System.Windows.Forms.DataGridView dgv_view;
        private System.Windows.Forms.Panel pnl_detail;
        private System.Windows.Forms.TextBox txt_trade_no;
        private System.Windows.Forms.TextBox txt_code;
        private System.Windows.Forms.Label lab_trade_no;
        private System.Windows.Forms.Label lab_code;
        private System.Windows.Forms.TextBox txt_trade_date;
        private System.Windows.Forms.Label lab_trade_date;
        private System.Windows.Forms.TextBox txt_trade_type;
        private System.Windows.Forms.TextBox txt_trade_amount;
        private System.Windows.Forms.TextBox txt_trade_price;
        private System.Windows.Forms.Label lab_trade_amount;
        private System.Windows.Forms.Label lab_trade_price;
        private System.Windows.Forms.TextBox txt_trade_charge;
        private System.Windows.Forms.TextBox txt_trade_per;
        private System.Windows.Forms.Label lab_trade_charge;
        private System.Windows.Forms.Label lab_trade_per;
        private System.Windows.Forms.Label lab_per;
        private System.Windows.Forms.Button btn_F1;
        private System.Windows.Forms.Button btn_F2;
        private System.Windows.Forms.Button btn_F3;
        private System.Windows.Forms.Button btn_F5;
        private System.Windows.Forms.Button btn_F10;
    }
}