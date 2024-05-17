import axios from 'axios';
import { useState, useEffect } from 'react'; 

export function TranscriptMessage({data, key}) {

  const [user, setUser] = useState(null);

  const UserName = user ? user.username : "User";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=1024` : "/blank-avatar.jpg"; 
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
