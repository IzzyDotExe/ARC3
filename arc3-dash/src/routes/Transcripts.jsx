import Navbar from '../components/Navbar';
import { useState, useEffect} from 'react';
import axios from 'axios'
import { Outlet } from "react-router-dom";
import './Transcripts.css'
import './Transcript.css'

const timeAgo = (prevDate) => {
  const diff = Number(new Date()) - prevDate;
  const minute = 60 * 1000;
  const hour = minute * 60;
  const day = hour * 24;
  const month = day * 30;
  const year = day * 365;
  switch (true) {
      case diff < minute:
          const seconds = Math.round(diff / 1000);
           return `${seconds} ${seconds > 1 ? 'seconds' : 'second'} ago`
      case diff < hour:
          return Math.round(diff / minute) + ' minute(s) ago';
      case diff < day:
          return Math.round(diff / hour) + ' hour(s) ago';
      case diff < month:
          return Math.round(diff / day) + ' day(s) ago';
      case diff < year:
          return Math.round(diff / month) + ' month(s) ago';
      case diff > year:
          return Math.round(diff / year) + ' year(s) ago';
      default:
          return "";
  }
};

function toggleSidebar() {
  const sidebar = document.querySelector('.side-bar');
  if (sidebar.style.display === 'flex')
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
}

function User({userid}) {

  const [user, setUser] = useState(null);

  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=128` : "/blank-avatar.jpg"; 
  const UserName = user ? user.username : "User";

  useEffect(() => {
    axios.get(`/api/discord/users/${userid}/`).then(res => {
      setUser(res.data);
    })
  }, [userid])

  return (<div className="user-tr">
    <img className="icon-tr" src={Avatar} alt={`${UserName}'s img`}/>
    <p>{UserName}</p>
  </div>)
}

function MailView({ data, key }) {

  const date = new Date(data.date);

  return (
    <a href={'/transcripts/'+data.modmailId}>
      <div className="tr"  key={key}> 

        <p>Mail {timeAgo(date)}</p>
        <p>With</p>
        {
          data.participants.map((x, i, d) => {
            return <User userid={x} key={i}/>
          })
        }

      </div>
    </a>
  )
}

export default function Transcripts() {

  const [transcripts, setTranscripts] = useState([]);

  useEffect(() => {

    axios.get('/api/transcripts/').then(res => {
      setTranscripts(res.data);
    })

  }, [])

  return (
    <>
    <div className="App">
      <title>Transcript</title>
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.15.6/styles/solarized-dark.min.css" />
      <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.15.6/highlight.min.js"></script>
      <script src="https://cdnjs.cloudflare.com/ajax/libs/lottie-web/5.8.1/lottie.min.js"></script>

      <Navbar/>
      <main>
        <div className="side-bar">
          <button onClick={toggleSidebar}>Close</button>
          <h1>Transcripts</h1>
          {transcripts.map((x, i, a) =>
            <MailView data={x} key={i}/>
          )}
        </div>
        <div className="transcript-body">
          <Outlet/>
        </div>
      </main>
    </div>
    </>
  );
};