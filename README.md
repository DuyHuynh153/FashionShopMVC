# **Blvck Fashion**

Blvck Fashion is a modern e-commerce platform built with .NET 6 MVC, featuring robust authentication, VnPay payment integration, and dynamic AJAX functionality. It uses generic repositories and infrastructure configuration for scalability and maintainability. Key features include email verification, password management, and user management.

---

## **Tech Stack**
- **Framework**: .NET 6 MVC  
- **Payment Gateway**: VnPay  
- **Dynamic Interactions**: AJAX for smooth page updates without reloads  
- **User Management**: Email-based password reset and change  
- **Database**: SQL Server with custom Stored Procedures  
- **Architecture**: Implemented with **Generic Repositories** and **Infrastructure Configuration** to maximize maintainability and scalability.

---

## **Getting Started**

### **1. First-Time Setup**

#### **Update the Database**
1. Open the project in Visual Studio.
2. Update the SQL Server name and database name in the `connectionString`:
   - Located in **`appsettings.json`**.
   - Also update the database reference in **`Views/Shared/_Layout.cshtml`** (if applicable).
3. Run the following commands in the Package Manager Console:
   - **Add-Migration**:  
     ```bash
     Add-Migration [Migration_Name]
     ```
   - **Update-Database**:  
     ```bash
     Update-Database
     ```

#### **Add Initial Data and Procedures**
1. Execute the SQL scripts located in:
   - `data/FashionShopMVC.sql`  
   - `StoredPro_Trigger.sql`  
2. These scripts will set up the necessary database values and stored procedures.

---

### **2. Run the Project**
1. Build and run the solution in Visual Studio.  
2. Access the application via your local server.  

---

## **Key Features**
- **User Authentication & Authorization**:
  - Secure login and role-based access control.
- **Payment Integration**:
  - Seamless checkout using VnPay.
- **AJAX-Driven UX**:
  - No full-page reloads for better performance and user experience.
- **Email Verification**:
  - Ensure user registration integrity with email confirmation.
- **Password Management**:
  - Easily reset and change passwords via email.
- **User Management**:
  - Comprehensive admin controls for managing users.
- **Generic Repositories**:
  - Designed with reusable generic repositories to streamline data access, improving maintainability and reducing redundancy.
- **Infrastructure Configuration**:
  - Elegant and scalable configuration of services in `Program.cs` ensures seamless dependency injection and modular service setup, allowing the application to grow with ease.

---





If you have any questions or encounter issues, don’t hesitate to reach out to the project maintainers. Let’s build something amazing together! 🎉
