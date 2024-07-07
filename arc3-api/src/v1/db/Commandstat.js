const mongoose = require('mongoose');

const commandStatSchema = new mongoose.Schema({
    _id: String,
    guild_id: String,
    args: Object,
    command_name: String
}, {
    collection: "Commandstats"
})

const CommandStat = mongoose.model("Commandstat", commandStatSchema);

module.exports = CommandStat;