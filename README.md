# Hospital Vacation Management API

API para gestão de férias hospitalares, construída com ASP.NET Core, Clean Architecture, PostgreSQL e Docker.

## Objetivo

O sistema gerencia solicitações de férias de profissionais hospitalares considerando regras de negócio sensíveis para operação de setores críticos.

## Regras De Negócio

- Um setor não pode ficar sem profissionais disponíveis.
- Pelo menos um profissional sênior deve permanecer disponível.
- Cada setor possui um limite de férias simultâneas.
- Períodos críticos, como Natal e Ano Novo, são bloqueados.
- Um funcionário não pode ter solicitações pendentes ou aprovadas em conflito de datas.
- Apenas solicitações pendentes podem ser aprovadas ou reprovadas.
- Solicitações aprovadas não podem ser canceladas diretamente.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- Docker e Docker Compose
- JWT Authentication
- FluentValidation
- Serilog
- Health Checks
- xUnit

## Integração Contínua

O projeto possui um workflow no GitHub Actions que executa automaticamente:

- restauração de dependências;
- build da solução;
- execução dos testes automatizados.

O workflow roda em pushes e pull requests para a branch `main`.

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

### Domain

Contém entidades, enums e regras de negócio.

Exemplos:

- `Employee`
- `Department`
- `VacationRequest`
- `VacationPolicyService`

### Application

Contém casos de uso, requests, responses, handlers, interfaces e validações.

Exemplos:

- `RequestVacationHandler`
- `ApproveVacationRequestHandler`
- `IEmployeeRepository`
- `IVacationRequestRepository`
- Validadores com FluentValidation

### Infrastructure

Contém implementações externas, como banco de dados e repositórios.

Exemplos:

- `AppDbContext`
- `EmployeeRepository`
- `DepartmentRepository`
- `VacationRequestRepository`
- `UnitOfWork`

### Api

Contém endpoints HTTP, autenticação JWT, Swagger, logs e health checks.

## Como Rodar

### Pré-requisitos

- .NET 8 SDK
- Docker Desktop
- Git

### Subir PostgreSQL

```powershell
docker compose up -d
```

### Aplicar Migrations

```powershell
dotnet ef database update --project src/HospitalVacationManagement.Infrastructure
```

### Rodar A API

```powershell
dotnet run --project src/HospitalVacationManagement.Api/HospitalVacationManagement.Api.csproj
```

Swagger:

```text
http://localhost:5130/swagger
```

A porta pode variar. Confira o terminal ao rodar a API.

## Autenticação

Endpoint de login:

```http
POST /auth/login
```

Payload:

```json
{
  "email": "admin@hospital.com",
  "password": "Admin@123"
}
```

Use o token retornado no Swagger em:

```text
Bearer SEU_TOKEN
```

## Endpoints Principais

### Férias

```http
POST /api/v1/vacation-requests
GET /api/v1/vacation-requests
PUT /api/v1/vacation-requests/{id}/approve
PUT /api/v1/vacation-requests/{id}/reject
PUT /api/v1/vacation-requests/{id}/cancel
```

### Setores

```http
POST /departments
GET /departments
GET /departments/{id}
```

### Funcionários

```http
POST /employees
GET /employees
GET /employees/{id}
GET /departments/{departmentId}/employees
```

### Monitoramento

```http
GET /health
```

## Testes

Rodar todos os testes:

```powershell
dotnet test tests/HospitalVacationManagement.Tests/HospitalVacationManagement.Tests.csproj
```

## Próximos Passos

- Criar usuários reais no banco.
- Implementar hash de senha.
- Adicionar roles e policies.
- Criar testes de integração.
- Criar Dockerfile da API.
- Melhorar documentação dos endpoints.
- Adicionar paginação em funcionários e setores.