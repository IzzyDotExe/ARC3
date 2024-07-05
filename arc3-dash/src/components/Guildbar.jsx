import './Guildbar.css'
import axios from 'axios'
import { useState, useEffect } from 'react';
import Guild, {IconLabel} from './Guild.jsx'
import {CLIENT_ID} from '../config.js'

export default function Guildbar() {

  const [guilds, setGuilds] = useState([])

  useEffect(() => {

    axios.get(`/api/discord/me/guilds`).then(res => {
      setGuilds(res.data);
    })
      
  }, [])


  return <nav className="guild-bar">

    <div className="guild"> 
      <a draggable="false" href={`https://discord.com/oauth2/authorize?client_id=${CLIENT_ID}&scope=bot&permissions=8`}>
        <IconLabel tag="Add a guild" src="/add.png"/>
      </a>
    </div>

    {guilds.map(x => {
      return <Guild href={x.id} guild={x} />
    })}

  </nav>
}