FROM node AS build-step

WORKDIR /app

COPY ./arc3-dash/package.json .
RUN npm i 

COPY ./arc3-dash/ .

RUN npm run build

FROM node
WORKDIR /app

COPY ./arc3-api/package.json .
RUN npm i

COPY ./arc3-api .
COPY --from=build-step /app/build ./build

ENTRYPOINT [ "npx", "forever", "bin/www" ]


