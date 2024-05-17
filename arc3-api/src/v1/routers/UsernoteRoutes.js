const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js');

const { GetUsers, GetUserNotes, GetUserNotesBy } = require('../controllers/UserNotesControllers.js') 


router.get('/:guildid/:userid', authenticated, GetUserNotes);
router.get('/:guildid', authenticated, GetUsers);
router.get('/:guildid/by/:userid', authenticated, GetUserNotesBy);

module.exports = router;