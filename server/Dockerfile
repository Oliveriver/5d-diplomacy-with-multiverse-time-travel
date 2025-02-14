FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . /app
RUN dotnet build -c Release -o bin

RUN dotnet tool install --global dotnet-ef

RUN echo "#!/bin/sh" >> /app/docker-entrypoint.sh \
  && echo "~/.dotnet/tools/dotnet-ef database update -Context SqliteGameContext" >> /app/docker-entrypoint.sh \
  && echo "dotnet /app/bin/5dDiplomacyWithMultiverseTimeTravel.dll" >> /app/docker-entrypoint.sh \
  && chmod +x /app/docker-entrypoint.sh

CMD ["/app/docker-entrypoint.sh"]
