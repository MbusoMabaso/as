namespace CMCS.Models
{
    // A central place for status string constants used across the app and views.
    public static class ClaimStatus
    {
        // When a lecturer submits a claim it will be treated as Pending/Submitted for review
        public const string Submitted = "Pending"; // controllers use Submitted to mean pending

        // After programme coordinator approves it moves to this state for Academic Manager
        public const string ApprovedByProgrammeCoordinator = "PC Approved";

        // When Academic Manager approves the claim we treat it as final Approved
        public const string ApprovedByAcademicManager = "Approved";

        public const string Rejected = "Rejected";
    }
}

