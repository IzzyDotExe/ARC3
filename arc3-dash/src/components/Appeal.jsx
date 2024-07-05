import Navbar from '../components/Navbar'
import { useState, useEffect, useCallback } from 'react' 
import axios from 'axios'
import unescape from 'unescape'

function Comment({data}) {

  const [user, setUser] = useState(null);
  const contents = data.commentContents;

  const UserName = user ? user.username : "User";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=1024` : "/missing.png"; 
  const date = new Date(data.commentDate);
  const formattedDate = date.toLocaleString('en-US', { timeZoneName: 'short' });

  useEffect(() => {
    axios.get(`/api/discord/users/${data.userSnowflake}/`).then(res => {
      setUser(res.data);
    })

  }, [data.userSnowflake])

  return (          
    <div className="comment">
      <div className="title">
        <p>{UserName}</p>
        <img src={Avatar} alt="" />
      </div>
      <div className="contents">
        <p>{unescape(contents)}</p>
      </div>
    </div>
  )
}

export default function Appeal({ self, data }) {

  const [user, setUser] = useState(null);
  const [dropdown, setDropdown] = useState(false);
  const [comments, setComments] = useState([]);

  const UserName = user ? user.username : "User";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=1024` : "/blank-avatar.jpg"; 
  const date = new Date(data.nextappeal);
  const formattedDate = date.toLocaleString('en-US', { timeZoneName: 'short' });

  const selfName = self? self.username : "User";
  const selfav = self ? `https://cdn.discordapp.com/avatars/${self.id}/${self.avatar}.png?size=1024` : "/blank-avatar.jpg"; 

  const openComment = useCallback(() => {
    setDropdown(val => !val);
  }, []);

  useEffect(() => {

    axios.get(`/api/discord/users/${data.userSnowflake}/`).then(res => {
      setUser(res.data);
    })

    axios.get(`/api/appeals/${data._id}/comments`).then(res => {
      setComments(res.data);
    })

  }, [data.userSnowflake, data._id])


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
          <h2>Comments</h2>
          <button onClick={openComment}>{!dropdown? "Open comments" : "Close comments"}</button>
          {dropdown && comments.map(data => <Comment data={data}/>)}
          
        </div>
        <div className="commentfield">
      
          <div>
            <img src={selfav} alt="" />
            <p>Commenting as {selfName}</p>
          </div>

          <form method="POST" action={`/api/appeals/${data._id}/comments`}>
            <textarea name="content" id="content" cols="30" rows="10"></textarea>
            <button type="submit" className="button secondary">Send</button>
          </form>

        </div>
      </div>
    </section>
  )
}