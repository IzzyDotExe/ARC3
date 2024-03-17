const express = require('express');
const v1 = require('./v1/v1.js');

const app = express();
app.use(express.json());
app.use(express.urlencoded({extended: true}));

app.use('/api', v1);

app.use('/static', express.static(process.env.BUILD_PATH+'/static'?? './build/static'))

app.get('/*', (req, res) => {
  res.sendFile('index.html', { root: process.env.BUILD_PATH?? "./build" });
});

module.exports = app;
