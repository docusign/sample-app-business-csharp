#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
    && apt-get install -y ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list
RUN apt-get update && apt-get install nodejs -y
RUN npm install -g @angular/cli && npm update

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build

WORKDIR /src
COPY ["DocuSign.MyBusiness/*.sln", "DocuSign.MyBusiness/"]
COPY ["DocuSign.MyBusiness/DocuSign.MyBusiness/DocuSign.MyBusiness.csproj", "DocuSign.MyBusiness/DocuSign.MyBusiness/"]
WORKDIR /src/DocuSign.MyBusiness
RUN dotnet restore
COPY . .

RUN dotnet build "DocuSign.MyBusiness/DocuSign.MyBusiness/DocuSign.MyBusiness.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update \
    && apt-get install -y ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list
RUN apt-get update && apt-get install nodejs -y \
    && npm install -g @angular/cli
RUN npm update && npm config delete proxy

RUN dotnet publish "DocuSign.MyBusiness/DocuSign.MyBusiness/DocuSign.MyBusiness.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocuSign.MyBusiness.dll"]
