﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient.Views
{
    public partial class textMessageBox : Form
    {
        public bool Success = false;
        public textMessageBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Success = true;
            this.Close();
        }
    }
}
