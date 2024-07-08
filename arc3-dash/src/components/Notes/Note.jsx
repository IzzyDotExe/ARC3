import {timeAgo} from '../../lib/utils.js';
import {TranscriptMemberComponent} from '../Transcripts/TranscriptMemberComponent.jsx';
import MemberLabel from "../Util/MemberLabel";
import {useEffect, useState} from "react";
import axios from "axios";
import "./Note.css"

export function Note({ note, lid}) {

    const date = timeAgo(parseInt(note.date)*1000);

    const [user, setUser] = useState({});
    const [author, setAuthor] = useState({});
    useEffect(( ) => {

        console.log(note)

        if (note) {
            axios.get(`/api/discord/users/${note.usersnowflake}/`).then(res => {
                setUser(res.data);
            })
            axios.get(`/api/discord/users/${note.authorsnowflake}/`).then(res => {
                setAuthor(res.data);
            })
        }

    }, [note])

    return (
        <div className="note"  id={`note-${lid}`}>

            <div className="note-top">
                <MemberLabel user={user} placement="right"/>
                <p>{date}</p>
            </div>
            <div className="note-middle">
                <p>{note.note}</p>
            </div>
            <div className="note-bottom">
                <p>Added by: {author ? author.username : "not found"}</p>
            </div>

        </div>

    )
}