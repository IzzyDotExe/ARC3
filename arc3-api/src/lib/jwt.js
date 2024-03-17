const jwt = require('jsonwebtoken');

const Sign = (object) => jwt.sign(object, process.env.JWT_SECRET, { expiresIn: "2h" });
const Verify = (token) => jwt.verify(token, process.env.JWT_SECRET);

module.exports = {
  Sign,
  Verify
}