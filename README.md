# ASP.NET Notes App

This notes app is built with ASP.NET Core, featuring various functionalities for organizing your notes and todos.

## Features

- **Notes and Todos:** Create and manage both notes and todos. Todos can have multiple entries.
- **Favorites:** Mark notes or todos as favorites for quick access.
- **Labels:** Organize your data with labels, and assign or remove them from notes or todos.
- **Authentication and Authorization:** Uses JWT tokens for secure authentication and authorization.
- **Pagination:** Navigate through your notes and todos efficiently with pagination.

## Technologies, Libraries, and Patterns

- **C# 12:** The programming language used for development.
- **ASP.NET Core 8.0:** The web framework for building the app.
- **Entity Framework Core 8.0:** The ORM for database interaction.
- **Microsoft SQL Server Database:** The relational database used for data storage.
- **FluentValidation:** [Link to FluentValidation](https://docs.fluentvalidation.net/en/latest/#) - for custom model validation
- **Unit of Work Pattern:**
- **Repository Pattern:**

## Deployment

- **SmarterASP:** [Link to smarter asp](https://www.smarterasp.net/) Used for deployment.
- **SmarterASP Database Hosting:** Used for hosting the Microsoft SQL Server database.
- **SSL Certificate:** Provided by SmarterASP for secure communication.

## Getting Started

1. Clone the repository.
2. Configure your environment variables.
3. Set up your database using Entity Framework migrations.
4. Run the ASP.NET Core application.

## Contributing

If you'd like to contribute to the project, do the following:- 
1. have a thorough look at the application's code and coding style.
2. check the `TODO.md` file in the **Todos** section for issues or features you would like to work on.
3. after choosing a feature and starting to write code, please make sure that:-

   3.1. you write clean, understandable code that could be well-read and modified later.<br/>
   3.2. your changes DO NOT break any existing functionality.<br/>
   3.3. you only add functionality, DO NOT REMOVE ANY WORKING CODE.<br/>
   
Any PR that does not comply with the rules above will be revoked, and the author shall be given notice of the reasons.

Happy contributions!
## License

This project is licensed under the [MIT License](LICENSE).
