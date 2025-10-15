5d-diplomacy-with-multiverse-time-travel/docker/start.sh
```
```5d-diplomacy-with-multiverse-time-travel/docker/start.sh#L1-20
#!/bin/sh
set -e

# Start the .NET server in the background
echo "Starting .NET server..."
dotnet /app/server/5dDiplomacyWithMultiverseTimeTravel.dll &

SERVER_PID=$!

# Start Caddy to serve the client
echo "Starting Caddy server for client..."
caddy run --config /etc/Caddyfile --adapter caddyfile &

CADDY_PID=$!

# Wait for either process to exit
wait -n $SERVER_PID $CADDY_PID

# If one process exits, kill the other
kill $SERVER_PID $CADDY_PID 2>/dev/null || true
