const { REST, Routes } = require('discord.js');
const debug = process.env.DEBUG;

var it = [ ... process.argv ] 

if (debug === 'true')
  console.log(it)

it.shift()
it.shift()

if (debug === 'true')
  console.log(it)

const token = process.env.TOKEN;
const client = process.env.DISCORD_CLIENT_ID;

// Connect to the bot client
const rest = new REST().setToken(token);

if (it.length > 0) {

  // Remove all application commands from every individual server selected
  it.forEach(guildid => {

    rest.put(Routes.applicationGuildCommands(client, guildid), { body: [] })
      .then(() => console.log('Successfully deleted all guild commands.'))
      .catch(console.error);

  })

}
else {

  // Remove all the global application commands if there are no individual servers selected
  rest.put(Routes.applicationCommands(client), { body: [] })
    .then(() => console.log('Successfully deleted all application commands.'))
    .catch(console.error);

}