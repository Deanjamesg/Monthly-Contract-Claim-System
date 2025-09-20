# Monthly Contract Claim System (CMCS)
PROG6212 - Portfolio of Evidence  
Programming 2B  
ST10378305  
Dean James Greeff  

CMCS is a web application built with ASP.NET Core MVC and Entity Framework Core that streamlines the process of monthly contract claims for university lecturers. It provides a multi-tiered, role-based system for submitting, reviewing, processing, and tracking claims, ensuring an efficient and organized workflow.

<img width="1907" height="899" alt="Screenshot 2025-09-20 103718" src="https://github.com/user-attachments/assets/c99a91de-eb8a-4f43-adb4-26eba0ba5ec7" />

<br>

## Key Features

- **Role-Based Access Control:** A secure system with distinct permissions for Lecturers, Administrators (Programme Coordinators, Academic Managers), and Human Resources (HR).
- **Claim Submission:** Lecturers can easily create new claims, providing details like hours worked, hourly rate, and supporting documentation.
- **File Uploads:** Supports uploading documents (`.pdf`, `.docx`, `.png`, `.jpg`) as evidence for claims.
- **Automated Claim Flagging:** The system automatically flags claims for manual review if they meet certain criteria (e.g., hourly rate > R500, hours worked > 80, claim amount > R15,000, or calculation mismatches).
- **Admin Review Panel:** Administrators have a dedicated dashboard to review, approve, or reject flagged claims.
- **HR Processing Workflow:** The HR department can view all approved claims, manage user accounts, and process payments.
- **Invoice Generation:** Automatically generates PDF invoices for approved and processed claims using the PdfSharp library.
- **Claim Tracking:** Both administrators and lecturers can track the status of claims (`Pending`, `Approved`, `Rejected`).

<br>

## User Roles

The application defines three distinct access levels to manage the claim lifecycle:

1.  **Lecturer (Access Level 1)**
    - Can submit new claims for hours worked.
    - Can view the status and history of their own submitted claims.

2.  **Administrator (Access Level 2)**
    - Includes roles like `ProgrammeCoordinator` and `AcademicManager`.
    - Can review any claim that has been automatically flagged by the system.
    - Can approve or reject flagged claims.
    - Has visibility over all claims in the system for tracking purposes.

3.  **Human Resources (HR) (Access Level 3)**
    - Can register new Lecturer accounts.
    - Can view and manage the details of all users in the system.
    - Can process claims that have been approved.
    - Can generate and download PDF invoices for payment processing.

<br>

## Technology Stack

- **Backend:** ASP.NET Core MVC, C#
- **Database:** Microsoft SQL Server (or other, via Entity Framework Core)
- **ORM:** Entity Framework Core
- **PDF Generation:** PdfSharp
- **Frontend:** Razor Pages (.cshtml), HTML, CSS, JavaScript

<br>

## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or the version specified in your project file)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- SQL Server or another compatible database.

<br>

### Installation

1.  **Clone the repository:**
    ```sh
    git clone [https://github.com/Deanjamesg/PROG6212-POE.git](https://github.com/Deanjamesg/PROG6212-POE.git)
    cd PROG6212-POE
    ```

2.  **Configure the database connection:**
    - Open the `appsettings.json` file.
    - The DefaultConnection should be pre-configured to use SQL Server Express LocalDB, which is installed with Visual Studio.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CMCS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```

3.  **Apply Entity Framework Migrations:**
    - Run the following commands in your terminal from the project's root directory to create the database and its tables based on the `AppDbContext`.
    ```sh
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4.  **Run the application:**
    ```sh
    dotnet run
    ```
    The application will be available at `https://localhost:7173` or a similar address.

<br>

## Usage

To make testing easy, the application includes a feature to create sample user data.

1.  **Create Test Data:**
    - Once the application is running, navigate to the home page.
    - Click the **"Create Test Data"** button. This will populate the database with one user for each role (Lecturer, Programme Coordinator, Academic Manager, HR).

2.  **Login with Test Accounts:**
    - Navigate to the Login page. You can use the quick login buttons or the credentials below:
    - **Lecturer:**
        - **Email:** `lecturer@example.com`
        - **Password:** `Lecturer`
    - **Administrator (HR):**
        - **Email:** `hr@example.com`
        - **Password:** `Admin`

3.  **Explore the Workflow:**
    - Log in as the Lecturer to submit a new claim. Try submitting one that will be flagged (e.g., claim amount over 15000).
    - Log out and log in as an administrator (`programme@example.com` or `academic@example.com`) to review and approve the flagged claim.
    - Log out and log in as HR (`hr@example.com`) to process the approved claim and generate the PDF invoice.

<br>

## Screenshots

It is recommended to add screenshots of the key application views here.

<table>
  <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/1691dd26-111e-4673-85f1-b1968cac833f" alt="User Sign In" width="100%">
      <br>
      <em>Fig 1. Sign In.</em>
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/16f79df7-a0d0-48e5-9a54-e0d683fa0505" alt="Lecturer Submitting Claim" width="100%">
      <br>
      <em>Fig 2. Submitting a Claim as a Lecturer.</em>
    </td>
  </tr>
    <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/0345ab09-e1e4-4cf7-93b1-af6570718530" alt="Lecturers' Claims Submitted" width="100%">
      <br>
      <em>Fig 3. Lecturers' view of their submitted claims.</em>
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/51da2d55-521f-4aff-9d5f-9e7012a5cae2" alt="Admin Claim Review" width="100%">
      <br>
      <em>Fig 4. Administrators reviewing flagged claims.</em>
    </td>
  </tr>
      <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/0c3db900-e86b-4764-9b4f-543e274827ce" alt="Human Resources 1" width="100%">
      <br>
      <em>Fig 5. Human Resources processing claims, and creating invoices.</em>
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/ef662409-dd8a-403f-a294-76e68dbd1551" alt="Human Resources 2" width="100%">
      <br>
      <em>Fig 6. Human Resources viewing all users and their details.</em>
    </td>
  </tr>
</table>
