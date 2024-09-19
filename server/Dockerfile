FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . /app
RUN dotnet build -c Release -o bin

RUN dotnet tool install --global dotnet-ef

CMD ["/app/docker-entrypoint.sh"]
