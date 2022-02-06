FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'
ENV Sns_LocalMode='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./DocumentService.sln ./
COPY ./DocumentService/DocumentService.csproj ./DocumentService/
COPY ./DocumentService.Tests/DocumentService.Tests.csproj ./DocumentService.Tests/

#COPY ../../TokenService/TokenService/dist/TokenService.dll ./

RUN dotnet restore ./DocumentService/DocumentService.csproj
RUN dotnet restore ./DocumentService.Tests/DocumentService.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug

# CMD ./DocumentService.Tests/startup.sh

CMD dotnet test