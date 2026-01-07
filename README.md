# Furniture\&GardenDesign (F\&GD)

# 

# Custom ASP.NET Online Store Application

# 

# Furniture\&GardenDesign (F\&GD) is a web application for ordering custom furniture and garden designs. Built with ASP.NET MVC and Individual Accounts, the platform allows customers to create personalized orders, which can then be reviewed and processed by admins or managers with AI-powered design visualization. Approved designs can also be added to a catalog for future reference.

# 

# Features

# For Customers

# 

# Registration \& Login: Customers must have an account to create and track orders.

# 

# Create Order: Each order contains:

# 

# Order name

# 

# Description

# 

# Dimensions (optional)

# 

# Reference image

# 

# Category

# 

# Design Preview: After the order is accepted by an admin or manager, an AI-generated design is created based on the description and reference image.

# 

# Approval Process: Customers can approve the design or request enhancements. This process repeats until the customer is satisfied.

# 

# Catalog Access: Customers can browse a catalog of previously approved designs for inspiration.

# 

# For Admins

# 

# Order Management: View, accept, and process all orders.

# 

# Category Management: Add, edit, or delete order categories.

# 

# User Management: Delete customer accounts if needed.

# 

# Catalog Management: Approve designs to be added to the public catalog.

# 

# For Managers

# 

# Order Management Only: Managers can view and process orders but cannot modify categories or manage users.

# 

# Order Workflow

# 

# Customer creates and submits an order with all required details.

# 

# Admin or Manager reviews the order.

# 

# AI generates an image based on the order description and reference.

# 

# The generated image is sent to the customer.

# 

# Customer can either:

# 

# Approve → order moves to execution, and optionally added to catalog

# 

# Request changes → AI refines the image until customer satisfaction

# 

# Catalog of Approved Designs

# 

# Displays previously approved orders and designs.

# 

# Available for all customers to browse.

# 

# Helps customers find inspiration or reuse popular designs.

# 

# Technologies

# 

# Backend: ASP.NET MVC with Individual Accounts

# 

# Database: Entity Framework Core

# 

# Frontend: Razor Views, Bootstrap (or Tailwind if applied)

# 

# AI Integration: Generates images based on text prompts and reference images

# 

# Roles \& Permissions

# Role	Permissions

# Admin	Manage orders, categories, users, and catalog

# Manager	Manage orders only

# Customer	Create and track orders, approve or request design revisions, 

# browse catalog

# Future Enhancements

# 

# Payment system integration

# 

# Automatic multiple design variations

# 

# Order history tracking with AI revisions

# 

# Advanced search and filter for catalog designs

