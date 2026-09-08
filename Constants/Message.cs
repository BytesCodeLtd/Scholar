namespace Scholar.Constants
{
    public static class Message
    {
        // Authentication
        public const string InvalidCredentials = "Invalid email or password.";

        // Login validation
        public const string EmailRequired = "Email is required.";
        public const string EmailInvalid = "Enter a valid email address.";
        public const string PasswordRequired = "Password is required.";

        // User creation validation
        public const string FullNameRequired = "Full name is required.";
        public const string InstituteRequired = "Institute name is required.";
        public const string InstituteExists = "An institute with this name already exists.";
        public const string RoleRequired = "Please select a role.";

        // Student validation
        public const string ClassRequired = "Please select a class.";
        public const string PhoneInvalid = "Enter a valid phone number.";

        // Logo upload validation
        public const string LogoInvalidType = "Logo must be a PNG, JPG, WEBP, or SVG image.";
        public const string LogoTooLarge = "Logo is too large. Maximum size is 2 MB.";

        // Past paper upload validation
        public const string PastPaperFileRequired = "Choose a PDF file to upload.";
        public const string PastPaperInvalidType = "Past paper must be a PDF file.";
        public const string PastPaperTooLarge = "File is too large. Maximum size is 10 MB.";

        // Attendance
        public const string AccountNotLinkedToInstitute = "Your account is not linked to an institute.";
        public const string SelectClassToMark = "Pick a class before marking the whole roster.";
    }
}
