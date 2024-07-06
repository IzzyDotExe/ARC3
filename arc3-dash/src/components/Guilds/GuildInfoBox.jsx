import InfoBox from '../Infobox.jsx'
import './GuildInfoBox.css'

export default function GuildInfoBox({guild}) {

  
  const GuildIcon = guild && guild.icon? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.png"; 
  return <InfoBox>
    <div className="guild-infobox">
      <img src={GuildIcon} alt="Icon"/>
      <div className="info">
        <h2>{guild.name? guild.name : "Guild Loading..."}</h2>
        <p>{guild.approximate_member_count} members</p>
        <p>0 Commands run</p>
        <p>Premium Enabled</p>
      </div>
    </div>
  </InfoBox>
}