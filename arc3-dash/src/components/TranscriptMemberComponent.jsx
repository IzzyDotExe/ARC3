import {useEffect, useState} from 'react';
import axios from 'axios';

export function TranscriptMemberComponent({userid}) {

  const [user, setUser] = useState(null);

  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=128` : "/blank-avatar.jpg"; 
  const UserName = user ? user.username : "User";

  useEffect(() => {
    axios.get(`/api/discord/users/${userid}/`).then(res => {
      setUser(res.data);
    })
  }, [userid])

  return (
    <div className="user-tr">
      <img className="icon-tr" src={Avatar} alt={`${UserName}'s img`}/>
      <p>{UserName}</p>
    </div>
  )

}