const axios = require('axios');

const fetch = (...args) =>
  import('node-fetch').then(({default:fetch}) => fetch(...args));

const options = {
  method: "GET",
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded',
    'Accept-Encoding': 'application/x-www-form-urlencoded',
    'Authorization' : `Bot ${process.env.TOKEN}`
  }
};

const discordCache = {

}

async function GetUser(req, res) {

  const id = req.params.id;

  // Guard if the userid is undefined
  if (id === undefined)
    res.status(404).json({status: 404, error: "Could not find that user"})

  try {
    
    if (discordCache[id]) {
      discordCache[id]['cached'] = true;
      res.status(200).json(discordCache[id]);
      return;
    }

    axios.get(`https://discord.com/api/users/${id}`, options).then(json => {
      
      if (!discordCache[id])
        discordCache[id] = json.data;

      discordCache[id]['cached'] = false;
      res.status(200).json(discordCache[id]);

    }).catch(err => {
      res.status(500).json({
        status: 500,
        error: "An error occured, try again later!"
      })  
    })

  } catch (err) {

    console.error(err);
    res.status(500).json({
      status: 500,
      error: "An error occured, try again later!"
    })

  }

}

async function GetGuild(req, res) {

  const id = req.params.id;

  // Guard if the userid is undefined
  if (id === undefined)
    res.status(404).json({status: 404, error: "Could not find that guild"})

  try {
    
    if (discordCache[id]) {
      discordCache[id]['cached'] = true;
      res.status(200).json(discordCache[id]);
      return;
    }

    axios.get(`https://discord.com/api/guilds/${id}/preview`, options).then(json => {
      
      if (!discordCache[id])
        discordCache[id] = json.data;

      discordCache[id]['cached'] = false;
      res.status(200).json(discordCache[id]);

    }).catch(err => {
      res.status(500).json({
        status: 500,
        error: "An error occured, try again later!"
      })  
    })

  } catch (err) {

    console.error(err);
    res.status(500).json({
      status: 500,
      error: "An error occured, try again later!"
    })

  }

}

module.exports = { GetUser, GetGuild }; 