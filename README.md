# Onion Architecture
The Onion Architecture is a software architectural pattern that focuses on maintaining the independence and separation of concerns in an application. It was introduced as an extension of the principles behind Domain-Driven Design* (DDD) and Clean Architecture**.
In the Onion Architecture, the application is organized into concentric circles or layers, with the core domain logic at the center. Each layer only depends on the layer closer to the center, hence dependencies flow inward.

<p align="center">
  <img src="https://miro.medium.com/v2/resize:fit:462/1*0Pg6_UsaKiiEqUV3kf2HXg.png" />
</p>

# Layers
## Domain Layer
This is the innermost layer, representing the core domain logic of the application. It contains domain entities, value objects, domain services, and business rules. The core layer is independent of the application's external concerns, frameworks, and technologies.
By keeping domain layer decoupled from other layers, enable application to be developed in a more testable and maintainable manner.

## Application Layer
This layer surrounds the Domain Layer and these two together forms the Core of the project. Application layer contains application-specific business logic, use cases, and workflows. It orchestrates interactions between the Core Layer and the outermost layers.
This is a an abstraction layer between Domain Layer and outermost layers, hence we implement our repositories and services in this layer (interfaces). DTOs and Mappers are also implemented in this layer.

## Infrastructure Layer
This is the outermost layer, responsible for dealing with data access and external services. It provides concrete implementations of interfaces defined in the Domain and Application Layers. Infrastructure Layer also contains Persistence Layer which is also implemented as a different layer where we implement
our database operations and data access logic. Database Contexts, Database Configurations, Migrations and Seedings are implemented in Persistence Layer where external business related services are implemented in Infrastructure Layer.

# What's wrong with traditional Layered Architecture ?
Although, layered architecture may enable us to design our project according to *Seperation of Concerns* principle via implementing different layers such as Presentation Layer, Business Logic Layer and Data Access Layer;
these layers are tightly-coupled, hence it is diffucult to maintain a largely scaled projects with this architecture. Also, Data Access Layer tend to be implemented as the inner-most layer in a traditional layered architecture and
this design principle results in a costly migration process if it is decided to change the data access logic of the application. (Ex: Changing the DBMS) Hence application of DDD as in Onion Architecture enable us to developed
loosely-coupled and maintainable code, where development process is based on the domain logic of the project which is the innermost layer of the architecture.


*Domain Driven Design : Domain driven design is an approach that emphasizes the modeling the domain of the application as the primary concern. The primary goal of DDD is to align the software's domain model closely with the real world domain it represents which0 leads to a more effective and expressive codebase.

** Clean Architecture : Clean Architecture emphasizes the seperation of concerns and independence of various components in an application. The primary goal of Clean Arch is to establish a flexible and maintainable codebase that can easily adapt to changes and updates on requirements and technologies.