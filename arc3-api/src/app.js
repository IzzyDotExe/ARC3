const express = require('express');
const helmet = require('helmet');
const cookieParser = require('cookie-parser')
const app = express();

const authenticated = require('./auth/middlewares/authenticated.js');
const whitelist = require('./auth/middlewares/whitelist.js');

const v1 = require('./v1/v1.js');
const auth = require('./auth/auth.js');

app.use(helmet());
app.use(cookieParser());
app.use(express.json());
app.use(express.urlencoded({extended: true}));

app.use('/api', v1);
app.use('/auth', auth);

app.use('/static', express.static(process.env.BUILD_PATH+'/static'?? './build/static'))

app.get('/login', (req, res) => {
  res.sendFile('login.html', { root: process.env.BUILD_PATH?? "./build" });
})

app.get('/transcripts/*', authenticated, whitelist, (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
})

app.get('/*', authenticated,  (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
});

module.exports = app;
