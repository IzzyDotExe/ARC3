import { useState, useEffect, useCallback } from "react";
import MemberLabel from "../Util/MemberLabel";
import axios from "axios";

import "./NoteUser.css"
import {isEmptyOrSpaces} from "../../lib/utils";

export function NoteUser({ userid, guildid, notes, lid, setNotes, filter}) {

    const [user, setUser] = useState(null)

    const noteClick = useCallback(() => {

        // Show notes from this user on left side
        axios.get(`/api/notes/${guildid}/${userid}`).then(res => {
            setNotes(res.data)
        })

    }, [guildid, userid, setNotes])

    useEffect(() => {

        axios.get(`/api/discord/users/${userid}/`).then(res => {
            setUser(res.data);
        })

    }, [userid])



    return (
        <>
        { !isEmptyOrSpaces(filter) && user && user.username.toLowerCase().includes(filter.toLowerCase()) &&
        <div onClick={noteClick} id={`note-user-${lid}`} className="note-user">
            <MemberLabel placement="right" user={user}/>
            <p>{notes} Notes</p>
        </div>
        }
        </>

    )
}