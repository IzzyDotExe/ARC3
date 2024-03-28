const express = require("express");
const router = express.Router();
const { GetAppeal, SubmitAppeal, GetAppeals} = require('../controllers/AppealsControllers.js');
const { GetComments, SubmitComment } = require('../controllers/CommentsControllers.js');

const authenticated = require('../../auth/middlewares/authenticated.js')
// Routers for the ban appeals
router.get('/', authenticated, GetAppeals);
router.get('/:id', authenticated, GetAppeal);
router.post('/', authenticated, SubmitAppeal);
router.get('/:id/comments', authenticated, GetComments);
router.post('/:id/comments', authenticated, SubmitComment);

module.exports = router;
