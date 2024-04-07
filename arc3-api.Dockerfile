FROM node AS build-step

WORKDIR /app

COPY ./arc3-dash/package.json .
RUN npm i

COPY ./arc3-dash/ .

RUN npm run build

FROM node
WORKDIR /app

COPY ./arc3-api/package*.json /app/
RUN npm ci

COPY --from=build-step /app/build /app/build

COPY ./arc3-api/src /app/src
COPY ./arc3-api/bin /app/bin
COPY ./arc3-api/__tests__ ./__tests__

ENTRYPOINT [ "node", "bin/www" ]


