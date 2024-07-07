import './MemberLabel.css'

export default function MemberLabel({user, placement}) {
  placement = placement?? "left";
  const Avatar = user ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png?size=128` : "/missing.png"; 
  const User = user? user.username: "user"
  return (<div className="member-label">
    {placement === "left" && <h2>{User}</h2>}
    <img draggable="false" className="member-icon" src={Avatar} alt="Avatar" />
    {placement === "right" && <h2>{User}</h2>}
  </div>)
}