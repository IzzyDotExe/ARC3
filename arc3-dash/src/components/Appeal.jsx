import Navbar from '../components/Navbar'
import './Appeal.css'
import { useState, useEffect } from 'react' 
import axios from 'axios'


export default function Appeal({ self, data }) {

  const [user, setUser] = useState(null);

  const UserName = user ? user.username : "User";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=1024` : "/blank-avatar.jpg"; 
  const date = new Date(data.nextappeal);
  const formattedDate = date.toLocaleString('en-US', { timeZoneName: 'short' });

  const selfName = self? self.username : "User";
  const selfav = self ? `https://cdn.discordapp.com/avatars/${self.id}/${self.avatar}.png?size=1024` : "/blank-avatar.jpg"; 

  useEffect(() => {
    axios.get(`/api/discord/users/${data.userSnowflake}/`).then(res => {
      setUser(res.data);
    })

  }, [data.userSnowflake])


  return (
    <section className="appeal">
      <div className="titlesec">
        <img src={Avatar} alt="user's Avatar" />
        <h3>Appeal from {UserName}</h3>
      </div>
      <div className="infosec">
        <p>Who were you banned by?</p>
        <span className="info">
          <p>{data.bannedBy}</p>
        </span>
        <p>What was the reason for your ban, and why should you be unbanned?</p>
        <span className="info">
          <p>{data.appealContent}</p>
        </span>
      </div>
      <div className="commentsec">
        <div className="buttons">
          <a className="button primary">🔓 Unban</a>
          <a className="button danger">🔨 Deny</a>
          <a className="button secondary">✉️ Mail</a>
        </div>
        <div className="comments">
        </div>
        <div className="comment">
          <div>
            <img src={selfav} alt="" />
            <p>Commenting as {selfName}</p>
          </div>

          <textarea name="" id="" cols="30" rows="10"></textarea>
          <a className="button secondary">Send</a>
        </div>
      </div>
    </section>
  )
}