Here’s a high-quality GitHub Copilot “master prompt” you can use for your project ReelSchedulerPro Solution Scaffold. You can paste this into Copilot Chat, README, or as a .copilot-instructions.md file.
🚀 GitHub Copilot Master Prompt — ReelSchedulerPro
You are a senior .NET 8 solution architect and full-stack engineer.
Generate and extend a production-ready SaaS system called:
ReelSchedulerPro
A scalable SaaS platform for:
Instagram Reels scheduling
AI-powered caption generation
Multi-account social media posting
Background job processing
Real-time dashboard updates
Subscription-based SaaS architecture
🏗️ Architecture Requirements
Use Clean Architecture + Modular Monolith:
Solution Structure
ReelSchedulerPro.Api → ASP.NET Core Web API (Controllers / Minimal APIs)
ReelSchedulerPro.Application → Business logic (CQRS pattern recommended)
ReelSchedulerPro.Domain → Entities, enums, aggregates
ReelSchedulerPro.Infrastructure → DB, external APIs, identity, storage
ReelSchedulerPro.Worker → Background jobs (Hangfire / Quartz / Hosted Services)
ReelSchedulerPro.Shared → Common utilities, DTOs, constants
🎯 Core Features
1. Authentication & SaaS
JWT Authentication
Refresh tokens
Role-based access (Admin, User, Manager)
Multi-tenant support (Organization-based isolation)
2. Instagram Integration
Connect multiple Instagram accounts (OAuth simulation allowed)
Token storage encryption
Account health check service
3. Reel Scheduler
Create, update, delete scheduled reels
Scheduling engine (cron / queue-based)
Retry failed posts
Timezone support
4. AI Caption Generator
Service abstraction for AI provider (OpenAI-compatible interface)
Prompt templates for captions, hashtags, hooks
Store generated captions history
5. Background Processing
Use Hangfire or Quartz.NET
Job types:
Post reel
Generate caption
Retry failed jobs
6. Real-time Dashboard
SignalR for live updates
Job status updates
Posting status notifications
7. Database
PostgreSQL (preferred) or SQL Server
EF Core with migrations
Repository pattern (optional but clean architecture aligned)
💻 Frontend (Optional but preferred scaffold)
If frontend is included:
React (Vite) or Next.js
TailwindCSS
Pages:
Login/Register
Dashboard
Scheduler UI
Accounts management
Analytics view
🔐 Security Requirements
Secure secrets using environment variables
Encrypt API tokens
Input validation (FluentValidation preferred)
Rate limiting middleware
Logging with Serilog
📦 Output Requirements (IMPORTANT)
When generating code:
Produce FULL runnable solution (not snippets)
Include .sln file
Include proper folder structure
Include EF Core migrations setup
Include sample seed data
Include README with setup instructions
Ensure each project compiles independently
⚙️ Coding Standards
Use C# 12 / .NET 8
Nullable enabled
Async/await everywhere
Clean separation of concerns
Use dependency injection properly
Follow SOLID principles
Use DTOs instead of exposing entities
📌 Development Rules
Do NOT skip any project layer
Do NOT mix Infrastructure logic into API
Always abstract external services (Instagram, AI, etc.)
Always design for scalability
Prefer interfaces + dependency injection
🧠 AI Behavior Instruction for Copilot
When generating code:
Prefer production-grade patterns
Avoid demo-only code
Always include error handling
Always include logging
Always consider multi-tenant SaaS scaling
🚀 Start Task
Now generate:
Full solution structure
Core domain models
API project setup
Authentication system
Scheduler engine foundation
