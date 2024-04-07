FROM node AS build-step

WORKDIR /app

COPY ./arc3-dash/package*.json .
RUN node --max-old-space-size=1000 $(which npm) ci

COPY ./arc3-dash/ .

RUN node --max-old-space-size=1000 $(which npm) run build

FROM node
WORKDIR /app

COPY ./arc3-api/package*.json /app/
RUN node --max-old-space-size=1000 $(which npm) ci

COPY --from=build-step /app/build /app/build
COPY ./keys /keys
COPY ./arc3-api/src /app/src
COPY ./arc3-api/bin /app/bin
COPY ./arc3-api/__tests__ ./__tests__

ENTRYPOINT [ "node", "--max-old-space-size=512", "bin/www" ]


