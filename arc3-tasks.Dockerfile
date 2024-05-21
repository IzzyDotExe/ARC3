FROM node 
WORKDIR /app

COPY ./arc3-tasks .

ENTRYPOINT [ "node" ]