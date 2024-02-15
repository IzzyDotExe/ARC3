const express = require('express');
const v1 = require('./v1/v1.js');

const app = express();
app.use('/api', v1);

app.use(express.static('./build'));

module.exports = app;
