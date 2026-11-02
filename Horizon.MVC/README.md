# Horizon.MVC - Unified Learning Platform Frontend

## Overview
Horizon.MVC is a unified ASP.NET Core MVC application that consolidates all frontend functionality for The Horizon Learning Platform. This single MVC application replaces the previous five separate MVCs (Chat.MVC, LearningPlatform.Mvc, Student.MVC, TeacherDashboardMvc, and LearningPlatformFrontend).

## Architecture

### Design System
The application implements the **Academic Premium** design system with:
- **Light Mode**: Warm off-white backgrounds with deep green primary colors
- **Dark Mode**: Deep green-tinted charcoal with mint accents
- **Typography**: Newsreader (serif) for headlines, Inter (sans-serif) for UI
- **Theme Toggle**: Users can switch between light and dark modes

### Project Structure
```
Horizon.MVC/
├── Controllers/
│   ├── AuthController.cs         # Login / register / logout via identity-service
│   ├── CoursesController.cs      # Course management & browsing
│   ├── DashboardController.cs    # Student dashboard (plus Profile, Bookmarks, Progress, ...)
│   ├── TeacherController.cs      # Teacher dashboard & course workflow
│   ├── ChatMvcController.cs      # Messaging functionality
│   └── HomeController.cs         # Landing pages
├── Services/
│   ├── ApiService.cs             # Identity (auth) API integration
│   ├── BaseApiService.cs         # Shared base for student-service API clients
│   ├── *ApiService.cs            # Dashboard, Profile, Bookmark, Progress, ... clients
│   └── TeacherApiService.cs      # Teacher API integration
├── Handlers/
│   └── BearerTokenHandler.cs     # JWT token management
├── DTOs/
│   ├── CourseDTOs.cs            # Course data transfer objects
│   ├── StudentDTOs.cs           # Student data transfer objects
│   └── ChatDTOs.cs              # Chat data transfer objects
├── Views/
│   ├── Auth/                    # Login, register views
│   ├── Courses/                 # Course listing, details, create, edit
│   ├── Student/                 # Dashboard, profile, bookmarks, discovery
│   ├── Teacher/                 # Dashboard, analytics
│   ├── Chat/                    # Messaging interface
│   └── Home/                    # Landing page
└── wwwroot/
    └── js/site.js               # Theme toggle functionality
```

## Features

### Authentication
- JWT-based authentication with cookie storage
- Role-based authorization (Student/Teacher)
- Session management with 2-hour expiration

### Course Management
- Browse all courses
- View course details with comments
- Enroll in courses (Students)
- Create, edit, and publish courses (Teachers)
- Course categorization and filtering

### Student Features
- Personal dashboard with progress tracking
- Course discovery and recommendations
- Bookmark management
- Profile management with XP and achievements
- Progress monitoring with milestones

### Teacher Features
- Course creation and management
- Student analytics
- Course publishing workflow
- Draft and published course states

### Chat/Messaging
- Real-time messaging between users
- Conversation management
- Unread message indicators
- Message history

## API Integration

### Gateway Communication
All API calls route through the API Gateway at `https://localhost:7000`:
- `/courses/*` → Course Service (port 7003)
- `/students/*` → Student Service (port 7002)
- `/teachers/*` → Teacher Service (port 7001)
- `/chat/*` → Chat Service (port 7004)
- `/auth/*` → Identity Service (port 7005)

### HTTP Client Configuration
- Named HttpClients: `ApiService` (auth), `StudentAPI`, `LearningApi` (courses), `TeacherAPI`, `ChatApi`, each based on `GatewayUrl`
- Automatic JWT token attachment via BearerTokenHandler
- 30-second timeout
- Retry logic for failed requests

## Configuration

### appsettings.json
```json
{
  "GatewayUrl": "https://localhost:7000",
  "ExternalUrls": {
    "CourseFrontend": "https://localhost:7006"
  }
}
```

## Running the Application

### Prerequisites
- .NET 10 SDK
- All backend services running
- API Gateway running on port 7000

### Steps
1. Navigate to Horizon.MVC directory
2. Restore packages: `dotnet restore`
3. Run the application: `dotnet run --launch-profile https`
4. Access at: `https://localhost:7006`

## User Flows

### Student Flow
1. Login → Student Dashboard
2. View enrolled courses and progress
3. Discover new courses
4. Enroll in courses
5. Track progress and earn XP
6. Manage bookmarks
7. Chat with teachers/peers

### Teacher Flow
1. Login → Teacher Dashboard
2. View all created courses
3. Create new courses
4. Edit course details
5. Publish courses
6. View student enrollments
7. Respond to student messages

## Design System Implementation

### Color Variables (CSS)
```css
--primary: #096444 (light) / #7dd9ab (dark)
--secondary: #1a648c (light) / #88cff7 (dark)
--surface: #f2fbff (light) / #101411 (dark)
--on-surface: #121d21 (light) / #dfe4de (dark)
```

### Typography
- Headlines: Newsreader, 600 weight
- Body: Inter, 400 weight
- Buttons: Inter, 500 weight

### Components
- Cards: 0.5rem border-radius, subtle shadows
- Buttons: 0.25rem border-radius, hover effects
- Forms: Clean borders, focus states with primary color
- Navigation: Sticky header with role-based menu items

## Security

### Authentication
- JWT tokens stored in HTTP-only cookies
- Session-based token management
- 2-hour token expiration
- Automatic token refresh on API calls

### Authorization
- Role-based access control
- Route protection with [Authorize] attributes
- Separate student and teacher dashboards

## Performance Optimizations
- HTTP client reuse via IHttpClientFactory
- Session caching for user data
- Lazy loading of course images
- Minimal API calls with data caching

## Future Enhancements
- Real-time notifications with SignalR
- Advanced search and filtering
- Video player integration
- Assignment submission system
- Certificate generation
- Payment integration

## Troubleshooting

### Common Issues
1. **401 Unauthorized**: Check JWT token in cookies/session
2. **API Connection Failed**: Verify Gateway is running
3. **Theme not switching**: Clear browser cache
4. **Session expired**: Re-login to refresh token
