# Cafe-Order-Inventory-Tracker-System
A café order and inventory system using ASP.NET Web Forms (C#) and SQL Server. Features role-based dashboards, real-time stock updates, automated order workflow, low-stock alerts, and secure DAL architecture for efficient database operations.


A full-stack web application for managing café orders and inventory.

📌 Overview

The Cafe Order & Inventory Tracker System is a database-driven web application designed to automate daily café operations. It replaces manual ordering, stock tracking, and communication with a centralized system that offers fast, error-free, and real-time management. The platform supports three user roles—Admin, Staff, and Customer—each with unique permissions and personalized dashboards. Built using ASP.NET Web Forms (C#) and SQL Server, the system demonstrates practical implementation of full-stack development, database design, and 3-layer architecture.

🎯 Key Features
🔐 Role-Based Access

Admin: Manage menu, prices, and inventory; receive low-stock alerts.

Staff: Process orders (Confirm, Complete, Cancel) and track stock levels.

Customer: Browse menu, add items to cart, and place orders with live totals.

📦 Inventory Management

Real-time stock updates

Automatic low-stock notifications

Prevention of overselling

Transaction-safe stock deduction using ADO.NET

🛒 Order Workflow

Add-to-cart system with quantity checks

Order placement with total calculation

Staff dashboards for updating order status

Automated notifications for customers

🧱 System Architecture

The application follows a structured three-layer architecture:

Presentation Layer: ASP.NET Web Forms (UI)

Business Logic Layer: Validation, workflow rules, role checks

Data Access Layer (DAL): Secure SQL operations using parameterized queries

This design ensures maintainability, security, and clean separation of responsibilities.

🛠 Technologies

Frontend: ASP.NET Web Forms, HTML, CSS

Backend: C#, ADO.NET

Database: SQL Server (tables, foreign keys, transactions)

Architecture: DAL-based modular structure

🚀 Setup Instructions

Clone the repository:

git clone https://github.com/yourusername/CafeManagementSystem.git


Open the project in Visual Studio.

Import SQL scripts into SQL Server to create database tables.

Update the web.config connection string.

Run the project using Visual Studio.

📸 Screenshots

Add images in a /Screenshots folder and reference them here.

👨‍💻 Skills Demonstrated

C#, ASP.NET Web Forms, SQL Server, database design, DAL architecture, authentication, real-time notifications, UI/UX for dashboards, debugging, teamwork, and documentation.
