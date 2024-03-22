# E-Commerce-Application-ASP.NET-Core-7

This application serves as an online marketplace offering various products for sale, with access limited to authorized users who can make purchases. The Ecommerce application is built using ASP.NET Core and Visual Studio.

## Prerequisites
Before you begin the setup process, ensure that you have the following prerequisites installed on your system:

- Visual Studio
- .NET Core SDK
- SQL Server



# Getting Started

## Setup

**1. Download the Solution: After downloading the Ecom.sln solution, navigate to the appsettings.json file to configure the correct connection string for your local machine.**

- Clone the Repository: Clone the repository from the Git repository hosting service to your local machine using the following command:
```bash
  git clone https://github.com/ahmetsukkar/Ecommerce-API
```

**2. Set Startup Project: Set the Ecom.API Project as the startup project in Visual Studio.**

**3. Build and Run Application:**
- Build the solution and run the application. Upon the first run, the application will automatically create the necessary database on your local machine based on the provided connection string.

## Usage
To utilize the Ecommerce application effectively, follow these steps:

**1. User Registration:**
- Utilize the /api/accounts/register endpoint to create a new user account.

**2. User Authentication:**
- Authenticate to the system using the /api/accounts/login endpoint. This endpoint will provide user information along with an access token.

**3. Browse Products:**
- Access available products via the /api/products/get-all-products endpoint to view the available items.

**4. Select Product:**
- Choose the desired product(s) from the available list.

**5. Create Order:**
- Utilize the /api/orders/create-order endpoint to place a new order. Ensure to include the user token for authorization. Only authorized users can create orders.

## Additional Information
- Postman Collection: Import the provided Postman file included in the README to consume the API services efficiently.
- API Documentation: For a comprehensive list of available endpoints and their functionalities, refer to the API Documentation file.

This provides a quick and efficient method to purchase products (books) through the Ecommerce application. For further details on available endpoints and their usage, please consult the API documentation.

Thank you for using our Ecommerce application!

## Document and Files

[API Documentation](https://github.com/ahmetsukkar/Ecommerce-API/blob/master/Ecommerce%20API%20Document.pdf)

[Postman file](https://github.com/ahmetsukkar/Ecommerce-API/blob/master/Ecommerce%20API%20Document.pdf)
