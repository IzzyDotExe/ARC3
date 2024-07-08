import {NoteUser} from "./NoteUser";
import './NotesUserList.css'
import {useEffect, useState} from "react";

export default function NoteUserList({notesUsers, setNotes, filter}) {

    let uniqueUsers = [...notesUsers]

    uniqueUsers = uniqueUsers.filter((elem, index) => {
        return uniqueUsers.findIndex(obj => obj.usersnowflake === elem.usersnowflake) === index
    })

    return <> {uniqueUsers.map((x, i, a)=> {
        return <NoteUser filter={filter} guildid={notesUsers[0].guildsnowflake} setNotes={setNotes} lid={i} key={i} userid={x.usersnowflake} notes={notesUsers.filter(y => y.usersnowflake === x.usersnowflake).length}/>
    })}</>

}