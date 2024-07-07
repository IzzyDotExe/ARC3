const Commandstat = require('../db/Commandstat.js');

async function GetCommands(req, res) {

    let {guildid} = req.query;

    // Guard if the form is undefined
    if (guildid === undefined ) {
        res.status(404);
        res.json({
            status: 404,
            error: "Could not find that guild"
        });
        return;
    }

    try {

        const insights = await Commandstat.find({guild_id: guildid});
        res.status(200).json(insights);

    } catch (e) {

        console.error(e.message);

        res.status(500);
        res.json({
            'status': 500,
            'error': 'An Error occured. Please try again later.'
        });

    }

}

module.exports = { GetCommands: GetCommands }