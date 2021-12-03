FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./DocumentServiceListener.sln ./
COPY ./DocumentServiceListener/DocumentServiceListener.csproj ./DocumentServiceListener/
COPY ./DocumentServiceListener.Tests/DocumentServiceListener.Tests.csproj ./DocumentServiceListener.Tests/
# COPY /nuget.config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./DocumentServiceListener/DocumentServiceListener.csproj
RUN dotnet restore ./DocumentServiceListener.Tests/DocumentServiceListener.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out DocumentServiceListener.Tests/DocumentServiceListener.Tests.csproj

CMD dotnet test
