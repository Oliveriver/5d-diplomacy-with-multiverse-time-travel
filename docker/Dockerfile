# Unified Dockerfile for 5d-diplomacy-with-multiverse-time-travel
# Builds and runs both the .NET server and Vite/Caddy client

############################################
# Build .NET Server
############################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build

WORKDIR /server
COPY server/ ./
RUN dotnet build -c Release -o /server/bin

# Apply migrations (optional, can be run at runtime)
RUN dotnet tool install --global dotnet-ef

############################################
# Build Vite Client
############################################
FROM node:20-alpine AS client-build

WORKDIR /client
COPY client/ ./
RUN yarn install --frozen-lockfile
RUN VITE_SERVER_URL=/api yarn build

############################################
# Final Runtime Image
############################################
FROM alpine:latest

# Install Caddy for static file serving
RUN apk add --no-cache caddy

# Install .NET runtime dependencies
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

# Copy server build artifacts
COPY --from=server-build /server/bin /app/server

# Copy client build artifacts
COPY --from=client-build /client/dist /app/client/dist
COPY client/Caddyfile /etc/Caddyfile

# Entrypoint script to run both server and client
COPY docker/start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 5000    # .NET server port
EXPOSE 80      # Caddy (client) port

ENTRYPOINT ["/start.sh"]
