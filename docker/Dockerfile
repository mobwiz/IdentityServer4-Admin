FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build


# build api
WORKDIR /server

COPY ["./server/", "."]

RUN dotnet restore "IdentityServer4.Admin.WebApi/IdentityServer4.Admin.WebApi.csproj"

RUN dotnet build "IdentityServer4.Admin.WebApi/IdentityServer4.Admin.WebApi.csproj" -c Release -o /app/publish --no-restore

# build view
FROM node:14.21.0 as view_base
WORKDIR /admin
COPY ["./admin/", "."]

RUN yarn
# RUN npm run fix-memory-limit
RUN yarn build

FROM  mcr.microsoft.com/dotnet/aspnet:6.0 AS final
RUN useradd -s /bin/bash works
WORKDIR /app
USER works

COPY --from=build --chown=works /app/publish .
COPY --from=view_base --chown=works /admin/dist ./wwwroot/admin

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "IdentityServer4.Admin.WebApi.dll"]


