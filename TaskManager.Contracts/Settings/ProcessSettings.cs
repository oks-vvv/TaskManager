using System.IO;
using TaskManager.Contracts.Validation;

namespace TaskManager.Settings
{
    public class ProcessSettings : IValidated
    {
        public ProcessSettings()
        {
            ProcessName = "/bin/bash";
            Arguments = string.Empty;
            WorkingDirectory = string.Empty;
        }

        public string ProcessName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public bool CreateNoWindow { get; set; }
        public bool UseShellExecute { get; set; }

        public IValidateResult Validate()
        {
            return string.IsNullOrWhiteSpace(ProcessName) || !File.Exists(ProcessName)
                ? ValidateResult.Invalid("Process name has to be an existing file")
                : ValidateResult.Valid();
        }

    }
}
