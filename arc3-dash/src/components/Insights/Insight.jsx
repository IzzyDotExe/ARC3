import InfoBox from '../Util/Infobox';
import './Insight.css'
import { timeAgo } from "../../lib/utils";


export default function Insight({ children, insight }) {
  return (
      <div className="insight">
          <p className="timestamp">{timeAgo(insight.date * 1000)}</p>
          <InfoBox inner={true}>

              <h3>{insight.tagline}</h3>
              <div className="content">
                  {children}
              </div>
          </InfoBox>
      </div>
  )
}