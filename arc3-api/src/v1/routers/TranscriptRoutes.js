const express = require('express');
const router = express.Router();

const { GetTranscripts, GetMailIds } = require('../controllers/TranscriptController.js');

// Send the user all the transcripts
router.get('/:mailId', GetTranscripts);
router.get('/', GetMailIds);

module.exports = router;