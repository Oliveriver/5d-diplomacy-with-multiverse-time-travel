FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Add build argument for architecture
ARG TARGETARCH

WORKDIR /app

COPY . /app

# Build with conditional flags based on architecture
RUN if [ "$TARGETARCH" = "arm64" ]; then \
  dotnet publish "5dDiplomacyWithMultiverseTimeTravel.csproj" \
  -c Release \
  -o bin \
  --disable-build-servers \
  /p:TreatWarningsAsErrors=false \
  /p:EnableNETAnalyzers=false; \
  else \
  dotnet publish "5dDiplomacyWithMultiverseTimeTravel.csproj" \
  -c Release \
  -o bin; \
  fi \
  && dotnet tool install --global dotnet-ef

RUN echo "#!/bin/sh" >> /app/docker-entrypoint.sh \
  && echo "~/.dotnet/tools/dotnet-ef database update --context SqliteGameContext" >> /app/docker-entrypoint.sh \
  && echo "dotnet /app/bin/5dDiplomacyWithMultiverseTimeTravel.dll" >> /app/docker-entrypoint.sh \
  && chmod +x /app/docker-entrypoint.sh

CMD ["/app/docker-entrypoint.sh"]
