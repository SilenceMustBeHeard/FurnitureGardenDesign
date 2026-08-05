🌿 Furniture & Garden Design (F&GD)

[![ASP.NET](https://img.shields.io/badge/ASP.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-10.0-512BD4?logo=.net)](https://learn.microsoft.com/en-us/ef/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)](https://www.docker.com/)
[![Render](https://img.shields.io/badge/Render-Deployed-46E3B7?logo=render)](https://render.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap)](https://getbootstrap.com/)
[![AI Powered](https://img.shields.io/badge/AI-Powered-FF6B6B?logo=openai)](https://openai.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

📖 Overview
Furniture & Garden Design (F&GD) is a full-stack ASP.NET Core MVC e-commerce platform that enables customers to order personalized furniture and garden designs. The platform leverages AI-powered design generation, interactive 3D visualizations, and a role-based approval workflow.

🔗 Live Demo: https://furnituregardendesign.onrender.com

Key Features
Feature	Description
🤖 AI-Powered Design Generation	Generate unique furniture designs from text descriptions
🎨 AI Image Generation	Create realistic product previews using multiple AI engines
🧊 Interactive 3D Models	View and interact with designs in 3D space
👥 Role-Based Access	Admin, Manager, and Customer roles with granular permissions
📦 Catalog Management	Curated collection of approved designs for inspiration
💬 Messaging System	In-app messaging between customers and admins
⭐ Reviews & Ratings	Customer feedback system for catalog items
🔐 Secure Authentication	ASP.NET Core Identity with email confirmation & password reset
📸 Reference Image Support	Upload reference images for AI to work from
🔄 Iterative Refinement	Request changes and AI will regenerate designs
🎯 Workflow
text
Customer Order → Admin/Manager Review → AI Design Generation → Customer Approval → Catalog Publication
                                                                           ↓
                                                          Optional 3D Render Request
                                                                           ↓
                                                          Customer Feedback / Refinement
                                                                           ↓
                                                          AI Regenerates Design
🛠️ Technology Stack
Backend
Technology	Version	Purpose
ASP.NET Core	10.0	Web framework
Entity Framework Core	10.0	ORM & data access
PostgreSQL	17	Relational database
Npgsql	10.0	PostgreSQL provider for EF Core
ASP.NET Core Identity	10.0	Authentication & authorization
SendGrid	Latest	Email service (password reset)
Infrastructure
Technology	Purpose
Docker	Containerization
Render	Cloud hosting & deployment
GitHub Actions	CI/CD pipeline
Docker Hub	Container registry
🚀 Getting Started
Prerequisites
.NET 10.0 SDK

PostgreSQL 17 (or use Docker)

Git

Docker Desktop (recommended)

Visual Studio 2022 (v17.14 or later) or VS Code

Quick Start with Docker (Recommended)
The easiest way to run the application:

bash
# Clone the repository
git clone https://github.com/SilenceMustBeHeard/FurnitureGardenDesign.git
cd FurnitureGardenDesign

# Create .env file with your credentials
cat > .env << EOF
POSTGRES_DB=FurnitureGardenDesign
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
EOF

# Start the application with Docker Compose
docker compose up --build
The application will be available at http://localhost:8080.

Manual Setup (Without Docker)
1. Configure PostgreSQL Connection
Update appsettings.json in the FurnitureGardenDesign.Web project:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FurnitureGardenDesign;Username=postgres;Password=your_password"
  }
}
2. Apply Database Migrations
bash
dotnet ef database update --project FurnitureGardenDesign.Data --startup-project FurnitureGardenDesign.Web
3. Configure SendGrid (for Password Reset)
bash
dotnet user-secrets set "SendGrid:ApiKey" "YOUR_SENDGRID_API_KEY"
dotnet user-secrets set "SendGrid:FromEmail" "your-verified-email@example.com"
4. Run the Application
bash
cd FurnitureGardenDesign.Web
dotnet run
🐳 Docker Deployment
Build and Run
bash
# Build the image
docker build -t fgd-app .

# Run with PostgreSQL (via docker-compose)
docker compose up -d

# Or run standalone with external PostgreSQL
docker run -d -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=fgd;Username=postgres;Password=secret" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name fgd-app fgd-app
Environment Variables
Variable	Description	Required
ASPNETCORE_ENVIRONMENT	Runtime environment (Development/Production)	✅
ConnectionStrings__DefaultConnection	PostgreSQL connection string	✅
POSTGRES_DB	Database name (for docker-compose)	✅
POSTGRES_USER	Database user (for docker-compose)	✅
POSTGRES_PASSWORD	Database password (for docker-compose)	✅
SendGrid:ApiKey	SendGrid API key for email	❌
SendGrid:FromEmail	Verified sender email	❌
🔧 CI/CD Pipeline
The project uses GitHub Actions for CI/CD:

On push to main or deployment-test:

Builds the Docker image
Pushes it to Docker Hub
Render automatically deploys the latest version
Deployment Status
Live URL: https://furnituregardendesign.onrender.com

Deployment Platform: Render.com

Database: PostgreSQL 17 (Render managed)

📸 Screenshots
Login Page
<img width="800" alt="login" src="https://github.com/user-attachments/assets/68e348e6-9fac-4412-b724-3a9c992945a5" />
Contact Message
<img width="800" alt="contact message" src="https://github.com/user-attachments/assets/8911eb4f-6945-49d2-836a-7676363c02fd" />
Admin Panel
<img width="800" alt="admin view" src="https://github.com/user-attachments/assets/e24ab638-64fd-4847-8f2e-00c885a63f9b" />
Create Order
<img width="800" alt="create order" src="https://github.com/user-attachments/assets/2e97b8e7-2b41-4d7a-9e33-c503231ab889" />
🔒 Environment Variables (Production)
For production deployment on Render, configure these environment variables:

env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...;Port=5432;SSL Mode=Require
🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/amazing-feature)

Commit your changes (git commit -m 'Add amazing feature')

Push to the branch (git push origin feature/amazing-feature)

Open a Pull Request

Coding Standards
Follow C# coding conventions

Write unit tests for new features

Update documentation accordingly

Use meaningful variable names

Target .NET 10.0 compatibility

🙏 Acknowledgments
3D Model Generation
Meshy.ai - AI-powered 3D model generation from text and images

Luma AI - 3D model generation from text and reference images

Tripo AI - Fast AI-powered 3D model generation

Masterpiece Studio - AI-powered 3D modeling and editing

AI Image Generation
Leonardo.ai - AI image generation for design concepts

Segmind.ai - AI image generation and editing

Playground AI - AI design generation and variations

Stable Diffusion - Open-source AI image generation

Infrastructure
SendGrid - Email service

Bootstrap - UI framework

Three.js - 3D model rendering

PostgreSQL - Database

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.



<div align="center"> Made with ❤️ by the Furniture & Garden Design Team <br/> <sub>Built with .NET 10.0 | PostgreSQL | Docker | Powered by AI & 3D Generation</sub> </div> ```
