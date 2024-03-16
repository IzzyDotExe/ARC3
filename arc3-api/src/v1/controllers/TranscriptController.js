const Transcript = require('../db/Transcript.js');

async function GetTranscripts(req, res) {
  const modmailID = req.params.mailId;

  // Guard if the modmail id is undefined
  if (modmailID === undefined) {
    res.status(404);
    res.json({'status': 404, 'error': 'Could not find that modmail'});
  }

  // Try and get the transcripts and send them
  try {

    const transcripts = await Transcript.find({ 'modmailId': modmailID});
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
  try {
    const modmailIds = await Transcript.distinct('modmailId');
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