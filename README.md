# FootScout

## About project
The project is a web application presenting a complex platform for managing football transfers. The application allows players to search for football clubs and clubs to search for players for specific positions of a football team. The system solves a problem related to the lack of a tool to facilitate the effective management of football transfers at different levels of the world's competition. To date, player transfers were mainly carried out through telephone contact, which did not provide an adequate level of organization. The application aims to improve players' skills through proper club selection and thus improve the quality of the game in the world's most popular sport. To check the quality of the software, unit tests and integration tests have been conducted using xUnit tool.

The project was built using **React** & **ASP.NET Core Web API** & **MongoDB**.

Other versions of the project:
- **React** & **ASP.NET Core Web API** & **MS SQL**: https://github.com/karoldziadkowiec/FootScout
- **React** & **ASP.NET Core Web API** & **PostgreSQL**: https://github.com/karoldziadkowiec/FootScout-PostgreSQL
- **Vue** & **ASP.NET Core Web API** & **MS SQL**: https://github.com/karoldziadkowiec/FootScout-Vue

## Construction and communication of application
- Application type: **web application**
- Architecture: **client-server**
- Communication:
    - **REST API** 
    - real-time communication (**SignalR**)

## Technologies
### Backend (server)
- **ASP.NET Core Web API** (.NET 8.0)
- **C#** (12.0)
    - asynchronous programming (async/await)
    - authentication (**JWT Bearer**), authorization with roles, policies
    - real-time communication between users (**SignalR**)
    - Dependency Injection, DTO, Controller, Service, Repository patterns
    - configuration management with file *appsettings.json*
    - application's seeder during the initial launch
    - password hashing
- Database system: 
    - **MongoDB**
        - **18** collections
- Database management tool: 
    - **MongoDB.Drvier**
- Main libraries:
    - **SignalR**
    - **MongoDB.Bson**
    - **IdentityModel.Tokens.Jwt**
    - **AutoMapper**
    - **ClosedXML**
- Unit and Integration tests: 
    - **xUnit**
- API tested by:
    - **Swagger UI**
    - **Postman**

### Frontend (client)
- **React** (18.3.1)
    - **TypeScript** (4.9.5)
        - asynchronous programming (async/await)
        - authentication (**JWT Bearer**), authorization with roles, cookies management
        - real-time communication between users (**microsoft/signalR**)
        - Service, Modal, Pagination, Routing patterns
        - Hooks
    - **HTML**
    - **CSS**
    - Main libraries:
        - **react-bootstrap, bootstrap-icons** components
        - **react-toastify**
        - **recharts**
        - **react-router-dom**
        - **axios**
        - **microsoft/signalr**
        - **jwt-decode**
        - **js-cookie**
        - **react-scroll**
        - **date-fns**

## Roles (actors) of the application
- Observer
- User
- Admin

## Main features
- **Observer**: login and account registration.
- **User**: account management, managing club histories, creating player and club advertisements, managing player and club advertisements, following advertisements, managing player and club offers, reporting application problems, managing chats, chatting with users in real time, logging out.
- **Admin**: management of users, chats, player and club advertisements, player and club offers, raporting data of user, chats, player and club advertisements, player and club offers, chatting with users in real time, reporting and managing application problems, adding new player position, adding/removing an admin, logging out.

## Images
Login page:

![1](github-img/1.png)

Registration page:

![2](github-img/2.png)

Home page:

![3](github-img/3.png)

My profile page:

![4](github-img/4.png)

Club History page:

![5](github-img/5.png)

Creating new player advertisement:

![6](github-img/6.png)

Displaying all player advertisements:

![7](github-img/7.png)

Displaying specific player advertisement with following:

![8](github-img/8.png)

Submitting a club offer:

![9](github-img/9.png)

Displaying all received club offers:

![10](github-img/10.png)

Displaying offer details:

![11](github-img/11.png)

Accepting an offer:

![12](github-img/12.png)

Chat rooms:

![13](github-img/13.png)

Real-time chatting with a selected user:

![14](github-img/14.png)

Reporting a problem connected with web app:

![15](github-img/15.png)

Admin home page:

![16](github-img/16.png)

Users page for admin:

![17](github-img/17.png)

Club advertisements page for admin:

![18](github-img/18.png)

Player advertisements - raports & stats page for admin:

![19](github-img/19.png)

Admin page for adding new admin:

![20](github-img/20.png)

### Mobile version of web app

Login page:

![21](github-img/21.png)

Chat page:

![22](github-img/22.png)

Admin home page:

![23](github-img/23.png)

### Database schema

![24](github-img/24.png)

### Unit tests results

![25](github-img/25.png)

### Integration tests results

![26](github-img/26.png)

