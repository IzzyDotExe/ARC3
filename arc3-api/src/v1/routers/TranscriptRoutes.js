const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated');
const whitelist = require('../../auth/middlewares/whitelist.js');

const { GetTranscripts, GetMailIds } = require('../controllers/TranscriptControllers.js');

// Send the user all the transcripts
router.get('/:guildid/:mailId', authenticated, whitelist, GetTranscripts);
router.get('/:guildid/', authenticated, whitelist, GetMailIds);

module.exports = router;