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