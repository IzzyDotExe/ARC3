const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js');
const whitelist = require('../../auth/middlewares/whitelist.js');

const { GetUsers, GetUserNotes, GetUserNotesBy } = require('../controllers/UserNotesControllers.js') 


router.get('/:guildid/:userid', authenticated, whitelist, GetUserNotes);
router.get('/:guildid', authenticated, whitelist, GetUsers);
router.get('/:guildid/by/:userid', authenticated, whitelist, GetUserNotesBy);

module.exports = router;