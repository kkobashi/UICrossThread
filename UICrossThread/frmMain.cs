using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KobashiComputing.TaskLog;

namespace UICrossThread
{
    public delegate void ThreadDataDelegate();
    public partial class frmMain : Form
    {
        private bool bRunningDataThread;
        private Thread threadData;
        private Thread threadTaskLog;
        private TaskLog taskLogController;
        
        public frmMain()
        {
            InitializeComponent();

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.AllowUserToAddRows = false;

            taskLogController = new TaskLog();
            taskLogController.AddView(new TaskLogView(dataGridView));

            bRunningDataThread = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            threadTaskLog = new Thread(new ThreadStart(taskLogController.Run));
            threadTaskLog.Start();

            ThreadDataDelegate delegateDataThread = new ThreadDataDelegate(ThreadDataRun);
            threadData = new Thread(new ThreadStart(delegateDataThread));
            threadData.Start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            taskLogController.Stop();
            bRunningDataThread = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void ThreadDataRun()
        {
            bRunningDataThread = true;
            while (bRunningDataThread)
            {
                taskLogController.AddTaskLogItem("hello");
                Thread.Sleep(500);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            List<TaskLogView> views;
            views = taskLogController.GetViews();
            if (null == views)
            {
                return;
            }
            foreach (TaskLogView view in views)
            {
                view.ClearGrid();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStop.Enabled)
            {
                MessageBox.Show("Please stop all running threads before closing application");
                e.Cancel = true;
                return;
            }
            e.Cancel = false;
        }
    }
}
