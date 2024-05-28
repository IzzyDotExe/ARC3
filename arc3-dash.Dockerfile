FROM node AS build-step

WORKDIR /app

COPY ./arc3-dash/package.json .
RUN npm ci

COPY ./arc3-dash/ .

RUN npm run build

FROM nginx:1.21.0-alpine

COPY ./nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build-step /app/build /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
