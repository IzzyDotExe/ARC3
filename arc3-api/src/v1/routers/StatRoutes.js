const express = require("express");
const router = express.Router();

const authenticated = require('../../auth/middlewares/authenticated.js');
const { GetCommands } = require('../controllers/StatController.js')


router.get('/', authenticated, GetCommands);

module.exports = router;