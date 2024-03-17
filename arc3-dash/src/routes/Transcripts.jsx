import Navbar from '../components/Navbar';
import { useState, useEffect} from 'react';
import axios from 'axios'
import { Outlet } from "react-router-dom";
import './Transcripts.css'
import './Transcript.css'

function toggleSidebar() {
  const sidebar = document.querySelector('.side-bar');
  if (sidebar.style.display === 'flex')
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
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
            <div key={i}>
              <a href={'/transcripts/'+x.modmailId}>{x.modmailId}</a>
              <br/>
            </div>
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