const { Sign, Verify } = require('../../lib/jwt.js');
const axios = require('axios');

const userCache = {

}

/**
 * Use this on any route to make sure that the user is logged in when using it.
 * 
 * Also allows access to gitlib from ``req.gitlib`` and the user's authentication
 * information from ``req.state.user``
 */
async function authenticated(req, res, next) {

  // Get the session token from the cookies
  const token = req.cookies.session;

  // If we got no cookies, we can redirect to the login page.
  if (!token) {
    res.redirect(`/auth/login?src=${req.originalUrl}`);
    return;
  }

  try {

    // Decode the session token which contains the authorization credentials
    const sub = Verify(token);

    req.state = {
      user: sub,
      self: async () => {

        if (userCache[sub.access_token])
          return userCache[sub.access_token];

        const headers = {
          'Content-Type': 'application/x-www-form-urlencoded',
          'Accept-Encoding': 'application/x-www-form-urlencoded'
        };
        

        const userResponse = await axios.get('https://discord.com/api/users/@me', {
          headers: {
            Authorization: `Bearer ${sub.access_token}`,
            ...headers
          }
        });
      
        userCache[sub.access_token] = userResponse.data;

        return userResponse.data;
        
      } 
    }
    
    next();

  } catch (e) {

    res.redirect('/auth/login');
    return;

  }

}

module.exports = authenticated;