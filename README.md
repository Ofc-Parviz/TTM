# TTM | Enterprise Team Task Management Platform

<p align="left">
  <a href="README.fa.md">🌐 مطالعه به زبان فارسی (Persian Version)</a>
</p>

A team and task management platform for coordinating shared work, assigning it to members, and tracking each task until it is closed.

Built with **ASP.NET Core 8 MVC and Web API**, **Clean Architecture**, **DDD**, and **CQRS**.

> Full documentation (for users, developers, and technical reviewers) is available at `/Home/Docs` when running the project.

---

## Features

- Sign up and sign in with email and password
- Create a team and define your own role title in it
- Invite members by email or user ID, with a suggested access level and role title
- Accept, reject, or cancel invitations
- Three-level hierarchy: `Owner(or Leader)`, `Supervisor(or Manager)`, and `Employee(or Regular Member)`
- Full task lifecycle: create, start, claim completion or failure, close, or request a retry
- Optional deadlines, with automatic closure as a deadline failure
- Responsive web UI with dark mode and mobile navigation

---

## Running Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server, or another database compatible with the project configuration

### Steps

```bash
git clone https://github.com/ofc-parviz/ttm
cd ttm
dotnet restore
dotnet build
dotnet ef database update --project TeamTaskManagement.Persistence --startup-project Front/TeamTaskManagement.Web
dotnet run --project Front/TeamTaskManagement.Web
```

Store the database connection string and JWT settings in `appsettings.Development.json` or **User Secrets**. Do not commit these values to the repository.

Run the tests:

```bash
dotnet test
```

---

## Solution Structure

The solution contains 8 projects. Dependencies always point inward, toward the inner layers.

```text
TeamTaskManagement.sln
├─ Front/
│  └─ TeamTaskManagement.Web            MVC · Razor · Web API · Cookie · JWT
├─ Shared/
│  └─ TeamTaskManagement.Shared         Result<T> and shared contracts
├─ TeamTaskManagement.Application       Commands · Queries · Behaviors · Validators
├─ TeamTaskManagement.Domain            Aggregates · Entities · ValueObjects · Enums · Errors · Events · Specifications
│  └─ Shared/                           Domain-specific helper types
├─ TeamTaskManagement.Infrastructure    External services: password hashing, JWT issuing, authentication, email
├─ TeamTaskManagement.Persistence       EF Core · Configurations · Repositories · QuerySpecifications · Domain event publishing
└─ Tests/
   ├─ TeamTaskManagement.Domain.Tests       Domain unit tests
   └─ TeamTaskManagement.IntegrationTests   API and database integration tests
```

### Layer Responsibilities

| Project | Responsibility |
|---|---|
| `Web` | Razor views, Web API, Cookie and JWT authentication, service registration (Composition Root) |
| `Application` | Commands and Queries with MediatR, response models, pipeline behaviors, validation with FluentValidation |
| `Domain` | Aggregates, entities, value objects, status enums, domain errors, events, specifications, and repository interfaces |
| `Shared` | The `Result<T>` type and shared contracts |
| `Infrastructure` | Password hashing, JWT issuing, authentication, and fake email delivery |
| `Persistence` | EF Core, entity configurations, repository implementations, QuerySpecifications built from domain specifications, and domain event publishing |

### Dependency Rules

- `Web` depends on `Application`. It also depends on `Infrastructure` and `Persistence` for service registration.
- `Application` depends on `Domain` and `Shared`.
- `Infrastructure` and `Persistence` depend on `Domain` and `Application`.
- `Domain` depends only on `Shared(which is depended nothing)` and `MediatR(INotification for IDomainEvent)`.

---

## Domain Model

There are three root aggregates: `Team`, `User`, and `TeamTask`. They reference each other only by ID (`Guid`).

- **Team:** Contains `TeamMember` and `TeamInvite` as child entities. Creating a team registers the owner as the first member in the same operation. Constructors and state-changing methods on child entities are `internal`.
- **TeamTask:** A separate aggregate that governs the task lifecycle. Each operation is authorized using an `ActingMember`, which carries the requesting member's ID and access level.
- **User:** An independent aggregate. Creating a user raises `UserRegisteredEvent`.

### Access Levels

| Operation | Owner | Supervisor | Employee |
|---|:---:|:---:|:---:|
| Edit team details | ✅ | — | — |
| Send invitations | ✅ | ✅ (Member role only) | — |
| Change member roles and access levels | ✅ | — | — |
| Remove a member | ✅ | — | — |
| Cancel an invitation | ✅ | ✅ (own invitations only) | — |
| Create a task | ✅ | ✅ | ✅ (for self only) |
| Work on assigned tasks | ✅ | ✅ | ✅ |
| Leave the team | — | ✅ | ✅ |

---

## Business Rules

### Teams and Invitations

- A team always has exactly one Owner. The Owner cannot be transferred through an invitation, and the Owner role cannot be demoted through a role change.
- An active member cannot receive a new invitation. Each user can have at most one open (`Sent`) invitation per team.
- Accepting an invitation automatically cancels all other open invitations from the same team for that user.
- The Owner cannot leave the team.
- A member who left or was removed can rejoin with a new invitation. Their membership record is kept, not deleted.

### Tasks

- A task cannot be created or updated with a deadline in the past. Deadlines are optional.
- Closed tasks cannot be edited, have their deadline changed, or be reassigned.
- A task cannot be moved to the same creator or the same assignee it already has.
- A Employee can create tasks only for themselves. No one can create a task for the Owner.
- Changing the assignee resets the task status to `Pending`.

### Task Status Transitions

| From | To | Method | Performed by |
|---|---|---|---|
| `Pending` | `OnGoing` | `Start` | Assignee |
| `OnGoing`, `AcceptedToTryAgain` | `ClaimedAsComplete` | `ClaimComplete` | Assignee |
| `OnGoing`, `AcceptedToTryAgain` | `ClaimedAsFailure` | `ClaimFailure` | Assignee |
| `ClaimedAsComplete`, `ClaimedAsFailure` | `OrderedToTryAgain` | `OrderRetry` | Creator or Team-Owner |
| `OrderedToTryAgain` | `AcceptedToTryAgain` | `AcceptRetry` | Assignee |
| `OrderedToTryAgain` | `RejectedToTryAgain` | `RejectRetry` | Assignee |
| `ClaimedAsComplete`, `AcceptedToTryAgain` | `ClosedAsComplete` | `CloseAsComplete` | Creator or Team-Owner |
| `ClaimedAsFailure`, `RejectedToTryAgain` | `ClosedAsFailure` | `CloseAsFailure` | Creator or Team-Owner |
| Any open status | `ClosedAsDeadlineFailure` | `CloseAsDeadlineFailure` | Scheduled process |
| Any open status | `Pending` | `RotateAssignee` | Creator or Team-Owner |

---

## Authentication

The system has two authentication channels that share one core:

- **Web (MVC):** After sign-in, the user's identity is stored in a **Cookie**, which the browser sends with each request.
- **API:** `POST /api/auth/login` returns an `accessToken`. Protected requests must include this header:

```http
Authorization: Bearer <accessToken>
```

Password hashing and JWT issuing are handled in the `Infrastructure` layer. Access to teams and tasks is checked in the domain layer. UI buttons are only a convenience for the user and do not enforce security.

---

## Tests

- **`Domain.Tests`:** Unit tests for business rules, task status transitions, and the invitation flow. These do not require a database.
- **`IntegrationTests`:** Full API flows and database interaction. Before running them, check the connection settings for the test database.

---

## API Reference

All responses are JSON. In error responses, `code` is intended for the application, and `fa_message` contains the Persian message shown to the user.

### Authentication and Profile

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/auth/register` | Register |
| `POST` | `/api/auth/login` | Sign in and receive a token |
| `GET` | `/api/profile` | View profile |
| `PUT` | `/api/profile` | Update profile |
| `POST` | `/api/profile/new-password` | Change password |

### Teams

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/api/teams` | List the user's teams |
| `GET` | `/api/teams/{id}` | Team details |
| `POST` | `/api/teams` | Create a team |
| `PUT` | `/api/teams` | Update a team |
| `DELETE` | `/api/teams/{teamId}/leave` | Leave a team |
| `DELETE` | `/api/teams/{teamId}/members/{memberId}` | Remove a member |
| `PUT` | `/api/teams/{teamId}/members/{memberId}/role` | Change a member's role |

### Invitations

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/api/invites` | List invitations |
| `POST` | `/api/invites` | Send an invitation |
| `DELETE` | `/api/invites` | Cancel an invitation |
| `POST` | `/api/invites/accept` | Accept an invitation |
| `PUT` | `/api/invites/reject` | Reject an invitation |

### Tasks

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/api/tasks/for-me` | Tasks assigned to the user |
| `GET` | `/api/tasks/for-team` | Tasks of a team |
| `GET` | `/api/tasks/{id}` | Task details |
| `POST` | `/api/tasks` | Create a task |
| `PUT` | `/api/tasks/details` | Update task details |
| `PUT` | `/api/tasks/deadline` | Change the deadline |
| `PUT` | `/api/tasks/assignee` | Change the assignee |
| `PUT` | `/api/tasks/creator` | Change the creator |
| `POST` | `/api/tasks/{taskId}/start` | Start a task |
| `POST` | `/api/tasks/{taskId}/claim-complete` | Claim completion |
| `POST` | `/api/tasks/{taskId}/claim-failure` | Claim failure |
| `POST` | `/api/tasks/{taskId}/close-complete` | Close as complete |
| `POST` | `/api/tasks/{taskId}/close-failure` | Close as failed |
| `POST` | `/api/tasks/{taskId}/order-retry` | Order a retry |
| `POST` | `/api/tasks/{taskId}/accept-retry` | Accept a retry order |
| `POST` | `/api/tasks/{taskId}/reject-retry` | Reject a retry order |

---

## Architectural Decisions

| Decision | Reason | Consequence |
|---|---|---|
| Clean Architecture with a one-way dependency rule | Business rules stay independent of the framework, database, and UI, and remain testable | More projects; adding a simple feature touches several layers |
| Team as the root aggregate | Owner uniqueness and automatic invitation cancellation are enforced in one boundary | A team loads its full membership history; there is no member limit |
| Task as a separate aggregate | Task lifecycle is independent of membership changes | Assignee membership is checked in the Application layer |
| Aggregates reference each other by ID | Keeps User, Team, and TeamTask decoupled | Displaying user names or team titles requires separate reads |
| Specification in the domain, QuerySpecification in Persistence | Data selection rules are defined once | Each new domain Specification needs a matching Persistence implementation |
| Domain events published from Persistence | The domain stays unaware of how events are dispatched | Events are published after a successful commit and delivered at most once |
| `Result<T>` instead of exceptions for domain errors | Expected errors are part of the contract | Every new error must be defined in `DomainErrors` |
| Two authentication channels with one core | Web uses Cookies and API clients use tokens, with the same rules behind both | Token expiry and revocation need a separate design |

---

## A Note from the Developer

I put a great deal of time, effort, and dedication into building this project, and I'm happy to make it freely available for anyone to use, learn from, or build upon.

All I ask is that you preserve my name as the original developer and include a link to my GitHub profile when redistributing or showcasing this project.

I'm currently seeking new professional opportunities, so keeping the original attribution would mean a lot to me and help others discover my work.

Thank you for taking the time to explore this project!

### Built with ❤ 4 You - 2026/11/05 (1405/08/14 - 05:27)
