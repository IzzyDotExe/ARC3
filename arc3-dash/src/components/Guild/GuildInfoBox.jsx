import InfoBox from '../Util/Infobox.jsx'
import './GuildInfoBox.css'
import Infobox from "../Util/Infobox.jsx";

export default function GuildInfoBox({guild, stats}) {

  const premium  = guild.data && guild.data.premium? "Enabled" : "Disabled"
  const GuildIcon = guild && guild.icon? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.png"; 
  return <InfoBox>
    <div className="guild-infobox">
      <img src={GuildIcon} alt="Icon"/>
      <div className="info">
        <h2>{guild.name? guild.name : "Guild Loading..."}</h2>
        <p>{guild.approximate_member_count} members</p>
        <p>{stats.length} Commands run</p>
        <p>Premium {premium}</p>
        <a className="button" href={`/${guild.id}/transcripts`}>Transcripts</a>
        <a className="button" href={`/${guild.id}/notes`}>User Notes</a>
      </div>
    </div>
  </InfoBox>
}
