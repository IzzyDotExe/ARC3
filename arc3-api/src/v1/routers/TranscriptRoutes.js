const express = require('express');
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated');

const { GetTranscripts, GetMailIds } = require('../controllers/TranscriptControllers.js');

// Send the user all the transcripts
router.get('/:mailId', authenticated, GetTranscripts);
router.get('/', authenticated, GetMailIds);

module.exports = router;