# Campus Event Manager

## Description

Campus Event Manager is a web application designed to manage university events. It provides an intuitive interface for administrators and users:

- Administrators: Add, edit, and delete events.

- Users (students or university members): Register for events, view their registrations, and see details of available events.

The application is built using ASP.NET Core with Razor Pages, PostgreSQL as the database, and containerized using Docker Compose.

## Key Features

**For Administrators**

- Add Events: Create new events with detailed information (title, date, location, description, etc.).

- Edit Events: Update information for existing events.

- Delete Events: Remove outdated or canceled events.

- Dashboard: View all events with pagination for easier management.

**For Users**

- Sign Up: Register with an email and password.

- Login: Access their dashboard after authentication.

- Event List: Browse available events and register for them.

- My Registrations: View events they have registered for.

- Unregister: Cancel their registration if needed.

## Default Configuration

**Predefined Admin Account:**

Email: Jack.bauer@hshl.de

Password: backend

**Main Endpoints**

**Authentication**

GET /Authentication/Login : Login page.

POST /Authentication/Authenticate : Authenticate the user.

GET /Authentication/Logout : Logout the user.

GET /Authentication/Register : Registration page.

POST /Authentication/Register : Register a new user.

GET /Authentication/PasswordForgotten : Password reset page.

POST /Authentication/SendPasswordResetMail : Send password reset email.

**Admin Dashboard**

GET /Dashboard : Display all events.

GET /Dashboard/Edit/{id} : Page to edit an existing event.

GET /Dashboard/New : Page to create a new event.

POST /Dashboard/Save : Save a new or updated event.

POST /Dashboard/Delete/{id} : Delete an event.

**User Dashboard**

GET /User/UserDashboard : List of available events.

POST /User/RegisterEvent : Register for an event.

POST /User/UnregisterEvent : Unregister from an event.

GET /User/EventDetails/{id} : View event details.

GET /User/RegisteredEvents : List of events the user has registered for.

## Technologies Used

**Backend:** ASP.NET Core

**Frontend:** Razor Pages

**Database:** PostgreSQL

**Containerization:** Docker Compose

**Authentication:** Cookies with Admin and User roles

**Pagination:** Implemented for dashboards

**Author**

Developed by Messu Brinda Aurelie and as part of the Backend Development course by Professor Alexander Stuckenholz. For questions or suggestions, feel free to contact me.
