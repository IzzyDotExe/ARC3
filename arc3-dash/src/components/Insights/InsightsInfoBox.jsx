import InfoBox from '../Infobox.jsx'
import './InsightsInfoBox.css'
import Insight from './Insight.jsx'

import axios from 'axios'

import { useState, useEffect, useCallback } from 'react'

export default function GuildInfoBox({guild}) {

  const [insights, setInsights] = useState([])

  const insightsBox = useCallback(() => {

    if (insights.length === 0)
      return <p>No Insights found</p>

    const insightElements = []
    
    insights.forEach(insight => {
      insightElements.push(
        <Insight insight={insight} />
      )
    })
    
    return insightElements
  });

  useEffect(() => {

    if (guild.id)
      axios.get(`/api/insights?guildid=${guild.id}`).then(res => {
        setInsights(res.data)
        console.log(res.data)
      })

  }, [guild.id])

  return <InfoBox>
    <h1>Insights</h1>
    <div className="insights-infobox">
      {insightsBox()}
    </div>
  </InfoBox>
}