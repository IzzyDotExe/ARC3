const axios = require('axios');
const { Sign, Verify } = require('../../lib/jwt.js'); 

// Send the login page
function LoginRoute(req, res) {
  const {src} = req.query
  if (src) {
    res.cookie('src', src)
  }
  
  res.redirect(`/login`);

}

function RedirectRoute(req, res) {

  let url = process.env.DIRECT_URL;
  
  res.redirect(url);

}


async function CallbackRoute(req, res) {

  // If there's no code then we redirect to the login page
  if (!req.query.code) {
    res.redirect('/auth/login');
    return;
  }

  // If there is an src available from this identified user, get it.

  const { src } = req.cookies;
 
  // Extract the code from the query parameters
  const { code } = req.query;

  // Next we want to build a request to get a token
  const params = new URLSearchParams({
    client_id: process.env.DISCORD_CLIENT_ID,
    client_secret: process.env.DISCORD_CLIENT_SECRET,
    grant_type: 'authorization_code',
    code,
    redirect_uri: process.env.DISCORD_REDIRECT_URI
  });

  const headers = {
    'Content-Type': 'application/x-www-form-urlencoded',
    'Accept-Encoding': 'application/x-www-form-urlencoded'
  };

  // Send a request to the discord api to get our token, now we can accesss the user's account.
  const response = await axios.post(
    'https://discord.com/api/oauth2/token',
    params,
    {
      headers
    }
  )

  // Now that we have the user we can send requests
  // Save it as a jwt token in cookies
  const jwt = Sign(response.data);

  res.cookie('session', jwt, { maxAge: response.data.expires_in })

  if (src)
    res.redirect(src)
  else
    res.redirect('/')

}

// Delete the session token
function RevokeRoute(req, res) {

  res.cookie('session', 'null', {maxAge: -1})
  res.redirect('/');

}

module.exports = {
  login: LoginRoute,
  discord: RedirectRoute,
  callback: CallbackRoute,
  revoke: RevokeRoute
}