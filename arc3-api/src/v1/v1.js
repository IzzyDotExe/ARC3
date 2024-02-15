const express = require('express');

const router = express.Router();
router.use(express.json());
router.use(express.urlencoded({extended: true}));


router.use((req, res) => {
  const errObj = {'status': 404, 'error': 'Page not found'};
  res.status(404);
  res.json(errObj);
});

module.exports = router;