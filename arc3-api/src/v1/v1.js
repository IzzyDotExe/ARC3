const express = require('express');
const Transcripts = require('./routers/TranscriptRoutes.js');
const Discord = require('./routers/DiscordRoutes.js');
const Appeals = require('./routers/AppealsRoutes.js');
const Notes = require('./routers/UsernoteRoutes.js');
const Insights = require('./routers/InsightRoutes.js');
const Stats = require('./routers/StatRoutes.js');

const router = express.Router();

router.use(express.json());
router.use(express.urlencoded({extended: true}));

router.use('/transcripts', Transcripts);
router.use('/discord', Discord);
router.use('/appeals', Appeals);
router.use('/notes', Notes);
router.use('/insights', Insights);
router.use('/stats', Stats);


router.use((req, res) => {
  const errObj = {'status': 404, 'error': 'Page not found'};
  res.status(404);
  res.json(errObj);
});

module.exports = router;