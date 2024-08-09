# 5D Diplomacy With Multiverse Time Travel

Currently a WIP.

A new standard in measuring how galaxy-brained you are, 5D Diplomacy With Multiverse Time Travel combines the classic game of pure negotiation with the modern classic game of pure disorientation. Can you convince your opponent to support an attack in the present while simultaneously backstabbing them five years ago and seven timelines over?

Inspired by and indebted to the board game [Diplomacy](https://shop.hasbro.com/en-us/product/avalon-hill-diplomacy-cooperative-strategy-board-game-ages-12-and-up-2-7-players/09A402C7-4CA2-4E9D-9449-4592B2066011) and the video game [5D Chess With Multiverse Time Travel](https://www.5dchesswithmultiversetimetravel.com/). Both are excellent in their own right, so we recommend picking up a copy of each to understand the rules for 5D Diplomacy.

_Diplomacy_ is a trademark of Avalon Hill. _5D Chess With Multiverse Time Travel_ is a trademark of Thunkspace, LLC. _5D Diplomacy With Multiverse Time Travel_ and its creators are not affiliated with either _Diplomacy_ or _5D Chess With Multiverse Time Travel_.

## Installation

The two components - found in the `server` and `client` directories - may be run together or independently. The client always requires a server instance (local or remote) for the game to function beyond the welcome and setup screens.

The `prototype` directory contains the original proof of concept from 2021. None of its contents are required for running the latest version of 5D Diplomacy.

### Server

Requirements:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- [Entity Framework Core command line tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet). If .NET has been installed, these can be installed by running `dotnet tool install --global dotnet-ef`.
- A blank database instance running on [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads).

Steps:

- Navigate to the `server` directory.
- Copy `appsettings.json` to a new file, `appsettings.Development.json`, and inside the new file, replace `DATABASE_CONNECTION_STRING` with the connection string for the active database instance.
- Run `dotnet build`.
- Run `dotnet ef database update`.
- Run `dotnet run` to start the server.

### Client

Requirements:

- [Node.js](https://nodejs.org/en/download/prebuilt-installer).
- [Yarn package manager](https://yarnpkg.com/). If Node.js has been installed, this can be installed by running `npm install --global yarn`.
- A running instance of the server, whether local or remote.

Steps:

- Navigate to the `client` directory.
- Copy `.env` to a new file, `.env.local`, and inside the new file, replace `SERVER_URL` with the base domain of the active server instance.
- Run `yarn install`.
- Run `yarn dev` to start the client in the default browser.
