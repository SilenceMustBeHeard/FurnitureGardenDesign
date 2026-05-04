# 🌿 Furniture & Garden Design (F&GD)

[![ASP.NET](https://img.shields.io/badge/ASP.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-10.0-512BD4?logo=.net)](https://learn.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap)](https://getbootstrap.com/)
[![AI Generated](https://img.shields.io/badge/AI-Powered-FF6B6B?logo=openai)](https://leonardo.ai/)
[![3D Models](https://img.shields.io/badge/3D-Meshy.ai-4A90E2?logo=three.js)](https://meshy.ai/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 📖 Overview

**Furniture & Garden Design (F&GD)** is a full-stack ASP.NET Core MVC e-commerce platform that enables customers to order personalized furniture and garden designs. The platform leverages **AI-powered design generation**, **interactive 3D visualizations**, and a role-based approval workflow.

### Key Features

| Feature | Description |
|---------|-------------|
| 🤖 **AI-Powered Design Generation** | Generate unique furniture designs from text descriptions |
| 🎨 **AI Image Generation** | Create realistic product previews using multiple AI engines |
| 🧊 **Interactive 3D Models** | View and interact with designs in 3D space |
| 👥 **Role-Based Access** | Admin, Manager, and Customer roles with granular permissions |
| 📦 **Catalog Management** | Curated collection of approved designs for inspiration |
| 💬 **Messaging System** | In-app messaging between customers and admins |
| ⭐ **Reviews & Ratings** | Customer feedback system for catalog items |
| 🔐 **Secure Authentication** | ASP.NET Core Identity with email confirmation & password reset |
| 📸 **Reference Image Support** | Upload reference images for AI to work from |
| 🔄 **Iterative Refinement** | Request changes and AI will regenerate designs |

## 🎯 Workflow
Customer Order → Admin/Manager Review → AI Design Generation → Customer Approval → Catalog Publication
↓
Optional 3D Render Request
↓
Customer Feedback / Refinement
↓
AI Regenerates Design

## 👥 User Roles & Permissions

| Permission | 👑 Admin | 📋 Manager | 👤 Customer |
|------------|----------|------------|-------------|
| Create orders | ✅ | ✅ | ✅ |
| Track orders | ✅ | ✅ | ✅ |
| Approve/refine designs | ✅ | ✅ | ✅ |
| Request AI generation | ✅ | ✅ | ✅ |
| Request 3D rendering | ✅ | ✅ | ✅ |
| Manage all orders | ✅ | ✅ | ❌ |
| Manage users | ✅ | ❌ | ❌ |
| Manage categories | ✅ | ❌ | ❌ |
| Manage catalog | ✅ | ❌ | ❌ |
| Manage reviews | ✅ | ❌ | ✅ (own only) |
| Send system messages | ✅ | ❌ | ❌ |
| Contact support | ✅ | ✅ | ✅ |

## 🛠️ Technology Stack

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 10.0 | Web framework |
| **Entity Framework Core** | 10.0 | ORM & data access |
| **SQL Server** | 2022 | Relational database |
| **ASP.NET Core Identity** | 10.0 | Authentication & authorization |
| **SendGrid** | Latest | Email service (password reset) |

### AI & 3D Services
| Service | Purpose |
|---------|---------|
| **Meshy.ai** | 3D model generation from text/images |
| **Luma AI** | 3D model generation from text/references |
| **Tripo AI** | Fast 3D model generation |
| **Masterpiece Studio** | AI-powered 3D modeling |
| **Kaedim** | 2D to 3D model conversion |
| **Sloyd** | AI-generated game-ready 3D assets |
| **CSM (Common Sense Machines)** | 3D generation from images/video |
| **Leonardo.ai** | AI image generation |
| **Segmind.ai** | AI image generation & editing |
| **Playground AI** | AI design variations |
| **Stable Diffusion** | Open-source AI image generation |
| **Midjourney** | AI art generation |
| **DALL-E** | AI image generation |
| **Remini** | AI image enhancement |
| **Krikey.ai** | AI video & animation |

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| **Razor Views** | 10.0 | Server-side rendering |
| **Bootstrap** | 5.3 | Responsive UI framework |
| **Bootstrap Icons** | 1.11 | Icon library |
| **CSS Glassmorphism** | Custom | Modern UI effects |
| **Three.js** | Latest | 3D model viewer |

### External Services
| Service | Purpose |
|---------|---------|
| **Meshy.ai** | 3D model generation |
| **Luma AI** | 3D model generation |
| **Tripo AI** | Fast 3D model generation |
| **Masterpiece Studio** | AI 3D modeling |
| **Kaedim** | 2D to 3D conversion |
| **Sloyd** | AI game-ready 3D assets |
| **CSM** | 3D from images/video |
| **Leonardo.ai** | AI image generation |
| **Segmind.ai** | AI image generation |
| **Playground AI** | AI design variations |
| **Stable Diffusion** | Open-source AI image |
| **Midjourney** | AI art generation |
| **DALL-E** | AI image generation |
| **Remini** | AI image enhancement |
| **Krikey.ai** | AI video & animation |
| **SendGrid** | Transactional emails |




## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Git](https://git-scm.com/)
- [Visual Studio 2022 - 2026](https://visualstudio.microsoft.com/) (v17.14 or later) or VS Code
- (Optional) [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Installation

#### 1. Clone the Repository


git clone https://github.com/SilenceMustBeHeard/FurnitureGardenDesign.git
cd FurnitureGardenDesign



2. Configure Database Connection
Update appsettings.json in the FurnitureGardenDesign.Web project:


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FurnitureGardenDesign;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }

Apply Database Migrations ( if not presented). If you have any problems - clear the previous migration folder and start anew.


. Seed Initial Data
The application automatically seeds:

Admin user: admin@example.com / Admin123!

Manager user: manager@example.com / Manager123!

Sample categories and catalog items

System messages

6. Configure SendGrid (for Password Reset)

   dotnet user-secrets set "SendGrid:ApiKey" "YOUR_SENDGRID_API_KEY"
dotnet user-secrets set "SendGrid:FromEmail" "your-verified-email@example.com"



7. Run the Application




Docker Deployment 

# Build the image
docker build -t furniture-garden-app .

# Run the container
docker run -d -p 8080:80 --name furniture-garden furniture-garden-app

🧪 Testing
Run Unit Tests
bash
dotnet test FurnitureGardenDesign.Services.Tests/FurnitureGardenDesign.Services.Tests.csproj
Run Integration Tests
bash
dotnet test FurnitureGardenDesign.Integration.Tests/FurnitureGardenDesign.Integration.Tests.csproj
Test Coverage
bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
reportgenerator -reports:TestResults/**/coverage.opencover.xml -targetdir:CoverageReport





🔐 Login Page
<img width="1904" height="912" alt="login" src="https://github.com/user-attachments/assets/68e348e6-9fac-4412-b724-3a9c992945a5" />



📝 Contact MEssage 
<img width="1882" height="910" alt="contact message" src="https://github.com/user-attachments/assets/8911eb4f-6945-49d2-836a-7676363c02fd" />




🛠 Admin Panel
<img width="1892" height="906" alt="admin view" src="https://github.com/user-attachments/assets/e24ab638-64fd-4847-8f2e-00c885a63f9b" />

📝 Create Order
<img width="1886" height="906" alt="create order" src="https://github.com/user-attachments/assets/2e97b8e7-2b41-4d7a-9e33-c503231ab889" />





🔧 Environment Variables
Variable	Description	Required
ASPNETCORE_ENVIRONMENT	Runtime environment (Development/Production)	✅
Leonardo:ApiKey	Leonardo.ai API key for image generation	❌
Meshy:ApiKey	Meshy.ai API key for 3D generation	❌
SendGrid:ApiKey	SendGrid API key for email	❌
SendGrid:FromEmail	Verified sender email	❌
📚 API Endpoints
Public Endpoints
Method	Endpoint	Description
GET	/Catalog/CatalogIndex	View catalog
GET	/Catalog/Details/{id}	Design details
POST	/Catalog/ToggleFavorite/{id}	Favorite/unfavorite design
POST	/Catalog/AddReview	Submit review
GET	/Profile/ProxyImage	Fetch external images (CORS proxy)
Admin Endpoints
Method	Endpoint	Description
GET	/Admin/CatalogManagement/EditList	Manage catalog
GET	/Admin/UserManagement/Index	Manage users
GET	/Admin/OrdersManagement/Manage	Manage orders
GET	/Admin/ReviewManagement/EditList	Moderate reviews
POST	/Admin/DesignVariants/Create	Create design variant
POST	/Admin/DesignVariants/Send	Send proposal to customer
AI Generation Endpoints
Method	Endpoint	Description
POST	/Api/GenerateImage	Generate AI image from prompt
POST	/Api/Generate3D	Generate 3D model from prompt/image
GET	/Api/GenerationStatus/{id}	Check generation status



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

Kaedim - Convert 2D images to 3D models

Sloyd - AI-generated game-ready 3D assets

CSM (Common Sense Machines) - 3D generation from images and video

AI Image Generation
Leonardo.ai - AI image generation for design concepts

Segmind.ai - AI image generation and editing

Playground AI - AI design generation and variations

Stable Diffusion - Open-source AI image generation

Midjourney - High-quality AI art generation

DALL-E - OpenAI's image generation

Remini - AI image enhancement and upscaling

Krikey.ai - AI video and animation generation

Infrastructure
SendGrid - Email service

Bootstrap - UI framework

Three.js - 3D model rendering

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

📞 Support
For issues, questions, or contributions:

Open an issue

Email: support@furnituregarden.com

<div align="center"> Made with ❤️ by the Furniture & Garden Design Team <br/> <sub>Built with .NET 10.0 | Powered by AI & 3D Generation</sub> </div> ```







  


