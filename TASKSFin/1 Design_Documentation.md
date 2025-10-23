# Contract Monthly Claim System (CMCS) - Design Documentation

## 1. System Overview

The Contract Monthly Claim System (CMCS) is a web-based application designed to streamline the process of submitting and approving monthly claims for Independent Contractor (IC) lecturers. The system facilitates the complex workflow involving claim submission, document upload, and multi-level approval processes.

## 2. Design Choices and Rationale

### 2.1 Architecture Choice: ASP.NET Core MVC

**Choice**: ASP.NET Core MVC with Entity Framework Core
**Rationale**:
- **Separation of Concerns**: MVC pattern provides clear separation between data models, business logic, and presentation
- **Scalability**: ASP.NET Core offers excellent performance and scalability for web applications
- **Cross-platform**: Runs on Windows, Linux, and macOS
- **Built-in Security**: Provides authentication, authorization, and data protection features
- **Entity Framework**: Simplifies database operations with Code-First approach

### 2.2 Database Design Choices

#### 2.2.1 Entity Structure

**User Entity**:
- **Purpose**: Centralized authentication and authorization
- **Design Choice**: Separate from Lecturer entity to support multiple user types
- **Rationale**: Allows for role-based access control (Academic Manager, HR, Programme Coordinator)

**Lecturer Entity**:
- **Purpose**: Represents Independent Contractor lecturers
- **Design Choice**: Minimal fields (Name, Email) for simplicity
- **Rationale**: Focuses on essential identification information

**Claim Entity**:
- **Purpose**: Core business entity representing monthly claims
- **Design Choice**: Includes status tracking and relationship to Lecturer
- **Rationale**: Supports workflow management and audit trails

**ClaimApproval Entity**:
- **Purpose**: Tracks approval workflow
- **Design Choice**: Separate entity for approval history
- **Rationale**: Allows multiple approval levels and maintains audit trail

**Document Entity**:
- **Purpose**: Stores supporting documents for claims
- **Design Choice**: File path storage with claim relationship
- **Rationale**: Enables document management and claim verification

#### 2.2.2 Relationship Design

**One-to-Many Relationships**:
- Lecturer → Claims: One lecturer can submit multiple monthly claims
- Claim → ClaimApprovals: One claim can have multiple approval records (for different approvers)
- Claim → Documents: One claim can have multiple supporting documents

**Rationale**: These relationships support the business workflow where:
- Lecturers submit claims monthly
- Claims go through multiple approval levels
- Claims require supporting documentation

### 2.3 Security Considerations

**Authentication**: User-based authentication with role-based access
**Authorization**: Role-based permissions (Lecturer, Academic Manager, HR, Programme Coordinator)
**Data Protection**: Password hashing and secure file upload handling

### 2.4 User Interface Design

**Choice**: Responsive web interface using Bootstrap and modern CSS
**Rationale**:
- **Accessibility**: Works across different devices and browsers
- **User Experience**: Intuitive navigation and clear information hierarchy
- **Maintainability**: Standard web technologies for easy updates

## 3. Database Structure Analysis

### 3.1 Core Entities

#### User Table
- **Primary Key**: UserID (Auto-increment)
- **Purpose**: System authentication and authorization
- **Key Fields**: Username, Password, Role
- **Constraints**: Username and Password are required

#### Lecturer Table
- **Primary Key**: LecturerID (Auto-increment)
- **Purpose**: Lecturer information management
- **Key Fields**: Name, Email
- **Constraints**: All fields are optional for flexibility

#### Claim Table
- **Primary Key**: ClaimID (Auto-increment)
- **Foreign Key**: LecturerID (references Lecturer.LecturerID)
- **Purpose**: Monthly claim management
- **Key Fields**: DateSubmitted, TotalHours, Status
- **Constraints**: All fields except Lecturer relationship are required

#### ClaimApproval Table
- **Primary Key**: ApprovalID (Auto-increment)
- **Foreign Key**: ClaimID (references Claim.ClaimID)
- **Purpose**: Approval workflow tracking
- **Key Fields**: ApprovalDate, ApprovedBy, IsApproved, Notes
- **Constraints**: ClaimID is required

#### Document Table
- **Primary Key**: DocumentID (Auto-increment)
- **Foreign Key**: ClaimID (references Claim.ClaimID)
- **Purpose**: Supporting document management
- **Key Fields**: FileName, FilePath
- **Constraints**: ClaimID is required

### 3.2 Data Integrity

**Referential Integrity**: Foreign key constraints ensure data consistency
**Business Rules**: Status validation, date validation, and required field constraints
**Audit Trail**: Approval history and submission timestamps

## 4. Assumptions and Constraints

### 4.1 Business Assumptions

1. **Monthly Claims**: Claims are submitted on a monthly basis
2. **Multi-level Approval**: Claims require approval from Programme Coordinator and Academic Manager
3. **Document Support**: Claims require supporting documents for verification
4. **Role-based Access**: Different user roles have different system permissions
5. **Status Tracking**: Claims progress through defined statuses (Pending, Approved, Rejected)

### 4.2 Technical Constraints

1. **Database**: SQL Server for data persistence
2. **Framework**: ASP.NET Core 9.0
3. **Authentication**: Built-in ASP.NET Core Identity (implied)
4. **File Storage**: Local file system for document storage
5. **Browser Support**: Modern web browsers with JavaScript support

### 4.3 Design Constraints

1. **Simplicity**: Keep the initial prototype simple and functional
2. **Scalability**: Design should support future enhancements
3. **Maintainability**: Code should be well-structured and documented
4. **Security**: Basic security measures for prototype phase

## 5. Future Enhancements

1. **Email Notifications**: Automated notifications for claim status changes
2. **Advanced Reporting**: Detailed analytics and reporting features
3. **Mobile Support**: Mobile-responsive design optimization
4. **Integration**: Integration with existing HR and payroll systems
5. **Advanced Security**: Enhanced authentication and authorization features

## 6. Risk Mitigation

1. **Data Loss**: Regular database backups and version control
2. **Security**: Input validation and secure file handling
3. **Performance**: Database indexing and query optimization
4. **User Experience**: User testing and feedback incorporation
5. **Scalability**: Modular design for easy feature additions
