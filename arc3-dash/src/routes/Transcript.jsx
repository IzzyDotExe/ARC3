import { useParams } from 'react-router';
import { useState, useEffect } from 'react';
import axios from 'axios';

import { toggleSidebar, setSideBar } from '../lib/domactions.js';
import { TranscriptMessage } from '../components/TranscriptMessage';


export default function Transcript() {

  // Get the ID of the modmail
  const mailId = useParams()['*'];
  
  const [modmail, setModmail] = useState([]);
  const [guild, setGuild] = useState(null);

  const GuildIcon = guild? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.jpg"; 
  const ServerName = guild? guild.name : "GuildName";

  useEffect(() => {
    setSideBar(false);
    // Fetch all the messages from the transcript
    axios.get(`/api/transcripts/${mailId}/`).then(res => {
      setModmail(res.data);
      axios.get(`/api/discord/guilds/${res.data[0].GuildSnowflake}/`).then(res => {
        setGuild(res.data);
      })
    });

  }, [mailId]);

  return (
    <>
      <div className="preamble">
          
        <div className="preamble__guild-icon-container">
          <img className="preamble__guild-icon"
               src={GuildIcon} alt=' Guild icon' loading="lazy"/>
        </div>

        <div className="preamble__entries-container">
            <div className="preamble__entry">{ServerName}</div>
            <div className="preamble__entry"><button onClick={toggleSidebar}>Transcripts</button></div>
            <div className="preamble__entry">{mailId}</div>
        </div>

      </div>

      <p>{modmail.map((x, i, d) => {
        return <TranscriptMessage data={x} key={i}/>
      })}</p>

      <div className="postamble">
        <div className="postamble__entry">Saved {modmail.length} message(s)</div>
      </div>

    </>
  );
}