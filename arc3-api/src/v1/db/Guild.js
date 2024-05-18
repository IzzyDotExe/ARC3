const mongoose = require('mongoose');

const guildSchema = new mongoose.Schema({
  _id: String,
  guildsnowflake: String, 
  premium: Boolean,
  moderators: [String],
  ownerid: String
}, {
  collection: "Guilds"
});

const Guild = mongoose.model("Guild", guildSchema);

module.exports = Guild;