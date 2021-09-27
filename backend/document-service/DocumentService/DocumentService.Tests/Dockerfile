FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./DocumentService.sln ./
COPY ./DocumentService/DocumentService.csproj ./DocumentService/
COPY ./DocumentService.Tests/DocumentService.Tests.csproj ./DocumentService.Tests/
# COPY /nuget.config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./DocumentService/DocumentService.csproj
RUN dotnet restore ./DocumentService.Tests/DocumentService.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out DocumentService.Tests/DocumentService.Tests.csproj

CMD dotnet test
