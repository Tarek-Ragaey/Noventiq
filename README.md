**Project Setup Instructions
Backend (.NET Application)**

**Configure Database Connection**
-Open the appsettings.json file.
-Update the ConnectionStrings section with your desired database server details.

**Apply Database Migrations**

Open the Package Manager Console in Visual Studio.
Run the following command:
update-database

This will create the necessary tables and seed initial data.

**Default Admin Credentials**

After seeding, you can log in with the following credentials:
Email: Admin@Admin.com
Password: P@ssw0rd123

Start the project using Visual Studio.

All APIs will be accessible and documented via Swagger.

**Frontend (Bonus React App)**

A simple React-based admin panel is provided (created with the help of Cursor).

To run it:
Open a terminal.

Navigate to the project directory:
cd noventiq.admin

Start the React app:
npm start
The app includes a basic login screen and dashboard that interfaces with the provided APIs.

