import InfoBox from '../Util/Infobox';
import './Insight.css'
export default function Insight({ children, insight }) {
  return (  
    <div className="insight">
      <InfoBox inner={true}>
        <h3>{insight.tagline}</h3>
        <div className="content">
          {children}
        </div>
      </InfoBox>
    </div>      
  )
}