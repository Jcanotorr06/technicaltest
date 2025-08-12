# 1. Database Querying

## Context

The development of the application requires storing and retrieving data in a scalable and performan manner with a short development cycle. I have identified two primary approaches for data access from Azure functions: LinQ to SQL and Stored Procedures.

LinQ to SQL provides a more developer-friendly experience, allowing for rapid development and easier maintenance. It abstracts the database interactions into a more readable format, which can speed up development time and reduce the potential for errors.

Stored Procedures, on the other hand, offer better performance for complex queries and can be optimized directly in the database. They allow for more control over the execution plan and can reduce the amount of data transferred over the network.

## Decision

After considering the trade-offs, I have decided to use LinQ to SQL for the initial implementation due to its ease of use and faster development cycle. However, I will monitor the application's performance and be open to refactoring critical queries into Stored Procedures if necessary.

## Rationale

1. LinQ to SQL allows for faster development and easier maintenance, which is crucial given the tight development timeline.
2. The abstraction provided by LinQ to SQL reduces the likelihood of errors in database interactions.
3. Stored Procedures may be considered for specific performance-critical queries in the future, but LinQ to SQL is sufficient for the initial implementation.

## Consequences

1. The initial development will be faster and more flexible, allowing for quicker iterations and feature additions.
2. If performance issues arise, additional work may be required to identify and optimize specific queries using Stored Procedures.
