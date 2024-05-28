const mongoose = require('mongoose');

const TranscriptSchema = new mongoose.Schema({

  modmailId: String,
  sendersnowflake: String,
  attachments: [String],
  createdat: Date,
  GuildSnowflake: String,
  messagecontent: String,
  transcripttype: String,
  comment: Boolean
});

const Transcript = mongoose.model('Transcript', TranscriptSchema);

module.exports = Transcript;