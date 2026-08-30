namespace Scholar.Constants
{
    public static class MsgKey
    {
        public static class Validation
        {
            public static string Required(string entity) => $"{entity} is required.";
            public static string Invalid(string entity) => $"Invalid {entity}.";
            public static string AlreadyExists(string entity) => $"{entity} already exists.";
            public static string Select(string entity) => $"Please select a {entity.ToLowerInvariant()}.";
        }

        public static class Success
        {
            public static string Created(string entity) => $"{entity} created successfully.";
            public static string Updated(string entity) => $"{entity} updated successfully.";
            public static string Deleted(string entity) => $"{entity} deleted successfully.";
            public static string Saved(string entity) => $"{entity} saved successfully.";
        }

        public static class Error
        {
            public static string NotFound(string entity) => $"{entity} not found.";
            public static string SaveFailed(string entity) => $"Failed to save {entity.ToLowerInvariant()}.";
            public static string DeleteFailed(string entity) => $"Failed to delete {entity.ToLowerInvariant()}.";
        }
    }
}
