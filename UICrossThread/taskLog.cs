using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace KobashiComputing.TaskLog
{
    public delegate void UpdateViewDelegate(TaskLogItem item);

    public class TaskLogItem
    {
        public TaskLogItem(String logDate, String logMsg)
        {
            LogDate = logDate;
            LogMsg = logMsg;
        }
        public String LogDate
        {
            get; set;
        }
        public String LogMsg
        {
            get; set;
        }
    }

    public class TaskLogView
    {
        private DataGridView dgv;       

        public TaskLogView(DataGridView dataGridView)
        {
            dgv = dataGridView;
        }

        public void ClearGrid()
        {
            dgv.Rows.Clear();
        }

        public void Update(TaskLogItem item)
        {
            if (dgv.InvokeRequired)
            {
                // invoke (0) if we are on different threads
                object[] args = new object[] { item };
                UpdateViewDelegate updateViewDelegate = new UpdateViewDelegate(Update);
                dgv.Invoke(updateViewDelegate, args);
            }
            else
            {
                // Add a new row of data (0)
                DataGridViewRow row;
                dgv.Rows.Add();
                row = dgv.Rows[dgv.RowCount - 1];
                row.Cells[0].Value = item.LogDate;
                row.Cells[1].Value = item.LogMsg;

                // make last row be scrolled into view
                dgv.FirstDisplayedScrollingRowIndex = dgv.Rows.Count - 1;
            }
        }
    }

    public class TaskLog
    {
        private List<TaskLogView> views;
        private bool bRunning;
        private ConcurrentQueue<TaskLogItem> queue;

        public TaskLog()
        {
            bRunning = false;
            queue = new ConcurrentQueue<TaskLogItem>();
            views = new List<TaskLogView>();
        }
        public void AddTaskLogItem(String msg)
        {
            TaskLogItem item = new TaskLogItem(DateTime.Now.ToString(), msg);
            queue.Enqueue(item);           
        }
        public void AddView(TaskLogView view)
        {
            views.Add(view);
        }
        public List<TaskLogView> GetViews()
        {
            return (views);
        }
        public void UpdateAllViews()
        {
            TaskLogItem result;
            queue.TryDequeue(out result);
            if (null == result)
                return;
            foreach (TaskLogView view in views)
            {
                view.Update(result);
            }
        }
        public void Run()
        {
            bRunning = true;
            while (bRunning)
            {
                UpdateAllViews();
                Thread.Sleep(100);
            }
        }
        public void Stop()
        {
            bRunning = false;
        }
    }
}