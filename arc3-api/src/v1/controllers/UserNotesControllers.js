const UserNote = require('../db/Notes.js');

async function GetUserNotes(req, res) {

  const guildid = req.params.guildid;
  const userid = req.params.userid;

  if (userid === undefined || guildid === undefined) {
    res.status(400);
    res.json({'status': 404, 'error': 'could not find those usernotes'});
    return;
  }

  try {

    const notes = await UserNote.find({ 'guildsnowflake': guildid, 'usersnowflake': userid }).sort({ 'date': 1 });
    res.status(200).json(notes);

  } catch (e) {
    console.error(e.message)

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });
  }


}

async function GetUserNotesBy(req, res) {
  const guildid = req.params.guildid;
}

async function GetUsers(req, res) {
  const guildid = req.params.guildid;
}