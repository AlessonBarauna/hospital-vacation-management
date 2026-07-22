FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HospitalVacationManagement.sln ./
COPY src/HospitalVacationManagement.Api/HospitalVacationManagement.Api.csproj src/HospitalVacationManagement.Api/
COPY src/HospitalVacationManagement.Application/HospitalVacationManagement.Application.csproj src/HospitalVacationManagement.Application/
COPY src/HospitalVacationManagement.Domain/HospitalVacationManagement.Domain.csproj src/HospitalVacationManagement.Domain/
COPY src/HospitalVacationManagement.Infrastructure/HospitalVacationManagement.Infrastructure.csproj src/HospitalVacationManagement.Infrastructure/

RUN dotnet restore src/HospitalVacationManagement.Api/HospitalVacationManagement.Api.csproj

COPY . .

RUN dotnet publish src/HospitalVacationManagement.Api/HospitalVacationManagement.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "HospitalVacationManagement.Api.dll"]