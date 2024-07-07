
import { useParams } from 'react-router';
import './Home.css'

import axios from 'axios'

import { useState, useEffect } from 'react'

import Infobox from '../components/Util/Infobox.jsx'
import GuildInfoBox from '../components/Guild/GuildInfoBox.jsx'
import InsightsInfoBox from '../components/Insights/InsightsInfoBox.jsx'

export default function Home() {

  const {guildid} = useParams()
  const [guild, setGuild ] = useState({})
  const [stats, setStats] = useState([])

  useEffect(() => {

    axios.get(`/api/discord/guilds/${guildid}/`).then(res => {
      setGuild(res.data);
    })

    axios.get(`/api/stats?guildid=${guildid}`).then(res => {
      setStats((res.data))
    })

  }, [guildid, setGuild, setStats])

  return (
    <div className="home">
      <section className="left">
        <GuildInfoBox stats={stats} guild={guild}/>
        <Infobox>
            {
                stats.map(x => {
                    let argsl = "";
                    for (let arg in x.args) {
                        argsl += `${arg}: ${x.args[arg]} `
                    }
                    return <p>/{x.command_name} {argsl}</p>
                })
            }
        </Infobox>
      </section>
      <section className="right">
        <InsightsInfoBox guild={guild}/>
      </section>
    </div>
  );
};