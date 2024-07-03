import Navbar from '../components/Navbar';
import { useState, useEffect} from 'react';
import axios from 'axios';
import { useParams } from 'react-router';

import Guild from '../components/Guild.jsx';

export default function Home() {

  const {guildid} = useParams()

  const [guilds, setGuilds] = useState([])
  const [guild, setGuild] = useState({})
  
  useEffect(() => {

    if (!guildid)
      axios.get(`/api/discord/me/guilds`).then(res => {
        setGuilds(res.data);
      })

    else 
      axios.get(`/api/discord/guilds/${guildid}`).then(res => {
        setGuild(res.data)
      })

  }, [guildid])

  return (
    <div className="App">
      <Navbar guild={guildid}/>
      <main>
        <h1>Home</h1>
        
        {!guildid && <div className="guilds">
          {guilds.map(x => {
            return <a href={x.id}><Guild guild={x}/></a>
          })}
        </div>}

        {
          guildid &&
          <Guild guild={guild}/>
        }

      </main>
    </div>
  );
};