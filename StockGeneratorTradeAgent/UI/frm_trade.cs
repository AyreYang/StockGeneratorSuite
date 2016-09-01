using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StockGeneratorTradeAgent.Core;
using CoreLib.Foundation.Database.MSSQLSERVER;
using SGNativeEntities.Common;
using SGTasks.BusinessTasks;
using SGNativeEntities.Entities.Database.Tables;
using CoreLib.Foundation.Task.common.components;
using CoreLib.Foundation.Task.common.interfaces;
using SGTasks.Enums;

namespace StockGeneratorTradeAgent.UI
{
    public partial class frm_trade : Form
    {
        DBHelper helper = null;
        DBTBizTradeDetailEntity TradeInfo = null;
        public frm_trade()
        {
            InitializeComponent();
            this.Load += new EventHandler(frm_trade_Load);
        }

        void frm_trade_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            string ls_msg = string.Empty;
            var DBAgent = new DatabaseAccessor();
            if (DBAgent.ConfigSetting(
                TaskManager.Instance.Configuration.DatabaseInfoEntity.GetInfoDict(), 
                ref ls_msg) < CoreLib.Foundation.Status.STATUS_READY)
            {
                return;
            }
            helper = new DBHelper(DBAgent);
            Fresh();

            btn_F1.Click += new EventHandler(btn_F1_Click);
            btn_F2.Click += new EventHandler(btn_F2_Click);
            btn_F5.Click += new EventHandler(btn_F5_Click);
            btn_F10.Click += new EventHandler(btn_F10_Click);
        }

        

        

        void btn_F1_Click(object sender, EventArgs e)
        {
            if (dgv_view.SelectedRows != null && dgv_view.SelectedRows.Count > 0)
            {
                FreshTradeToView(dgv_view.SelectedRows[0]);
            }
        }

        void btn_F2_Click(object sender, EventArgs e)
        {
            if (FreshTradFromView())
            {
                // Find Charactor
                var charactors = TaskManager.Instance.CharactorList.Where<ICharactor>(chtr => chtr.GetID().Equals(TradeInfo.Code));
                Charactor charactor = null;
                if (charactors != null && charactors.Count<ICharactor>() > 0 &&
                    (charactor = charactors.ElementAt<ICharactor>(0) as Charactor) != null)
                {
                    charactor.SendDataIntoPipe(PipeType.Input, TaskID.TradeAgent.ToString(), new TransportData("Withdraw", TradeInfo));
                }
            }
            Fresh();
        }

        void btn_F5_Click(object sender, EventArgs e)
        {
            Fresh();
        }

        void btn_F10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Fresh()
        {
            DiscardTrade();
            dgv_view.DataSource = helper.GetTable_BIZ_TRADE_TD();
        }

        private void DiscardTrade()
        {
            txt_code.Text = string.Empty;
            txt_trade_type.Text = string.Empty;
            txt_trade_no.Text = string.Empty;
            txt_trade_date.Text = string.Empty;
            txt_trade_price.Text = string.Empty;
            txt_trade_amount.Text = string.Empty;
            txt_trade_per.Text = string.Empty;
            txt_trade_charge.Text = string.Empty;
        }
        private bool FreshTradeToView(DataGridViewRow row)
        {
            bool lb_ret = false;
            TradeInfo = null;
            DiscardTrade();
            if (row == null) return lb_ret;
            DBTBizTradeDetailEntity ldbt_trad = new DBTBizTradeDetailEntity();
            ldbt_trad.SetEntity(row);
            if (ldbt_trad.IsOK)
            {
                txt_code.Text = ldbt_trad.Code;
                txt_trade_type.Text = ldbt_trad.TradeType.ToString();
                txt_trade_no.Text = ldbt_trad.TradeNO;
                txt_trade_date.Text = ldbt_trad.TradeDate.ToString("yyyy-MM-dd hh:mm:ss");
                txt_trade_price.Text = ldbt_trad.TradePrice.ToString();
                txt_trade_amount.Text = ldbt_trad.TradeAmount.ToString();
                txt_trade_per.Text = ldbt_trad.TradePercentage.ToString();
                txt_trade_charge.Text = ldbt_trad.TradeCharge.ToString();
                TradeInfo = ldbt_trad;
                lb_ret = true;
            }
            return lb_ret;
        }
        private bool FreshTradFromView()
        {
            bool lb_ret = false;
            if (TradeInfo != null)
            {
                TradeInfo.TradePrice = Convert.ToDecimal(txt_trade_price.Text);
                TradeInfo.TradeAmount = Convert.ToInt32(txt_trade_amount.Text);
                TradeInfo.TradePercentage = Convert.ToDecimal(txt_trade_per.Text);
                TradeInfo.TradeCharge = Convert.ToDecimal(txt_trade_charge.Text);
                lb_ret = true;
            }
            return lb_ret;
        }
    }
}
