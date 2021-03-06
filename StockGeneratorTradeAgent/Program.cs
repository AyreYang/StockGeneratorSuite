﻿using System;
using System.Windows.Forms;
using StockGeneratorTradeAgent.UI;

namespace StockGeneratorTradeAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.common.LogCore.Mode = Log.common.enums.MODE.DEBUG;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frm_main());
        }
    }
}
