
import { useState, useEffect, useCallback} from 'react';
import axios from 'axios';
import { useParams } from 'react-router';
import "./Notes.css"

import NoteUserList from "../components/Notes/NotesUserList";
import {Note} from "../components/Notes/Note";

export default function Notes() {

  const {guild, userid} = useParams();
  const [notesUsers, setNoteUsers] = useState([]);

  const [notes, setNotes] = useState([]);
  const [filter, setFilter] = useState("");

  useEffect(() => {
    axios.get(`/api/notes/${guild}/all`).then(res => {
      setNoteUsers(res.data);
    })

  }, [guild])


  return (
      <div className="notes">
          <section className="right">
              <h2>Users</h2>
              <input onChange={(e) => {
                  setFilter(e.target.value)
              }}/>
              <section className="notes-user-list">
                  <NoteUserList filter={filter} setNotes={setNotes} notesUsers={notesUsers}/>
              </section>
          </section>
          <section className="left">
              <h2>Notes</h2>
                <NotesDisplay filter={filter} notes={notes}/>
          </section>
      </div>

  )

}


function NotesDisplay({notes, filter}) {

    return <section className="notes-display">
        {
            notes.sort((a, b) => {
                return parseInt(b.date) - parseInt(a.date)
            }).map((x, i) => {
                return <Note note={x} lid={i} key={i}/>
            })
        }
    </section>
}