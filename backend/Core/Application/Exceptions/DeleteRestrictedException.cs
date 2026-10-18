namespace Application.Exceptions
{
    public class DeleteRestrictedException : Exception
    {
        public string EntityName { get; }
        public string DependentName { get; }

        public DeleteRestrictedException(string entityName, string dependentName)
            : base($"{entityName} cannot be deleted because it has related {dependentName} records.")
        {
            EntityName = entityName;
            DependentName = dependentName;
        }
    }
}
