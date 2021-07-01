namespace TaskManager.Contracts.Result
{
    public enum ErrorType
    {
        Failed = 0,
        AlreadyExists = 1,
        NotFound = 2,
        CapacityLimit = 3,
    }
}
