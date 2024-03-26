const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js')

const { GetUser, GetGuild, GetMe } = require('../controllers/DiscordControllers.js');

router.get('/users/:id', authenticated, GetUser);
router.get('/guilds/:id', authenticated, GetGuild);
router.get('/me', authenticated, GetMe);

module.exports = router;