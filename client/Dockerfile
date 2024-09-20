FROM alpine:latest AS builder

RUN apk add nodejs yarn

WORKDIR /app
COPY . .

RUN yarn
RUN VITE_SERVER_URL=/api yarn build

FROM alpine:latest

RUN apk add caddy
COPY Caddyfile /etc/Caddyfile

COPY --from=builder /app/dist /dist

ENTRYPOINT [ "caddy", "run", "-c", "/etc/Caddyfile" ]
