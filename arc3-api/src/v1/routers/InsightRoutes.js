const express = require("express");
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js');
const { GetInsights } = require('../controllers/InsightsController.js')


router.get('/', authenticated, GetInsights);

module.exports = router;