
import './Infobox.css'

export default function Infobox( { children, inner, onClick, outline } ) {

  if (!onClick)
      onClick = () => {}

  return <div style={outline? {border: "solid white 2px"} : {}} onClick={onClick} className={inner? "inner-infobox" : "infobox"}>
    {children}
  </div>

}
