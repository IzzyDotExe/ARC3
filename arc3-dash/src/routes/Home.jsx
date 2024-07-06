
import { useParams } from 'react-router';
import './Home.css'

import axios from 'axios'

import { useState, useEffect } from 'react'

import Infobox from '../components/Infobox.jsx'
import GuildInfoBox from '../components/Guilds/GuildInfoBox.jsx'
import InsightsInfoBox from '../components/Insights/InsightsInfoBox.jsx'

export default function Home() {

  const {guildid} = useParams()
  const [guild, setGuild ] = useState({})

  useEffect(() => {

    axios.get(`/api/discord/guilds/${guildid}/`).then(res => {
      setGuild(res.data);
    })

  }, [guildid, setGuild])

  return (
    <div className="home">
      <section className="left">
        <GuildInfoBox guild={guild}/>
        <Infobox>
          a
        </Infobox>
      </section>
      <section className="right">
        <InsightsInfoBox guild={guild}/>
      </section>
    </div>
  );
};