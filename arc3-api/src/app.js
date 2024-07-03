const express = require('express');
const helmet = require('helmet');
const cookieParser = require('cookie-parser')
const app = express();
const fs = require('fs');
const authenticated = require('./auth/middlewares/authenticated.js');
const whitelist = require('./auth/middlewares/whitelist.js');

const v1 = require('./v1/v1.js');
const auth = require('./auth/auth.js');

const STATIC_FILES = fs.readdirSync(process.env.BUILD_PATH);

app.use(
  helmet({
    contentSecurityPolicy: {
      directives: {
        ...helmet.contentSecurityPolicy.getDefaultDirectives(),
        "img-src": ["'self'", "cdn.discordapp.com", "st3.depositphotos.com"],
      },
    },
  })
);
app.use(cookieParser());
app.use(express.json());
app.use(express.urlencoded({extended: true}));

app.use('/api', v1);
app.use('/auth', auth);

app.use('/static', express.static(process.env.BUILD_PATH+'/static'?? './build/static'))

app.get('/login', (req, res) => {
  res.sendFile('login.html', { root: process.env.BUILD_PATH?? "./build" });
})

// Protect the transcripts route.
app.get('/:guildid/transcripts/', authenticated, whitelist, (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
})


// Protect the transcripts route.
app.get('/:guildid/transcripts/*', authenticated, whitelist, (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
})

// Protect the notes routes.
app.get('/:guildid/notes/', authenticated, whitelist, (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
})

// Protect the notes routes.
app.get('/:guildid/notes/*', authenticated, whitelist, (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
})

// Authenticate the rest of the client.
app.get('/*',  (req, res, next) => {

  const file = req.path.split('/')[1];
  if (STATIC_FILES.includes(file)) {
    res.sendFile(file, {root: process.env.BUILD_PATH?? "./build"})
    return;
  }

  authenticated(req, res, () => {
    res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
  });

});

module.exports = app;
