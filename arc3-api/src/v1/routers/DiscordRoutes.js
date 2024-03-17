const express = require('express');
const router = express.Router();

const { GetUser, GetGuild } = require('../controllers/DiscordControllers.js');

router.get('/users/:id', GetUser);
router.get('/guilds/:id', GetGuild);

module.exports = router;