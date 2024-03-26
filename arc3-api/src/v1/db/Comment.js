const mongoose = require('mongoose');

const CommentSchema = new mongoose.Schema({
  userSnowflake: String,
  appealId: String,
  commentContents: String,
  commentDate: String
})

const Comment = mongoose.model('Comment', CommentSchema);

module.exports = Comment;