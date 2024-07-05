import "./Guild.css"

export default function Guild({guild, key, href}) {

  const GuildIcon = guild && guild.icon? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.png"; 

  return (
    <div className="guild">
      {href && <a draggable="false" href={href}>
        <IconLabel tag={guild.name} src={GuildIcon}/>
      </a>}

      {!href && <>
        <IconLabel tag={guild.name} src={GuildIcon}/>
      </>}
    </div>
  )
}

export function IconLabel({tag, src}) {
  return (<>
    <img draggable="false" src={src} alt="" />
    <p>{tag}</p>
  </>)
}