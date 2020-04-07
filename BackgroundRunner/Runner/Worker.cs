using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BackgroundRunner.Runner
{
    public class Worker
    {
        private BackgroundWorker _worker;
        private IOutput _writer;
        private Queue<Action> _actions = new Queue<Action>();
        public List<string> Executed { get; }
        public Worker(IOutput writer)
        {
            Executed = new List<string>();

            _writer = writer;
            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.WorkerSupportsCancellation = true;
        }

        internal void Add(Action act)
        {
            _actions.Enqueue(act);
        }

        public void Run()
        {
            if (_worker.IsBusy)
            {
                return;
            }
            _worker.RunWorkerAsync();
        }

        public void Cancel()
        {
            _worker.CancelAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                _writer.Write("Canceled");
            }
            else if (e.Error != null)
            {
                _writer.Write("Error: " + e.Error.Message);
            }
            else
            {
                _writer.Write("Done!");
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_worker.CancellationPending)
            {
                if (_actions.Count == 0)
                {
                    break;
                }
                var act = _actions.Dequeue();
                act();
                _writer.Write($"{act.Method.Name} --> Is done");
                Executed.Add(act.Method.Name);
            }
            if (_worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }
    }
}
