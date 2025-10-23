# UML Class Diagram for Contract Monthly Claim System (CMCS)

## Database Structure

```mermaid
classDiagram
    class User {
        +int UserID
        +string Username
        +string Password
        +string Role
    }

    class Lecturer {
        +int LecturerID
        +string Name
        +string Email
    }

    class Claim {
        +int ClaimID
        +int LecturerID
        +DateTime DateSubmitted
        +double TotalHours
        +string Status
        +Lecturer Lecturer
    }

    class ClaimApproval {
        +int ApprovalID
        +int ClaimID
        +DateTime ApprovalDate
        +string ApprovedBy
        +bool IsApproved
        +string Notes
    }

    class Document {
        +int DocumentID
        +int ClaimID
        +string FileName
        +string FilePath
    }

    class DashboardViewModel {
        +int TotalClaims
        +int PendingClaims
        +int ApprovedClaims
        +int RejectedClaims
        +decimal TotalAmount
        +decimal ApprovedAmount
        +List~Claim~ RecentClaims
    }

    %% Relationships
    Lecturer ||--o{ Claim : "submits"
    Claim ||--o{ ClaimApproval : "has"
    Claim ||--o{ Document : "contains"
    Claim ||--o{ DashboardViewModel : "aggregated in"
```

## Entity Relationships

1. **User** - Base authentication entity for all system users
2. **Lecturer** - Represents independent contractor lecturers
3. **Claim** - Core entity representing monthly claims submitted by lecturers
4. **ClaimApproval** - Tracks approval workflow for claims
5. **Document** - Stores supporting documents for claims
6. **DashboardViewModel** - View model for dashboard statistics

## Key Relationships

- **One-to-Many**: Lecturer → Claims (one lecturer can submit multiple claims)
- **One-to-Many**: Claim → ClaimApprovals (one claim can have multiple approval records)
- **One-to-Many**: Claim → Documents (one claim can have multiple supporting documents)
- **Aggregation**: DashboardViewModel aggregates Claim data for reporting
