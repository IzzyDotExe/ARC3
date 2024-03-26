const express = require("express");
const router = express.Router();
const authenticated = require('../../auth/middlewares/authenticated');
const { GetAppeal, SubmitAppeal, GetAppeals} = require('../controllers/AppealsControllers.js');
const { GetComments, SubmitComment } = require('../controllers/CommentsControllers.js');
// Routers for the ban appeals
router.get('/', authenticated, GetAppeals);
router.get('/:id', authenticated, GetAppeal);
router.post('/', authenticated, SubmitAppeal);
router.get('/comments/:id', GetComments);
router.post('/comments', SubmitComment);

module.exports = router;
