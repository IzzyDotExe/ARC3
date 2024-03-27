const Comment = require('../db/Comment');
const clean = str =>  str.replace(/[^\x00-\x7F]/g, "");
const escape = require('escape-html');

async function GetComments(req, res) {

  const id = req.params.id;

  if (id === undefined) {

    res.status(400).json({
      status: 400,
      message: "Invalid id"
    })

    return;
  }

  try {

    const comments = await Comment.find({ 'appealId': id });
    res.status(200).json(comments);

  } catch (e) {

    console.error(e.message);

    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });

  }

}

async function SubmitComment(req, res) {

  const id = req.params.id;
  let content = req.body.content;

  const self = await req.state.self()

  if (content === undefined || id === undefined) {
    res.status(400);
    res.json({
        status: 400,
        error: "Failed to submit, fill in all fields."
    });
    return;
  }

  content = clean(escape(content));
  const date = new Date();

  try {

    const comment = new Comment({
      userSnowflake: self.id,
      appealId: id,
      commentContents: content,
      commentDate: date.getTime()
    })

    comment.save();

    res.status(200);
    res.redirect('/appeals')

  } catch (e) {

    console.error(e.message);
        
    res.status(500);
    res.json({
      'status': 500,
      'error': 'An Error occured. Please try again later.'
    });

  }

}

module.exports = { GetComments, SubmitComment }
