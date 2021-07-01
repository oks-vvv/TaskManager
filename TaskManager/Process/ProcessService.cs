using System;
using System.Diagnostics;
using TaskManager.Contracts.Task;
using TaskManager.Settings;
using SystemProcess = System.Diagnostics.Process;

namespace TaskManager.Process
{
    public class ProcessService : IProcessService
    {
        private readonly ProcessSettings processSettings;

        public ProcessService(ProcessSettings processSettings)
        {
            this.processSettings = processSettings;
        }

        public void Kill(int processId)
        {
            using var process = SystemProcess.GetProcessById(processId);

            if (process == null || process.HasExited)
            {
                return;
            }

            process.Kill(true);
        }

        public int Start(Priority priority)
        {
            using var process = new SystemProcess
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = processSettings.ProcessName,
                    WorkingDirectory = processSettings.WorkingDirectory,
                    Arguments = processSettings.Arguments,
                    CreateNoWindow = processSettings.CreateNoWindow,
                    UseShellExecute = processSettings.UseShellExecute,
                }
            };

            process.Start();

            SetPriority(priority, process);

            return process.Id;
        }

        private void SetPriority(Priority priority, SystemProcess process)
        {
            try
            {
                if (priority != Priority.Uknown)
                {
                    process.PriorityClass = ToPriorityClass(priority);
                }
            }
            catch (Exception)
            {
                process.Kill();
                throw;
            }
        }

        private ProcessPriorityClass ToPriorityClass(Priority priority)
            => priority switch
            {
                Priority.Low => ProcessPriorityClass.BelowNormal,
                Priority.High => ProcessPriorityClass.AboveNormal,
                Priority.Medium => ProcessPriorityClass.Normal,
                _ => throw new NotSupportedException($"{priority} not supported"),
            };
    }
}
