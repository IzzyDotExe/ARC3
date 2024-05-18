const Transcript = require('../db/Transcript.js');

async function GetTranscripts(req, res) {

  const modmailID = req.params.mailId;
  const guild = req.params.guildid;

  // Guard if the modmail id is undefined
  if (modmailID === undefined || guild === undefined) {
    res.status(404);
    res.json({'status': 404, 'error': 'Could not find that modmail'});
    return;
  }

  // Try and get the transcripts and send them
  try {

    const transcripts = await Transcript.find({ 'modmailId': modmailID, 'GuildSnowflake': guild }).sort({'createdat': 1});
    res.status(200).json(transcripts);

  } catch (e) {

    console.error(e.message);

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });

  }

}

async function GetMailIds(req, res) {

  const guild = req.params.guildid;

  // Guard if the modmail id is undefined
  if (guild === undefined) {
    res.status(404);
    res.json({'status': 404, 'error': 'Could not find that modmail'});
    return;
  }


  try {
    let modmailIds = await Transcript.find({'GuildSnowflake': guild}).aggregate([
      { 
        $group: {
          "_id": {

            modmailId: "$modmailId",
            transcripttype: "$transcripttype",
            GuildSnowflake: {
              "$toString": "$GuildSnowflake"
            }
          },
          Participants: {
            "$push": {
              sendersnowflake: {
                "$toString" : "$sendersnowflake"
              }
            }
          },
          Created: {
            "$first": {
              createdAt: "$createdat"
            }
          }

        },

      }
    ]);

    modmailIds = modmailIds.map(x => {
      return {
        ...x["_id"],
        participants: x["Participants"].map(x => x["sendersnowflake"]).filter((value, index, array) => {
          return array.indexOf(value) === index;
        }),
        date: x["Created"]["createdAt"]
      }
    })
    
    res.status(200).json(modmailIds)
  } catch (e) {
    console.error(e.message);

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });
  }

}

module.exports = { GetTranscripts, GetMailIds };