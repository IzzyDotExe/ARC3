import { useParams } from 'react-router';
import { useState, useEffect } from 'react';
import axios from 'axios';

const reqCache = {

}

function setSideBar(set) {
  const sidebar = document.querySelector('.side-bar');
  if (!set)
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
}

function toggleSidebar() {
  const sidebar = document.querySelector('.side-bar');
  if (sidebar.style.display === 'flex')
    sidebar.style.display = 'none';
  else 
    sidebar.style.display = 'flex';
}

function Message({data, key}) {

  const [user, setUser] = useState(null);

  const UserName = user ? user.username : "User";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=1024` : "https://cdn.discordapp.com/avatars/964332892094341150/33ab55d7da71c325d56d820a7810ae15.png?size=1024"; 
  const date = new Date(data.createdat);
  const formattedDate = date.toLocaleString('en-US', { timeZoneName: 'short' });

  useEffect(() => {

    axios.get(`/api/discord/users/${data.sendersnowflake}/`).then(res => {
      setUser(res.data);
    })

  }, [data.sendersnowflake])

  return (
    <>
      <div id={key} class="chatlog__message-group">
        <div className="chatlog__message-container">
          <div className="chatlog__message">
            <div className="chatlog__message-aside">
              <img className="chatlog__avatar"
                src={Avatar}
                alt="Avatar" loading="lazy"/>
            </div>
            <div className="chatlog__message-primary">
              <div className="chatlog__header">
                <span className="chatlog__author" style={{color:"rgb(155,89,182)"}} title={UserName} data-user-id={data.sendersnowflake}>{UserName}</span> 
                <span className="chatlog__timestamp"><a href={`#${key}`}>{formattedDate}</a></span>
              </div>
              <div class='chatlog__content chatlog__markdown'>
                <span className="chatlog__markdown-preserve">{data.messagecontent}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}


export default function Transcript() {

  // Get the ID of the modmail
  const mailId = useParams()['*'];
  
  const [modmail, setModmail] = useState([]);
  const [guild, setGuild] = useState(null);

  const GuildIcon = guild? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "https://st3.depositphotos.com/17828278/33150/v/450/depositphotos_331503262-stock-illustration-no-image-vector-symbol-missing.jpg";
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
        return <Message data={x} key={i}/>
      })}</p>

      <div className="postamble">
        <div className="postamble__entry">Saved {modmail.length} message(s)</div>
      </div>

    </>
  );
}