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

async function axiosGet(url, creds) {

  if (!reqCache[url])
    reqCache[url] = await axios.get(url);
  
  return reqCache[url].data;

}

function Message({data, key}) {

  const [user, setUser] = useState(null);

  const UserName = user ? user.username : "User";
  const Avatar = user ? user.avatarUrl : "https://cdn.discordapp.com/avatars/964332892094341150/33ab55d7da71c325d56d820a7810ae15.png?size=1024";

  const date = new Date(data.createdat);
  const formattedDate = date.toLocaleString('en-US', { timeZoneName: 'short' });

  useEffect(() => {



  }, [])

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

  const GuildIcon = "https://st3.depositphotos.com/17828278/33150/v/450/depositphotos_331503262-stock-illustration-no-image-vector-symbol-missing.jpg";
  const SenderName = "SenderUsername";
  const ServerName = "GuildName";

  useEffect(() => {
    setSideBar(false);
    // Fetch all the messages from the transcript
    axios.get(`/api/transcripts/${mailId}/`).then(res => {
      setModmail(res.data);
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
            <div className="preamble__entry">{SenderName}</div>
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