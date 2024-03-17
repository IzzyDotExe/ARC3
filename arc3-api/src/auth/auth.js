const express = require('express');
const router = express.Router();

const { login, callback, discord, revoke } = require('./controllers/authentication.js');


router.get('/login', login);

router.get('/revoke', revoke);

router.get('/callback', callback);

router.get('/discord', discord)


module.exports = router;