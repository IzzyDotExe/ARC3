FROM node:22
WORKDIR /app

COPY ./arc3-tasks/package*.json .
RUN node --max-old-space-size=1000 $(which npm) ci

COPY ./arc3-tasks .

ENTRYPOINT [ "tail", "-f", "/dev/null" ]