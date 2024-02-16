#!/bin/bash

# Set the path of the .env file relative to this new current directory
export ENV_FILE=.env

if [ -f $ENV_FILE ] 
then

  echo "Found .env file!"


  # run docker compose 
  echo Running Docker Compose...
  docker compose -p arcsystem --env-file "$ENV_FILE" -f docker-compose.yml up --build
  
  exit 0

else

  echo "Error: .env file not found"
  read -p "Please make sure it exists and try again"
  exit 1

fi
