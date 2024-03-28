const mongoose = require('mongoose');

const AppealSchema = new mongoose.Schema({
    _id: String,
    userSnowflake: String,
    bannedBy: String,
    action: String,
    appealContent: String,
    nextappeal: String
});

const Appeal = mongoose.model('Appeal', AppealSchema);

module.exports = Appeal;