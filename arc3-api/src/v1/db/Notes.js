const mongoose = require('mongoose');

const userNoteSchema = new mongoose.Schema({
  _id: String,
  usersnowflake: String,
  guildsnowflake: String,
  note: String,
  date: String,
  authorsnowflake: String
});

const UserNote = mongoose.model('user_note', userNoteSchema)

module.exports = UserNote;