# Hospital Vacation Management API

API para gestão de férias hospitalares, construída com ASP.NET Core, Clean Architecture, PostgreSQL, Docker, JWT, observabilidade e testes automatizados.

## Objetivo

O sistema gerencia solicitações de férias de profissionais hospitalares considerando regras de negócio sensíveis para operação de setores críticos.

A API impede cenários como setor sem profissionais disponíveis, ausência de profissional sênior, conflitos de datas e solicitações em períodos críticos.

## Regras De Negócio

- Um setor não pode ficar sem profissionais disponíveis.
- Pelo menos um profissional sênior deve permanecer disponível.
- Cada setor possui um limite de férias simultâneas.
- Períodos críticos, como Natal e Ano Novo, são bloqueados.
- Um funcionário não pode ter solicitações pendentes ou aprovadas em conflito de datas.
- Apenas solicitações pendentes podem ser aprovadas ou reprovadas.
- Solicitações aprovadas não podem ser canceladas diretamente.
- Usuários comuns não podem cancelar solicitações de outros usuários.
- Administradores e gestores podem aprovar, reprovar e cancelar solicitações conforme permissões.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- Docker e Docker Compose
- JWT Authentication
- FluentValidation
- Serilog
- Seq
- Health Checks
- xUnit
- GitHub Actions

## Arquitetura

O projeto segue uma organização inspirada em Clean Architecture:

```text
src
├── HospitalVacationManagement.Domain
├── HospitalVacationManagement.Application
├── HospitalVacationManagement.Infrastructure
└── HospitalVacationManagement.Api

tests
└── HospitalVacationManagement.Tests
```
## Camadas

### Domain

Contém entidades, enums e regras de negócio puras.

Exemplos:

```text
- Employee
- Department
- VacationRequest
- VacationPolicyService
- User
- RefreshToken
```

## Application

Contém casos de uso, requests, responses, handlers, interfaces e validações.

## Exemplos:

- RequestVacationHandler
- ApproveVacationRequestHandler
- CancelVacationRequestHandler
- ListVacationRequestsHandler
- StaffAvailabilityHandler
- IEmployeeRepository
- IVacationRequestRepository
- ICurrentUserService
- Validadores com FluentValidation
