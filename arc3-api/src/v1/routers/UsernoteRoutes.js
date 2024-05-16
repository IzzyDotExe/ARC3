const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js');

const { GetUserNotes, GetUserNotesBy, GetUsers } = require('../controllers/UserNotesControllers.js') 


router.get('/:guildid/:userid', authenticated, GetUserNotes);
router.get('/:guildid', authenticated, GetUsers);
router.get('/:guildid/:auhorid', authenticated, GetUserNotesBy);