
import './Infobox.css'

export default function Infobox( { children, inner } ) {

  return <div className={inner? "inner-infobox" : "infobox"}>
    {children}
  </div>

}
