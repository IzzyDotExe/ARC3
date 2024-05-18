import Navbar from '../components/Navbar';
import { useState, useEffect} from 'react';
import axios from 'axios';
import { useParams } from 'react-router';
import { Outlet } from "react-router-dom";

import './Notes.css'

import {TranscriptMemberComponent} from '../components/TranscriptMemberComponent'

export default function Notes() {

  const {guild} = useParams();
  const [notesUsers, setNoteUsers] = useState([]);

  useEffect(() => {
    axios.get(`/api/notes/${guild}`).then(res => {
      setNoteUsers(res.data);
    })
  }, [guild])

  return (

    <>
    
    <div className="App">

      <title>User Notes</title>

      <Navbar guild={guild}/>

      <main>
        <div className="NotesPick">
          <div className = "sidebar">
            {notesUsers.map(x => <a href={`/${guild}/notes/${x}`}><TranscriptMemberComponent userid={x}/></a>)}
          </div>
          
          <div className = "notes-sec">
            <Outlet/>
          </div>
          
        </div>
      </main>

    </div>
    
    </>

  )

}