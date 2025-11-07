# ==================================================
# Stage 1: Restore dependencies
# ==================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy only dependency-related files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restore only Web project (will restore all referenced projects)
RUN dotnet restore "src/Web/Web.csproj"

# ==================================================
# Stage 2: Build and Publish application
# ==================================================
FROM restore AS build
ARG BUILD_CONFIGURATION=Release

# Copy only source code needed for Web project
COPY ["src/Web/", "src/Web/"]
COPY ["src/Application/", "src/Application/"]
COPY ["src/Domain/", "src/Domain/"]
COPY ["src/Infrastructure/", "src/Infrastructure/"]

# Build and publish with optimizations (framework-dependent con trimming limitado)
WORKDIR "/src/src/Web"
RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:DebugType=None \
    /p:DebugSymbols=false \
    /p:Optimize=true

# ==================================================
# Stage 3: Final runtime image
# ==================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install curl for healthcheck and ICU libraries for globalization support
RUN apk add --no-cache \
    curl \
    icu-libs \
    icu-data-full

# Expose ports
EXPOSE 8080

# Copy published files
COPY --from=build /app/publish .

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

USER appuser

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "FinalProject.Web.dll"]
