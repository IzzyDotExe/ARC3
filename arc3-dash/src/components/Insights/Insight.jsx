import InfoBox from '../Util/Infobox';
import './Insight.css'
import { timeAgo } from "../../lib/utils";


export default function Insight({ children, insight }) {

    let showtime  = false;

    console.log(Insight.prevtime)

    if (timeAgo(insight.date * 1000) === Insight.prevtime) {
        showtime = false
    } else {
        Insight.prevtime = timeAgo(insight.date * 1000)
        showtime = true
    }


  return (
      <div className="insight">
          { showtime && <p className="timestamp">{timeAgo(insight.date * 1000)}</p> }
          <InfoBox inner={true}>

              <h3>{insight.tagline}</h3>
              <div className="content">
                  {children}
              </div>
          </InfoBox>
      </div>
  )
}