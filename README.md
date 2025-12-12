# Jewelry AWS System

A comprehensive backend API system for managing jewelry products, built with .NET 9.0 and integrated with AWS services. This system provides product management, user authentication, reviews, and image storage capabilities.

## 🚀 Features

- **Product Management**: Full CRUD operations for jewelry products with image upload
- **Authentication & Authorization**: AWS Cognito integration with JWT Bearer token authentication and role-based access control (Admin/User)
- **Image Storage**: AWS S3 integration for product image storage
- **Reviews System**: Users can create and view product reviews
- **Product Sizes**: Manage product size variations
- **Logging**: AWS CloudWatch Logs integration for centralized logging
- **API Documentation**: Swagger/OpenAPI documentation for easy API exploration
- **Database Migrations**: Entity Framework Core migrations for database schema management

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or access to a PostgreSQL database)
- [AWS Account](https://aws.amazon.com/) with the following services configured:
  - Amazon S3 (for image storage)
  - AWS Cognito (for authentication)
  - AWS CloudWatch Logs (for logging)

## 🛠️ Technology Stack

- **Framework**: .NET 9.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: AWS Cognito with JWT Bearer tokens
- **Cloud Services**:
  - Amazon S3 (File Storage)
  - AWS Cognito (User Authentication)
  - AWS CloudWatch Logs (Logging)
- **API Documentation**: Swagger/OpenAPI

## 📁 Project Structure

```
JewelryAWS/
├── Jewelry-API/              # Main API project (Controllers, Program.cs, Dependency Injection)
│   ├── Controllers/          # API controllers
│   ├── Constant/             # API endpoint constants
│   └── Properties/           # Launch settings
├── Jewelry-Model/            # Data models and entities
│   ├── Entity/               # Database entities and DbContext
│   ├── Enum/                 # Enumerations (Role, Status)
│   ├── Migrations/           # Entity Framework migrations
│   ├── Paginate/             # Pagination utilities
│   ├── Payload/              # Request/Response models
│   ├── Settings/             # Configuration settings classes
│   └── Utils/                # Utility classes (JWT, User, etc.)
├── Jewelry-Repository/       # Repository pattern implementation
│   ├── Interface/            # Repository interfaces
│   └── Implement/            # Repository implementations
└── Jewelry-Service/          # Business logic layer
    ├── AwsS3/                # AWS S3 storage service
    ├── Interfaces/           # Service interfaces
    └── Implements/           # Service implementations
```

## ⚙️ Configuration

### 1. Database Connection

Configure your PostgreSQL connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultDB": "Host=localhost;Database=jewelrydb;Username=your_user;Password=your_password"
  }
}
```

### 2. AWS Configuration

Add AWS configuration to `appsettings.json`:

```json
{
  "AwsConfig": {
    "UserCredentials": {
      "AccessKey": "your-aws-access-key",
      "SecretKey": "your-aws-secret-key"
    }
  },
  "AwsSettings": {
    "UserCredentials": {
      "AccessKey": "your-aws-access-key",
      "SecretKey": "your-aws-secret-key"
    },
    "S3": {
      "Region": "us-east-1",
      "BucketName": "your-s3-bucket-name"
    },
    "Cognito": {
      "ClientId": "your-cognito-client-id",
      "ClientSecret": "your-cognito-client-secret",
      "Domain": "https://cognito-idp.region.amazonaws.com/your-user-pool-id",
      "ReturnUrl": "http://localhost:3000"
    }
  },
  "Logging": {
    "Region": "us-east-1",
    "LogGroup": "jewelry-api-logs"
  }
}
```

**⚠️ Security Note**: Never commit `appsettings.json` with real credentials to version control. Use environment variables, User Secrets, or AWS Secrets Manager in production.

## 🔧 Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd AWS_Jewelry_System-main/JewelryAWS
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure appsettings.json**
   - Set up your database connection string
   - Configure AWS credentials and settings

4. **Run database migrations**
   ```bash
   cd Jewelry-API
   dotnet ef database update --project ../Jewelry-Model
   ```
   
   Or the migrations will run automatically on application start (configured in `Program.cs`).

5. **Run the application**
   ```bash
   dotnet run --project Jewelry-API
   ```

6. **Access Swagger UI**
   - Navigate to `https://localhost:5001/swagger` (or the configured port)
   - The Swagger UI provides interactive API documentation

## 📚 API Endpoints

### Authentication
- `POST /api/v1/auth/token` - Exchange authorization code for JWT token
- `GET /api/v1/auth/userinfo` - Get authenticated user information (Requires authentication)

### Account Management
- `POST /api/v1/account` - Register new account (Admin only)
- `GET /api/v1/account` - Get all accounts (Admin only)
- `GET /api/v1/account/{id}` - Get account by ID (Admin only)
- `PUT /api/v1/account` - Update account (Admin only)
- `DELETE /api/v1/account/{id}` - Delete account (Admin only)

### Product Management
- `POST /api/v1/product` - Create product with image upload (Admin only)
- `GET /api/v1/product` - Get all products (paginated)
- `GET /api/v1/product/{id}` - Get product by ID
- `PUT /api/v1/product/{id}` - Update product (Admin only)
- `DELETE /api/v1/product/{id}` - Delete product (Admin only)

### Product Sizes
- `GET /api/v1/product/{productId}/sizes` - Get all sizes for a product (Admin/User)
- `POST /api/v1/product/{productId}/sizes` - Create product size (Admin only)
- `PUT /api/v1/product/{productId}/sizes/{id}` - Update product size (Admin only)
- `DELETE /api/v1/product/{productId}/sizes/{id}` - Delete product size (Admin only)

### Reviews
- `POST /api/v1/product/{id}/review` - Create review for a product (Admin/User)
- `GET /api/v1/product/{id}/review` - Get all reviews for a product (paginated, Admin/User)
- `DELETE /api/v1/review/{id}` - Delete review (Admin only)

### Sizes
- `POST /api/v1/size` - Create size (Admin only)
- `GET /api/v1/size` - Get all sizes
- `GET /api/v1/size/{id}` - Get size by ID

## 🔐 Authentication

The API uses AWS Cognito for authentication. To access protected endpoints:

1. Obtain an authorization code from AWS Cognito
2. Exchange the code for a JWT token using `POST /api/v1/auth/token`
3. Include the token in the Authorization header: `Bearer <your-token>`

### Role-Based Authorization

The API supports two roles:
- **Admin**: Full access to all endpoints
- **User**: Access to product viewing, reviews, and limited operations

## 🗄️ Database Schema

### Entities

- **Account**: User accounts with roles (Admin/User)
- **Product**: Jewelry products with images stored in S3
- **Size**: Available product sizes
- **ProductSize**: Junction table linking products to sizes with stock information
- **Review**: User reviews for products

## 🚀 Deployment

### Environment Variables

For production deployment, use environment variables or AWS Secrets Manager:

- `ConnectionStrings__DefaultDB`
- `AwsConfig__UserCredentials__AccessKey`
- `AwsConfig__UserCredentials__SecretKey`
- `AwsSettings__S3__Region`
- `AwsSettings__S3__BucketName`
- `AwsSettings__Cognito__ClientId`
- `AwsSettings__Cognito__ClientSecret`
- `AwsSettings__Cognito__Domain`
- `Logging__Region`
- `Logging__LogGroup`

### AWS Services Setup

1. **S3 Bucket**: Create a bucket for storing product images
2. **Cognito User Pool**: Set up a user pool with appropriate app clients
3. **CloudWatch Logs**: Create a log group for application logs
4. **IAM Roles**: Ensure proper IAM permissions for S3, Cognito, and CloudWatch

## 🧪 Development

### Running Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project Jewelry-Model --startup-project Jewelry-API

# Update database
dotnet ef database update --project Jewelry-Model --startup-project Jewelry-API
```

### Project Dependencies

The solution uses a layered architecture:
- **Jewelry-API** depends on **Jewelry-Service**
- **Jewelry-Service** depends on **Jewelry-Repository**
- **Jewelry-Repository** depends on **Jewelry-Model**
- All projects reference **Jewelry-Model**

## 📝 License

This project is part of an educational/training system.

## 👥 Contributors

T1 VN - Fall 2025

## 📞 Support

For issues and questions, please create an issue in the repository.

---

**Note**: Make sure to configure all AWS services and credentials before running the application. The application requires active AWS credentials and properly configured S3 buckets and Cognito user pools.
