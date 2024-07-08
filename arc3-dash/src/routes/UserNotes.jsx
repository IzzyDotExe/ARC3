import { useParams } from 'react-router';
import { useState, useEffect } from 'react';
import axios from 'axios';

import {Note} from '../components/Notes/Note'


export default function UserNotes() {

  const { userid, guild } = useParams();

  const [usernotes, setUserNotes] = useState([])

  useEffect(() => {
    axios.get(`/api/notes/${guild}/${userid}`).then(res => {
      setUserNotes(res.data);
    })
  }, [guild, userid])

  return (
    <>
    {usernotes.map(x => <Note data={x}/>)}
    </>
  )

}