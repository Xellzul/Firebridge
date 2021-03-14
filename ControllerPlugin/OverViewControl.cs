﻿using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPlugin
{
    public partial class OverViewControl : UserControl
    {
        ControllerMain ControllerMain;
        public OverrideProgramController opc;
        private ServiceConnection serviceConnection;
        private DetailView detailView;

        private bool _selected = false;
        private bool _init = false;
        public bool Selected {
            get { return _selected; } 
            set { _selected = value;
                if (_selected)
                {
                    ConnectionManger.Instance.SelectService(serviceConnection.Id);
                    this.BackColor = Color.FromArgb(180, 166, 134);
                }
                else
                {
                    ConnectionManger.Instance.DeselectService(serviceConnection.Id);
                    this.BackColor = Color.FromArgb(225, 201, 173);
                }
            } 
        }

        public OverViewControl(ControllerMain controllerMain)
        {
            ControllerMain = controllerMain;
            detailView = new DetailView();
            InitializeComponent();

            foreach(Control control in Controls)
            {
                control.Click += Control_Click;
                control.MouseDown += Control_MouseDown;
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private void Control_Click(object sender, EventArgs e)
        {
            Selected = !Selected; 
        }

        public void Init(ServiceConnection serviceConnection)
        {
            this.serviceConnection = serviceConnection;

            serviceConnection.ConnectionStatusChanged += ServiceConnection_ConnectionStatusChanged;

            if (serviceConnection.Status == ConnectionStatus.Connected)
            {
                lock (serviceConnection)
                {
                    if (_init)
                        return;
                    _init = true;
                    Init();
                }
            }

            l_ip.Text = serviceConnection.IPAddress.ToString();
            Selected = false;
        }

        private void ServiceConnection_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (sender == null || e == null || !(sender is ServiceConnection))
                return;

            if(e.Now == ConnectionStatus.Connected)
            {
                lock (serviceConnection)
                {
                    if (_init)
                        return;
                    _init = true;
                    Init();
                }
            }
            else if (e.Now == ConnectionStatus.Disconnected)
            {
                this.Parent.Invoke((Action)(() => {
                    this.Parent?.Controls.Remove(this);
                }));
            }


        }

        private void Init()
        {
            opc = new OverrideProgramController();
            opc.ImageRecieved += ClientProgram_ImageRecieved;

            serviceConnection.StartProgram( typeof(OverrideProgram), IIntegrityLevel.Medium, uint.MaxValue, ControllerMain.AssemblyData, null, opc);

            this.Invoke((Action)(() => {
                l_connecting.Hide();
            }));
        }

        private void ClientProgram_ImageRecieved(object sender, ImageRecievedEventArgs e)
        {
            var img = this.imagePanel.BackgroundImage;
            this.imagePanel.BackgroundImage = e.Image;
            detailView.p_screenshot.BackgroundImage = e.Image;
            img?.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void toolsPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Selected = !Selected;
        }

        private void b_detail_Click(object sender, EventArgs e)
        {
            detailView.Show();
            detailView.Focus();
        }
    }
}