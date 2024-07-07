
import { useParams } from 'react-router';
import './Home.css'

import axios from 'axios'

import { useState, useEffect } from 'react'

import GuildInfoBox from '../components/Guild/GuildInfoBox.jsx'
import InsightsInfoBox from '../components/Insights/InsightsInfoBox.jsx'
import CommandStatInfoBox from "../components/Stats/CommandStatInfoBox";
import Infobox from "../components/Util/Infobox";

export default function Home() {

  const {guildid} = useParams()
  const [guild, setGuild ] = useState({})
  const [stats, setStats] = useState([])

  useEffect(() => {

    if (guildid) {
      axios.get(`/api/discord/guilds/${guildid}/`).then(res => {
        setGuild(res.data);
      })

      axios.get(`/api/stats?guildid=${guildid}`).then(res => {
        setStats((res.data))
      })
    }


  }, [guildid, setGuild, setStats])

  return (
    <div className="home">
      { guildid && <>
        <section className="left">
          <GuildInfoBox stats={stats} guild={guild}/>
          <CommandStatInfoBox stats={stats}/>
        </section>
        <section className="right">
          <InsightsInfoBox guild={guild}/>
        </section>
      </>
      }
    </div>
  );
};