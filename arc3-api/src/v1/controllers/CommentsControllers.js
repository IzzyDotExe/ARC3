const Comment = require('../db/Comment');

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
    const comments = await Comment.find();
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

  let content = req.body.content;
  const self = await req.state.self()

  if (content === undefined) {
    res.status(400);
    res.json({
        status: 400,
        error: "Failed to submit, fill in all fields."
    });
  }



}

module.exports = { GetComments, SubmitComment }
