const Appeal = require('../db/Appeal.js');
const escape = require('escape-html');
const uuid = require('uuid');
const net = require('net');
const io = require('socket.io-client');

const clean = str =>  str.replace(/[^\x00-\x7F]/g, "");

async function GetAppeals(req, res) {
    
    try {
        
        const appeals = await Appeal.find();
        res.status(200).json(appeals);
        
    } catch (e) {
        
        console.error(e.message);
        
        res.status(500);
        res.json({
            'status': 500,
            'error': 'An Error occured. Please try again later.'
        });
        
    }
    
}

async function SubmitAppeal(req, res) {

    let {bannedBy, appealContent} = req.body;

    // Guard if the form is undefined
    if (bannedBy === undefined || appealContent === undefined) {
        res.status(400);
        res.json({
            status: 400,
            error: "Failed to submit, fill in all fields."
        });
        return;
    }

    // Escape and Validate data
    bannedBy = clean(escape(bannedBy));
    appealContent = clean(escape(appealContent));

    const self = await req.state.self();
    // console.log(self);
    
    const existing = await Appeal.find({'userSnowflake': self.id});
    
    try {
        const nextAppeal  = new Date(parseInt(existing[0].nextappeal));
        // console.log(existing)
        if (nextAppeal < new Date()) {
            await Appeal.deleteOne(existing[0]);
            existing.pop(existing[0]);
        }    
    } catch (e) {
        // console.log(e);
    }

    // Guard if the user has already appealed
    if ( existing.length > 0) {

        res.status(400);
        res.json({
            status: 400,
            error: "Failed to submit, already appealed."
        });
        return;
    }

    const date = new Date()
    const date2 = new Date()
    date2.setDate(date.getDate() + 30);

    const appeal = new Appeal({
        _id: uuid.v4(),
        bannedBy: bannedBy,
        action: 'none',
        appealContent: appealContent,
        userSnowflake: self.id,
        nextappeal: date2.getTime()
    });
    
    appeal.save();

    res.status(200)
    res.json({
        status: 200,
        message: "success"
    });
    
}

async function GetAppeal(req, res) {
    
    const id = req.params.id;
    
    // Guard if the id is undefined
    if (id === undefined) {

        res.status(400).json({
            status: 400,
            message: "Invalid id"
        })
        return;
    }
    
    // Try and get the appeal and send it
    try {
        
        const appeal = await Appeal.find({'_id': id});
        res.status(200).json(appeal);
        
    } catch (e) {
        
        console.error(e.message);
        
        res.status(500);
        res.json({
            'status': 500,
            'error': 'An Error occured. Please try again later.'
        });
        
    }
    
}

module.exports = { GetAppeal, SubmitAppeal, GetAppeals };