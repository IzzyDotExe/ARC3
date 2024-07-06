import InfoBox from '../Infobox';

export default function Insight({ children, insight }) {
  return (  
    <div className="insight">
      <InfoBox inner={true}>
        <h3>{insight.tagline}</h3>
        {children}
      </InfoBox>
    </div>      
  )
}