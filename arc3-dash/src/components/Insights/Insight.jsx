import InfoBox from '../Util/Infobox';
import './Insight.css'
import { timeAgo } from "../../lib/utils";
import {useCallback, useEffect, useState} from "react";
import {click} from "@testing-library/user-event/dist/click";


export default function Insight({ children, insight }) {

    let showtime  = false;
    const [read, setRead] = useState((() => {
        if (!localStorage.getItem("insights-read"))
            localStorage.setItem("insights-read", "{}")

        const readStatus = JSON.parse(localStorage.getItem("insights-read"))
        return readStatus[insight._id]
    })())

    if (timeAgo(insight.date * 1000) === Insight.prevtime) {
        showtime = false
    } else {
        Insight.prevtime = timeAgo(insight.date * 1000)
        showtime = true
    }

    const clickEvent = useCallback(() => {

        const readStatus = JSON.parse(localStorage.getItem("insights-read"))

        readStatus[insight._id] = true;
        setRead(true);

        localStorage.setItem("insights-read", JSON.stringify(readStatus))

    }, [insight._id])


  return (
      <div className="insight">
          { showtime && <p className="timestamp">{timeAgo(insight.date * 1000)}</p> }
          <InfoBox outline={!read} onClick={clickEvent} inner={true}>
              <h3>{insight.tagline}</h3>
              <div className="content">
                  {children}
              </div>
          </InfoBox>
      </div>
  )
}