using System;
using TaskManager.Contracts.Validation;

namespace TaskManager.Settings
{
    public class AppSettings : IValidated
    {
        public AddProcessStrategy AddProcessStrategy { get; set; }

        public int Capacity { get; set; }

        public IValidateResult Validate()
        {
            return Capacity < 1
                ? ValidateResult.Invalid("Capacity has to be a positive number")
                : ValidateResult.Valid();
        }
    }
}
