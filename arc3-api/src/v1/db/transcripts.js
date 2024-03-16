const mongoose = require('mongooose');

const TranscriptSchema = new mongoose.Schema({
  
  ID: String,
  modmailId: String,
  sendersnowflake: Number,
  attachments: [String],
  createdat: Date,
  GuildSnowflake: Number,
  messagecontent: String

});

const Transcript = mongoose.model('transcripts', TranscriptSchema);

module.exports = Transcript;