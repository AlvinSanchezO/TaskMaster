# 1. Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos solo el proyecto para restaurar
COPY TaskMaster.CLI/TaskMaster.CLI.csproj TaskMaster.CLI/
RUN dotnet restore "TaskMaster.CLI/TaskMaster.CLI.csproj"

# Copiamos el resto del código
COPY . .

# Nos movemos a la carpeta del proyecto y publicamos
WORKDIR "/src/TaskMaster.CLI"
RUN dotnet publish "TaskMaster.CLI.csproj" -c Release -o /app/out

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "TaskMaster.CLI.dll"]