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

    const notes = await UserNote.find();
    res.status(200).json(notes.filter(x => x.guildsnowflake === guildid && ( userid === "all" || x.usersnowflake === userid )));

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
  const userid = req.params.userid;

  if (guildid === undefined || userid === undefined) {
    res.status(400);
    res.json({'status': 404, 'error': 'could not find those usernotes'});
    return;
  }

  try { 

    const notes = await UserNote.find();
    res.status(200).json(notes.filter(x => x.guildsnowflake == guildid ))


  } catch (e) {
    
    console.error(e.message);

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });  

  }

  
}

async function GetUsers(req, res) {
  const guildid = req.params.guildid;
  if (guildid === undefined) {
    res.status(400);
    res.json({'status': 404, 'error': 'could not find those usernotes'});
    return;
  }

  try {
    let userids = await UserNote.find()
    userids = [... new Set(userids.filter(x => x.guildsnowflake == guildid).map(x => x.usersnowflake))]
    res.status(200).json(userids)

  } catch (e) {
    console.error(e.message);

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });
  }
}

module.exports = {GetUsers, GetUserNotes, GetUserNotesBy}