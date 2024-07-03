

export default function Guild({guild, key}) {

  const GuildIcon = guild && guild.icon? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.jpg"; 
  return (
    <div className="guild">
      
      <img src={GuildIcon} alt="" />
      <p>{guild.name}</p>

    </div>
  )
}