# ARC V3

Arc V3 is the next generation of the open source ARC discord bot. The application now is complete with a web management dashboard coded in react JS and an API coded in express JS. 

All three services are connected together using a central mongo database. The app also includes a database explorer deployed automatically (mongo-express)

## Running the system

The arc system can be run easily using the docker-compose file provided (CURRENTLY UNFINISHED AS OF 27/03/2024)

First you should set up your `.env` file as follows
```ini
TOKEN='TOKEN' #  This should be your discord bot application token obtained from the discord developer portal.

MONGODB_URI='mongodb://root:<password>@mongo:27017/Arc3?authSource=admin' # passwords here and the mongo_inidn_root_password should match
MONGO_INITDB_ROOT_PASSWORD=<password>

GUILD_ID='id' # TEMPORORY - Currently the system will only fully function on one main server. Input it's ID here. 

BUILD_PATH='../arc3-dash/build' # This is set to wherever your react client build will be relative to the expess server.

DISCORD_CLIENT_ID="id" # Discord application client id obtained from the discord developer portal.
DISCORD_CLIENT_SECRET="secret" # discord application client secret obtained from the discord developer portal.

DISCORD_REDIRECT_URI="<base_uri>/auth/callback" # only change the base_uri part unless you know what you are doing. This will control where the user is redirected apon login. The base_uri shoudl match the one found in the client_redirect_url variable and the hosted_url

JWT_SECRET="secret" # the secret used for JWT signing of the session tokens.

CLIENT_REDIRECT_URL="<base_uri>" # This is where the user will be redirected after logging in sucessfully.

DIRECT_URL="" # This is the oauth2 url that is generated by discord for the login link. Create this in the discord developer portal. 


HOSTED_URL="<base_uri>" # the base uri where the client and server are hosted.

```

Then, simply install docker and docker compose and then run it with this command.
```
docker compose up
```


## Development

You can see some of the instructions in the default readmes given by create-react-app and such. There is also some vscode configs given as part of the project where you can run and debug every part of the system individually.